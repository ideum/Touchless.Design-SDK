using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using Timer = System.Threading.Timer;
using TouchlessDesign.Components;
using TouchlessDesign.Components.Ipc;
using TouchlessDesign;
using TouchlessDesign.Components.Remote;
using TouchlessDesign.Components.Input;
using TouchlessDesign.Components.Input.Providers;
using TouchlessDesign.Components.Ipc.Networking.Tcp;
using TouchlessDesign.Components.Input.Providers.LeapMotion;
using TouchlessDesign.Components.Input.Providers.Remote;

public class TouchlessInput : AppComponent, Client.IListener
{
  public bool IsEmulationEnabled { get; private set; }

  public HoverStates HoverState { get { return _hoverState; } private set { _hoverState = value; OnHoverStateChanged?.Invoke(_hoverState); } }

  private HoverStates _hoverState;

  public bool IsButtonDown { get; private set; }

  public bool IsNoTouch { get; private set; }

  public int HandCount { get; private set; }

  public bool IsOnboardingActive { get; private set; }

  // Any message trying to set hand/click/notouch state with a lower priority than this value, will be ignored.
  public int ClientPriority { get; private set; }

  private Rect? _bounds;

  private Action<HoverStates> OnHoverStateChanged;

  public Rect Bounds
  {
    get
    {
      if (!_bounds.HasValue)
      {
        // _bounds = Screen.PrimaryScreen.Bounds;
      }
      return _bounds.Value;
    }
    set { _bounds = value; }
  }

  // private IKeyboardMouseEvents _hook;
  private DateTime? _timeSinceToggleEmulationKeyCombination;

  private Client _remoteClient;
  private bool _remoteClientActive = false;

  private RemoteClient _udpClient;

  protected override void DoStart()
  {
    // InitializeHooks();
    InitializeClickHandling();
    InitializeInputProvider();

    if (Config.General.RemoteProviderMode)
    {
      _udpClient = new RemoteClient();
      _udpClient.DataDir = DataDir;
      _udpClient.Start();
    }
  }

  protected override void DoStop()
  {
    _remoteClientActive = false;
    _remoteClient?.Connection.Close();

    DeinitializeInputProvider();
    DeInitializeClickHandling();
    // DeInitializeHooks();

    if (Config.General.RemoteProviderMode)
    {
      _udpClient?.Stop();
    }
  }

  #region Remote Input
  public void MakeRemoteConnection(IPEndPoint endpoint)
  {
    try
    {
      TcpConnection connection;
      if (TcpConnection.TryOpen(endpoint, out connection))
      {
        TcpMessageParser parser = new TcpMessageParser();
        _remoteClient = new Client(connection, parser);
        _remoteClient.Bind(this);
        _remoteClientActive = true;
      }
    }
    catch (Exception e)
    {
      Debug.LogError($"Connection error: {e}");
    }
  }
  public void MessageReceived(Client client, Msg msg)
  {
    if (!_remoteClientActive) return;
    switch (msg.Type)
    {
      case Msg.Types.ClickAndHoverQuery:
        Input.HoverState = msg.HoverState;
        Input.SetMouseButtonDown(msg.Bool.Value);
        break;
      case Msg.Types.NoTouchQuery:
        Input.IsNoTouch = msg.Bool.Value;
        break;
    }
  }

  public void ConnectionClosed(Client client)
  {
    Debug.Log("Remote connection to Service closed.");
    _remoteClientActive = false;
    _udpClient.Disconnect();
  }

  public void OnException(Client client, Exception e)
  {
    Debug.LogError("Exception caught with remote client connection: " + e.ToString());
    _remoteClientActive = false;
    _udpClient.Disconnect();
  }

  private void HandleRemoteUpdate(object state)
  {
    if (!IsEmulationEnabled) return;
    lock (_hands)
    {
      if (!_provider.Update(_hands))
      {
        _hands.Clear();
      }

      if (_udpClient.AvailableToSend)
      {
        _udpClient.SendHandData(_hands.Values.ToArray());
      }
    }
    QueryClickAndHoverState();
    QueryNoTouch();
  }

  private void QueryClickAndHoverState()
  {
    if (_remoteClientActive)
    {
      _remoteClient.Send(Msg.Factories.ClickAndHoverQuery());
    }
  }

  private void QueryNoTouch()
  {
    if (_remoteClientActive)
    {
      _remoteClient.Send(Msg.Factories.NoTouchQuery());
    }
  }
  #endregion

  #region Click Handling

  private bool _isClicking;
  private Timer _clickTimer;

  private void InitializeClickHandling()
  {
    OnHoverStateChanged += HandleHoverStateChangedForClick;
    _clickTimer = new Timer(HandleClickCallback, null, Timeout.Infinite, Timeout.Infinite);
  }

  private void HandleHoverStateChangedForClick(HoverStates value)
  {
    if (value != HoverStates.Click && _isClicking)
    {
      if (IsButtonDown)
      {
        SetMouseButtonDown(false);
      }
      StopClickCountdown();
    }
  }

  private void StartClickCountdown()
  {
    _isClicking = true;
    _clickTimer.Change(Config.Input.ClickDuration_ms, Config.Input.ClickDuration_ms);
  }

  private void StopClickCountdown()
  {
    _clickTimer.Change(Timeout.Infinite, Timeout.Infinite);
    _isClicking = false;
  }

  private void HandleClickCallback(object state)
  {
    if (!_isClicking) return;
    StopClickCountdown();
    SetMouseButtonDown(false);
  }

  private void DeInitializeClickHandling()
  {
    _clickTimer.Dispose();
    _clickTimer = null;
  }


  #endregion

  #region Input Provider

  private readonly Dictionary<int, Hand> _hands = new Dictionary<int, Hand>();
  private Timer _inputProviderTimer;
  private IInputProvider _provider;

  private void InitializeInputProvider()
  {
    var providerInterfaceType = typeof(IInputProvider);

    var providerTypes = new[] { typeof(LeapMotionProvider), typeof(RemoteProvider) };
    Type providerType = null;


    if (providerTypes.Length <= 0)
    {
      Debug.LogError($"No {providerInterfaceType.Name} implementation could be found.");
      return;
    }

    if (Config.Input.InputProvider >= 0 && Config.Input.InputProvider < providerTypes.Length)
    {
      providerType = providerTypes[Config.Input.InputProvider];
    }

    if (providerType == null)
    {
      Debug.LogWarning($"No matching input provider could be found with the index {Config.Input.InputProvider}. The default will be used instead.");
      providerType = typeof(LeapMotionProvider);
    }

    object instance = null;
    try
    {
      instance = Activator.CreateInstance(providerType);
      _provider = (IInputProvider)instance;
    }
    catch (Exception e)
    {
      Debug.LogError($"Exception thrown when instantiating or casting provider '{instance?.GetType().Name}': {e}");
      return;
    }

    if (_provider == null)
    {
      Debug.LogError($"Unknown error. Provider could not be instantiated/cast to {providerInterfaceType.Name}");
      return;
    }

    _provider.DataDir = DataDir;

    try
    {
      _provider.Start();
      Debug.Log($"Input provider '{_provider.GetType().Name}' started.");
      if (Config.General.RemoteProviderMode)
      {
        _inputProviderTimer = new Timer(HandleRemoteUpdate, null, 0, Config.Input.UpdateRate_ms);
      }
      else
      {
        _inputProviderTimer = new Timer(HandleProviderUpdate, null, 0, Config.Input.UpdateRate_ms);
      }
    }
    catch (Exception e)
    {
      Debug.LogError($"Exception caught while starting {_provider.GetType().Name}. {e}");
    }
  }

  private bool _hasClicked;

  private void HandleProviderUpdate(object state)
  {
    if (!IsEmulationEnabled) return;
    lock (_hands)
    {
      try
      {
        if (!_provider.Update(_hands))
        {
          _hands.Clear();
        }

        if (_hands.Count <= 0)
        {
          HandCount = 0;
          if (IsButtonDown)
          {
            SetMouseButtonDown(false);
          }

          if (_hasClicked)
          {
            _hasClicked = false;
          }
        }
        else
        {
          var hand = _hands.Values.First();
          HandCount = _hands.Values.Count;

          //position
          Config.Input.NormalizedPosition(hand.X, hand.Y, hand.Z, out var h, out var v);
          var pixelX = (int)Math.Round(h * Bounds.width + Bounds.xMin);
          var pixelY = (int)Math.Round(v * Bounds.height + Bounds.yMin);
          SetPosition(pixelX, pixelY);

          //click
          var isGrabbing = hand.GrabStrength > Config.Input.GrabClickThreshold;
          SetMouseDownConfidence(hand.GrabStrength);
          var hoverState = HoverState;
          if (isGrabbing && !IsButtonDown)
          {
            if (hoverState == HoverStates.Click)
            {
              if (!_hasClicked)
              {
                _hasClicked = true;
                DoClick();
              }
            }
            else
            {
              SetMouseButtonDown(true);
            }
          }
          else if (!isGrabbing && IsButtonDown)
          {
            SetMouseButtonDown(false);
          }

          if (!isGrabbing && _hasClicked)
          {
            _hasClicked = false;
          }
        }
      }
      catch (Exception e)
      {
        Debug.LogError($"Caught exception while updating input provider: {e}");
      }
    }
  }

  public void DeinitializeInputProvider()
  {
    _inputProviderTimer.Dispose();
    _inputProviderTimer = null;
    _provider?.Stop();
    lock (_hands)
    {
      _hands.Clear();
    }
  }

  #endregion

  #region Global Mouse / Keyboard Hooks

  //private void InitializeHooks()
  //{
  //  _hook = Hook.GlobalEvents();
  //  var d = new Dictionary<Combination, Action> {
  //      {Combination.FromString(Config.Input.ToggleEmulationKeyCombination), HandleToggleEmulationKeyCombination}
  //    };
  //  _hook.OnCombination(d);
  //}

  //private void DeInitializeHooks()
  //{
  //  if (_hook != null)
  //  {
  //    _hook?.Dispose();
  //  }
  //  _hook = null;
  //}

  //private void HandleToggleEmulationKeyCombination()
  //{
  //  if (_timeSinceToggleEmulationKeyCombination == null)
  //  {
  //    DoToggleEmulation();
  //  }
  //  else
  //  {
  //    var now = DateTime.Now;
  //    var delta = now - _timeSinceToggleEmulationKeyCombination.Value;
  //    if (delta.TotalMilliseconds > Config.Input.ToggleEmulationToggleSpeed_ms)
  //    {
  //      DoToggleEmulation();
  //    }
  //  }
  //}

  //private void OnGlobalMouseUp(object sender, MouseEventArgs e)
  //{
  //  if (IsButtonDown)
  //  {
  //    IsButtonDown = false;
  //  }
  //}

  //private void OnGlobalMouseDown(object sender, MouseEventArgs e)
  //{
  //  if (!IsButtonDown.Value)
  //  {
  //    IsButtonDown.Value = true;
  //  }
  //}

  //private void DoToggleEmulation()
  //{
  //  _timeSinceToggleEmulationKeyCombination = DateTime.Now;
  //  var v = IsEmulationEnabled.Value;
  //  if (IsButtonDown.Value)
  //  {
  //    SetMouseButtonDown(false);
  //  }
  //  IsEmulationEnabled.Value = v = !v;

  //  Log.Info($"Toggle Emulation invoked - mouse emulation: {v}");
  //}

  #endregion

  #region ICursor

  public void SetPosition(int x, int y)
  {
    if (!IsEmulationEnabled) return;
    var b = Bounds;
    if (x < b.xMin) x = (int)b.xMin;
    if (x > b.xMax) x = (int)b.xMax;
    if (y > b.yMax) y = (int)b.yMax;
    if (y < b.yMin) y = (int)b.yMin;
    PosX = x;
    PosY = y;
    // SetCursorPos(x, y);
  }

  public int PosX { get; private set; }
  public int PosY { get; private set; }

  public void SetMouseButtonDown(bool isDown)
  {
    if (!IsEmulationEnabled) return;
    if (IsButtonDown == isDown) return;
    IsButtonDown = isDown;
    if (_isClicking && !isDown)
    {
      StopClickCountdown();
    }
    //var status = isDown ? MouseEventFlags.LeftDown : MouseEventFlags.LeftUp;
    //mouse_event((uint)status, 0, 0, 0, 0);
  }

  public void DoClick()
  {
    if (!Config.Input.ClickEnabled) return;
    if (_isClicking) return;
    SetMouseButtonDown(true);
    StartClickCountdown();
  }

  public bool GetIsClicking()
  {
    return _isClicking;
  }

  public bool GetClickingEnabled()
  {
    return Config.Input.ClickEnabled;
  }

  public double MouseDownConfidence { get; private set; }

  public void SetMouseDownConfidence(double value)
  {
    MouseDownConfidence = value;
  }

  //public void GetComputedPosition(out int x, out int y)
  //{
  //  GetPosition(out x, out y);
  //}

  #endregion
}

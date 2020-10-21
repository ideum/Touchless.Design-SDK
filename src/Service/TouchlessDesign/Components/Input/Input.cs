using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Gma.System.MouseKeyHook;
using TouchlessDesign.Components.Input.Providers;
using TouchlessDesign.Components.Input.Providers.LeapMotion;
using TouchlessDesign.Components.Input.Providers.Remote;
using TouchlessDesign.Components.Ipc;
using TouchlessDesign.Components.Ipc.Networking;
using TouchlessDesign.Components.Ipc.Networking.Tcp;
using static TouchlessDesign.Components.Ipc.Networking.Client;
using Timer = System.Threading.Timer;

namespace TouchlessDesign.Components.Input
{

  public class Input : AppComponent, Client.IListener
  {

    public Property<bool> IsEmulationEnabled { get; } = new Property<bool>(true);

    public Property<HoverStates> HoverState { get; } = new Property<HoverStates>(HoverStates.None);

    public Property<bool> IsButtonDown { get; } = new Property<bool>(false);

    public Property<bool> IsNoTouch { get; } = new Property<bool>(false);

    public Property<int> HandCount { get; } = new Property<int>(0);

    private Rectangle? _bounds;

    public Rectangle Bounds {
      get {
        if (!_bounds.HasValue) {
          _bounds = Screen.PrimaryScreen.Bounds;
        }
        return _bounds.Value;
      }
      set { _bounds = value; }
    }

    private IKeyboardMouseEvents _hook;
    private DateTime? _timeSinceToggleEmulationKeyCombination;

    private Client _remoteClient;
    private bool _remoteClientActive = false;

    protected override void DoStart() {
      InitializeHooks();
      InitializeClickHandling();
      InitializeInputProvider();
    }

    protected override void DoStop() {
      DeinitializeInputProvider();
      DeInitializeClickHandling();
      DeInitializeHooks();
    }

    #region Remote Input

    public void MakeRemoteConnection(IPEndPoint endpoint) {
      try {
        TcpConnection connection;
        if (TcpConnection.TryOpen(endpoint, out connection)) {
          TcpMessageParser parser = new TcpMessageParser();
          _remoteClient = new Client(connection, parser);
          _remoteClient.Bind(this);
        }
      } catch (Exception e) {
        Log.Error($"Connection error: {e}");
      }
    }
    public void MessageReceived(Client client, Msg msg) {
      switch (msg.Type) {
        case Msg.Types.ClickAndHoverQuery:
          Input.HoverState.Value = msg.HoverState;
          Input.SetMouseButtonDown(msg.Bool.Value);
          break;
        case Msg.Types.NoTouchQuery:
          Input.IsNoTouch.Value = msg.Bool.Value;
          break;
      }
    }

    public void ConnectionClosed(Client client) {
      Log.Debug("Remote connection to Service closed.");
      _remoteClientActive = false;
    }

    public void OnException(Client client, Exception e) {
      Log.Error("Exception caught with remote client connection: " + e.ToString());
      _remoteClientActive = false;
    }

    private void HandleRemoteUpdate(object state) {
      if (!IsEmulationEnabled.Value) return;
      lock (_hands) {
        if (!_provider.Update(_hands)) {
          _hands.Clear();
        }

        if (RemoteClient.AvailableToSend) {
          RemoteClient.SendHandData(_hands.Values.ToArray());
        }
      }
      QueryClickAndHoverState();
      QueryNoTouch();
    }

    private void QueryClickAndHoverState() {
      if (_remoteClientActive) {
        _remoteClient.Send(Msg.Factories.ClickAndHoverQuery());
      }
    }

    private void QueryNoTouch() {
      if (_remoteClientActive) {
        _remoteClient.Send(Msg.Factories.NoTouchQuery());
      }
    }
    #endregion


    #region Click Handling

    private bool _isClicking;
    private Timer _clickTimer;

    private void InitializeClickHandling() {
      HoverState.AddChangedListener(HandleHoverStateChangedForClick);
      _clickTimer = new Timer(HandleClickCallback, null, Timeout.Infinite, Timeout.Infinite);
    }

    private void HandleHoverStateChangedForClick(Property<HoverStates> property, HoverStates oldValue, HoverStates value) {
      if (value != HoverStates.Click && _isClicking) {
        if (IsButtonDown.Value) {
          SetMouseButtonDown(false);
        }
        StopClickCountdown();
      }
    }

    private void StartClickCountdown() {
      _isClicking = true;
      _clickTimer.Change(0, Config.Input.ClickDuration_ms);
    }

    private void StopClickCountdown() {
      _clickTimer.Change(Timeout.Infinite, Timeout.Infinite);
      _isClicking = false;
    }

    private void HandleClickCallback(object state) {
      if (!_isClicking) return;
      StopClickCountdown();
      SetMouseButtonDown(false);
    }

    private void DeInitializeClickHandling() {
      _clickTimer.Dispose();
      _clickTimer = null;
    }


    #endregion

    #region Input Provider

    private readonly Dictionary<int, Hand> _hands = new Dictionary<int, Hand>();
    private Timer _inputProviderTimer;
    private IInputProvider _provider;

    private void InitializeInputProvider() {
      var providerInterfaceType = typeof(IInputProvider);

      var providerTypes = new[] { typeof(LeapMotionProvider), typeof(RemoteProvider) };
      Type providerType = null;


      if (providerTypes.Length <= 0) {
        Log.Error($"No {providerInterfaceType.Name} implementation could be found.");
        return;
      }

      if (Config.Input.InputProvider >= 0 && Config.Input.InputProvider < providerTypes.Length) {
        providerType = providerTypes[Config.Input.InputProvider];
      }

      if (providerType == null) {
        Log.Warn($"No matching input provider could be found with the index {Config.Input.InputProvider}. The default will be used instead.");
        providerType = typeof(LeapMotionProvider);
      }

      object instance = null;
      try {
        instance = Activator.CreateInstance(providerType);
        _provider = (IInputProvider)instance;
      } catch (Exception e) {
        Log.Error($"Exception thrown when instantiating or casting provider '{instance?.GetType().Name}': {e}");
        return;
      }

      if (_provider == null) {
        Log.Error($"Unknown error. Provider could not be instantiated/cast to {providerInterfaceType.Name}");
        return;
      }

      _provider.DataDir = DataDir;

      try {
        _provider.Start();
        Log.Info($"Input provider '{_provider.GetType().Name}' started.");
        if (Config.General.RemoteProviderMode) {
          _inputProviderTimer = new Timer(HandleRemoteUpdate, null, 0, Config.Input.UpdateRate_ms);
        } else {
          _inputProviderTimer = new Timer(HandleProviderUpdate, null, 0, Config.Input.UpdateRate_ms);
        }
      } catch (Exception e) {
        Log.Error($"Exception caught while starting {_provider.GetType().Name}. {e}");
      }
    }

    private bool _hasClicked;

    private void HandleProviderUpdate(object state) {
      if (!IsEmulationEnabled.Value) return;
      lock (_hands) {
        try {
          if (!_provider.Update(_hands)) {
            _hands.Clear();
          }

          if (_hands.Count <= 0) {
            HandCount.Value = 0;
            if (IsButtonDown.Value) {
              SetMouseButtonDown(false);
            }

            if (_hasClicked) {
              _hasClicked = false;
            }
          } else {
            var hand = _hands.Values.First();
            HandCount.Value = _hands.Values.Count;

            //position
            Config.Input.NormalizedPosition(hand.X, hand.Y, hand.Z, out var h, out var v);
            var pixelX = (int)Math.Round(h * Bounds.Width + Bounds.Left);
            var pixelY = (int)Math.Round(v * Bounds.Height + Bounds.Top);
            SetPosition(pixelX, pixelY);

            //click
            var isGrabbing = hand.GrabStrength > Config.Input.GrabClickThreshold;
            SetMouseDownConfidence(hand.GrabStrength);
            var hoverState = HoverState.Value;
            if (isGrabbing && !IsButtonDown.Value) {
              if (hoverState == HoverStates.Click) {
                if (!_hasClicked) {
                  _hasClicked = true;
                  DoClick();
                }
              } else {
                SetMouseButtonDown(true);
              }
            } else if (!isGrabbing && IsButtonDown.Value) {
              SetMouseButtonDown(false);
            }

            if (!isGrabbing && _hasClicked) {
              _hasClicked = false;
            }
          }
        } catch (Exception e) {
          Log.Error($"Caught exception while updating input provider: {e}");
        }


      }
    }

    public void DeinitializeInputProvider() {
      _inputProviderTimer.Dispose();
      _inputProviderTimer = null;
      _provider?.Stop();
      lock (_hands) {
        _hands.Clear();
      }
    }

    #endregion

    #region Global Mouse / Keyboard Hooks

    private void InitializeHooks() {
      _hook = Hook.GlobalEvents();
      var d = new Dictionary<Combination, Action> {
        {Combination.FromString(Config.Input.ToggleEmulationKeyCombination), HandleToggleEmulationKeyCombination}
      };
      _hook.OnCombination(d);
    }

    private void DeInitializeHooks() {
      if (_hook != null) {
        _hook?.Dispose();
      }
      _hook = null;
    }

    private void HandleToggleEmulationKeyCombination() {
      if (_timeSinceToggleEmulationKeyCombination == null) {
        DoToggleEmulation();
      } else {
        var now = DateTime.Now;
        var delta = now - _timeSinceToggleEmulationKeyCombination.Value;
        if (delta.TotalMilliseconds > Config.Input.ToggleEmulationToggleSpeed_ms) {
          DoToggleEmulation();
        }
      }
    }

    private void OnGlobalMouseUp(object sender, MouseEventArgs e) {
      if (IsButtonDown.Value) {
        IsButtonDown.Value = false;
      }
    }

    private void OnGlobalMouseDown(object sender, MouseEventArgs e) {
      if (!IsButtonDown.Value) {
        IsButtonDown.Value = true;
      }
    }

    private void DoToggleEmulation() {
      _timeSinceToggleEmulationKeyCombination = DateTime.Now;
      var v = IsEmulationEnabled.Value;
      if (IsButtonDown.Value) {
        SetMouseButtonDown(false);
      }
      IsEmulationEnabled.Value = v = !v;

      Log.Info($"Toggle Emulation invoked - mouse emulation: {v}");
    }

    #endregion

    #region ICursor

    public void SetPosition(int x, int y) {
      if (!IsEmulationEnabled.Value) return;
      var b = Bounds;
      if (x < b.Left) x = b.Left;
      if (x > b.Right) x = b.Right;
      if (y > b.Bottom) y = b.Bottom;
      if (y < b.Top) y = b.Top;
      PosX = x;
      PosY = y;
      SetCursorPos(x, y);
    }

    public int PosX { get; private set; }
    public int PosY { get; private set; }

    public void SetMouseButtonDown(bool isDown) {
      if (!IsEmulationEnabled.Value) return;
      if (IsButtonDown.Value == isDown) return;
      IsButtonDown.Value = isDown;
      if (_isClicking && !isDown) {
        StopClickCountdown();
      }
      var status = isDown ? MouseEventFlags.LeftDown : MouseEventFlags.LeftUp;
      mouse_event((uint)status, 0, 0, 0, 0);
    }

    public void DoClick() {
      if (!Config.Input.ClickEnabled) return;
      if (_isClicking) return;
      SetMouseButtonDown(true);
      StartClickCountdown();
    }

    public bool GetIsClicking() {
      return _isClicking;
    }

    public bool GetClickingEnabled() {
      return Config.Input.ClickEnabled;
    }

    public double MouseDownConfidence { get; private set; }

    public void SetMouseDownConfidence(double value) {
      MouseDownConfidence = value;
    }

    public void GetComputedPosition(out int x, out int y) {
      GetPosition(out x, out y);
    }

    #endregion

    #region Native Methods

    #region mouse_event
    [Flags]
    public enum MouseEventFlags : uint
    {
      LeftDown = 0x00000002,
      LeftUp = 0x00000004,
      MiddleDown = 0x00000020,
      MiddleUp = 0x00000040,
      Move = 0x00000001,
      Absolute = 0x00008000,
      RightDown = 0x00000008,
      RightUp = 0x00000010,
      Wheel = 0x00000800,
      XDown = 0x00000080,
      XUp = 0x00000100
    }

    //Use the values of this enum for the 'dwData' parameter
    //to specify an X button when using MouseEventFlags.XDown or
    //MouseEventFlags.XUp for the dwFlags parameter.
    public enum MouseEventDataXButtons : uint
    {
      XButton1 = 0x00000001,
      XButton2 = 0x00000002
    }

    [DllImport("user32.dll")]
    private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, int dwExtraInfo);

    private static void MouseEventMoveAbsolute(uint mickeysX, uint mickeysY) {
      mouse_event((uint)(MouseEventFlags.Move | MouseEventFlags.Absolute), mickeysX, mickeysY, 0, 0);
    }

    #endregion

    #region GetCursorInfo

    private struct CursorInfo
    {
      public bool IsShown;
      public int X, Y;
    }

    private const int CURSOR_SHOWING = 0x00000001;
    private const int CURSOR_SUPPRESSED = 0x00000002;

    [StructLayout(LayoutKind.Sequential)]
    private struct POINT
    {
      public int x;
      public int y;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct CURSORINFO
    {
      public int cbSize;        // Specifies the size, in bytes, of the structure.
                                // The caller must set this to Marshal.SizeOf(typeof(CURSORINFO)).
      public int flags;         // Specifies the cursor state. This parameter can be one of the following values:
                                //    0                  The cursor is hidden.
                                //    CURSOR_SHOWING     The cursor is showing.
                                //    CURSOR_SUPPRESSED  (Windows 8 and above.) The cursor is suppressed. This flag indicates that the system is not drawing the cursor because the user is providing input through touch or pen instead of the mouse.
      public IntPtr hCursor;          // Handle to the cursor.
      public POINT ptScreenPos;       // A POINT structure that receives the screen coordinates of the cursor.
    }

    /// <summary>Must initialize cbSize</summary>
    [DllImport("user32.dll")]
    private static extern bool GetCursorInfo(ref CURSORINFO pci);

    private static bool GetCursor(out CURSORINFO cursor) {
      cursor = new CURSORINFO { cbSize = Marshal.SizeOf(typeof(CURSORINFO)) };
      return GetCursorInfo(ref cursor);
    }

    private static CursorInfo GetCursorInfo() {
      if (GetCursor(out var nativeCursorInfo)) {
        return new CursorInfo {
          X = nativeCursorInfo.ptScreenPos.x,
          Y = nativeCursorInfo.ptScreenPos.y,
          IsShown = nativeCursorInfo.flags == CURSOR_SHOWING
        };
      } else {
        return new CursorInfo();
      }
    }

    private static void GetPosition(out int x, out int y) {
      if (GetCursor(out var cursor)) {
        x = cursor.ptScreenPos.x;
        y = cursor.ptScreenPos.y;
      } else {
        x = y = 0;
      }
    }

    private static bool IsCursorShowing {
      get {
        if (GetCursor(out var cursor)) {
          return cursor.flags == CURSOR_SHOWING;
        } else {
          return false;
        }
      }
    }

    #endregion

    #region SetCursorPos
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetCursorPos(int x, int y);
    #endregion

    #endregion
  }
}
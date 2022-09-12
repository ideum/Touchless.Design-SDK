﻿using System;
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
using TouchlessDesign.Components.Remote;
using Timer = System.Threading.Timer;

namespace TouchlessDesign.Components.Input {

  public class Input : AppComponent, Client.IListener {
    public Property<bool> IsEmulationEnabled { get; } = new Property<bool>(true);

    public Property<bool> IsNoTouch { get; } = new Property<bool>(false);

    public Property<bool> IsOnboardingActive { get; } = new Property<bool>(false);

    public TouchlessUser stateUser;
    public Dictionary<int, TouchlessUser> RegisteredUsers;
    public event Action<HoverStates, HoverStates> StateUserHoverChanged;
    public event Action<bool, bool> StateUserButtonChanged;

    // Any message trying to set hand/click/notouch state with a lower priority than this value, will be ignored.
    public Property<int> ClientPriority { get; } = new Property<int>(0);

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

    private RemoteClient _udpClient;
    private RemoteClient _userDataClient;

    protected override void DoStart() {
      RegisteredUsers = new Dictionary<int, TouchlessUser>();
      InitializeHooks();
      //InitializeClickHandling();
      InitializeInputProvider();
      if (Config.General.RemoteProviderMode) {
        _udpClient = new RemoteClient();
        _udpClient.DataDir = DataDir;
        _udpClient.Start();
      }
    }

    protected override void DoStop() {
      _remoteClientActive = false;
      _remoteClient?.Connection.Close();

      DeinitializeInputProvider();
      DeInitializeClickHandling();
      DeInitializeHooks();

      if (Config.General.RemoteProviderMode) {
        _udpClient?.Stop();
      }
    }

    public int GetStateUserId() {
      return stateUser != null ? stateUser.RemoteUserInfo.DeviceId : -1;
    }

    public void RegisterUser(TouchlessUser user) {
      if (user == null) {
        Log.Error($"Tried to register null user");
      }
      else if (RegisteredUsers.ContainsKey(user.RemoteUserInfo.DeviceId)) {
        Log.Error($"Tried to register already-existing user {user.RemoteUserInfo.DeviceId}");
      }
      else {
        RegisteredUsers.Add(user.RemoteUserInfo.DeviceId, user);
        Log.Info($"Successfully registered user {user.RemoteUserInfo.DeviceId}");

        // If we don't have a mouse user assigned, make this user our mouse user until the user disconnects.
        if (stateUser == null) {
          stateUser = user;
          InitializeClickHandling();
        }
      }
    }

    public void DeregisterUser(TouchlessUser user) {
      if (user == null) {
        Log.Error($"Tried to deregister a null user");
      }
      else if (!RegisteredUsers.ContainsKey(user.RemoteUserInfo.DeviceId)) {
        Log.Error($"Tried to deregister non-existent user {user.RemoteUserInfo.DeviceId}");
      }
      else {
        RegisteredUsers.Remove(user.RemoteUserInfo.DeviceId);
        Log.Info($"Successfully deregistered user {user.RemoteUserInfo.DeviceId}");

        // If the user that disconnected is our mouse user, deinitialize click handling and attempt to find a new user to assign.
        if (stateUser == user) {
          DeInitializeClickHandling();
          stateUser = null;
          Log.Warn($"Assigned mouse user has disconnected.");
          if (RegisteredUsers.Count > 0) {
            stateUser = RegisteredUsers.First().Value;
            InitializeClickHandling();
            Log.Warn($"Assigning user {stateUser.RemoteUserInfo.DeviceId} as new mouse user.");
          }
        }
      }
    }

    #region Remote Input
    public bool MakeRemoteConnection(IPEndPoint endpoint) {
      try {
        TcpConnection connection;
        if (TcpConnection.TryOpen(endpoint, out connection)) {
          TcpMessageParser parser = new TcpMessageParser();
          _remoteClient = new Client(connection, parser);
          _remoteClient.Bind(this);
          _remoteClientActive = true;
          Log.Info("Connected. Attempting to register as a remote client");
          _remoteClient.Send(new Msg(Msg.Types.RegisterRemoteClient, Config.General.DeviceID));
          return true;
        }
      }
      catch (Exception e) {
        Log.Error($"Connection error: {e}");
      }
      return false;
    }

    public void MessageReceived(Client client, Msg msg) {
      if (!_remoteClientActive) return;
      switch (msg.Type) {
        case Msg.Types.ClickAndHoverQuery:
          if (stateUser != null && msg.DeviceId == stateUser.RemoteUserInfo.DeviceId) {
            stateUser.HoverState.Value = msg.HoverState;
            stateUser.SetMouseButtonDown(msg.Bool.Value);
          }
          break;
        case Msg.Types.NoTouchQuery:
          Input.IsNoTouch.Value = msg.Bool.Value;
          break;
      }
    }

    public void ConnectionClosed(Client client) {
      Log.Debug("Remote connection to Service closed.");
      _remoteClientActive = false;
      _udpClient.Disconnect();
    }

    public void OnException(Client client, Exception e) {
      Log.Error("Exception caught with remote client connection: " + e.ToString());
      _remoteClientActive = false;
      _remoteClient.Connection?.Close();
      _remoteClient.Connection?.Dispose();
      _udpClient.Disconnect();
    }

    private void HandleRemoteUpdate(object state) {
      if (!IsEmulationEnabled.Value) return;
      lock (RegisteredUsers) {
        if (!_provider.Update(RegisteredUsers)) {
          foreach (var user in RegisteredUsers) {
            user.Value.Hands.Clear();
          }
        }

        if (_udpClient.AvailableToSend) {
          foreach (var user in RegisteredUsers) {
            _udpClient.SendHandData(user.Key, user.Value.Hands.ToArray());
          }
        }
      }
      QueryClickAndHoverState();
      QueryNoTouch();
    }

    private void QueryClickAndHoverState() {
      if (_remoteClientActive) {
        _remoteClient.Send(Msg.Factories.ClickAndHoverQuery(Config.General.DeviceID));
      }
    }

    private void QueryNoTouch() {
      if (_remoteClientActive) {
        _remoteClient.Send(Msg.Factories.NoTouchQuery(Config.General.DeviceID));
      }
    }
    #endregion

    #region Click Handling

    //private bool _isClicking;
    //private Timer _clickTimer;

    private void InitializeClickHandling() {
      stateUser.HoverState.AddChangedListener(HandleHoverStateChangedForClick);
      stateUser.IsButtonDown.AddChangedListener(HandleButtonPropertyChanged);
      stateUser.MouseButtonStateSet += HandleStateUserButtonDown;
      //stateUser.ClickCallback += HandleStateUserButtonDown;
      //_clickTimer = new Timer(HandleClickCallback, null, Timeout.Infinite, Timeout.Infinite);
    }

    private void DeInitializeClickHandling() {
      if (stateUser != null) {
        stateUser.HoverState.RemoveChangedListener(HandleHoverStateChangedForClick);
        stateUser.IsButtonDown.RemoveChangedListener(HandleButtonPropertyChanged);
        stateUser.MouseButtonStateSet -= HandleStateUserButtonDown;
        //stateUser.ClickCallback -= HandleStateUserButtonDown;
      }
      //_clickTimer.Dispose();
      //_clickTimer = null;
    }

    private void HandleButtonPropertyChanged(Property<bool> property, bool oldValue, bool value) {
      StateUserButtonChanged?.Invoke(oldValue, value);
    }

    private void HandleHoverStateChangedForClick(Property<HoverStates> property, HoverStates oldValue, HoverStates value) {
      StateUserHoverChanged?.Invoke(oldValue, value);
      if (value != HoverStates.Click && stateUser.IsClicking) {
        if (stateUser.IsButtonDown.Value) {
          stateUser.SetMouseButtonDown(false);
        }
        stateUser.StopClickCountdown();
      }
    }

    //private void StartClickCountdown() {
    //  _isClicking = true;
    //  _clickTimer.Change(Config.Input.ClickDuration_ms, Config.Input.ClickDuration_ms);
    //}

    //private void StopClickCountdown() {
    //  _clickTimer.Change(Timeout.Infinite, Timeout.Infinite);
    //  _isClicking = false;
    //}

    //private void HandleClickCallback(object state) {
    //  if (!_isClicking) return;
    //  HandleStateUserButtonDown(false);
    //}



    #endregion

    #region Input Provider

    //private readonly Dictionary<int, List<Hand>> _hands = new Dictionary<int, List<Hand>>();
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

      // Add our local user if needed
      if (providerType == typeof(LeapMotionProvider)) {
        RegisterUser(new TouchlessUser(Config.General.DeviceID, "127.0.0.1"));
      }

      object instance = null;
      try {
        instance = Activator.CreateInstance(providerType);
        _provider = (IInputProvider)instance;
      }
      catch (Exception e) {
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
        }
        else {
          _inputProviderTimer = new Timer(HandleProviderUpdate, null, 0, Config.Input.UpdateRate_ms);
        }
      }
      catch (Exception e) {
        Log.Error($"Exception caught while starting {_provider.GetType().Name}. {e}");
      }
    }

    //private bool _hasClicked;

    private void HandleProviderUpdate(object state) {
      if (!IsEmulationEnabled.Value) return;
      lock (RegisteredUsers) {
        try {

          if (!_provider.Update(RegisteredUsers)) {
            foreach (var userKey in RegisteredUsers) {
              userKey.Value.Hands.Clear();
            }
          }

          foreach (var userKey in RegisteredUsers) {
            TouchlessUser user = userKey.Value;
            user.InitialPress = false;
            user.InitialRelease = false;
            if (user.Hands.Count <= 0) {
              user.HandCount = 0;
              if (user.IsButtonDown.Value) {
                user.SetMouseButtonDown(false);
              }

              if (user.HasClicked) {
                user.HasClicked = false;
              }

              user.ScreenX = user.ScreenY = 0;

            }
            else {
              var hand = user.Hands.First();
              user.HandCount = user.Hands.Count;

              //position
              Config.Input.NormalizedPosition(hand.X, hand.Y, hand.Z, out var h, out var v);
              var pixelX = (int)Math.Round(h * Bounds.Width + Bounds.Left);
              var pixelY = (int)Math.Round(v * Bounds.Height + Bounds.Top);
              user.ScreenX = pixelX; 
              user.ScreenY = pixelY;
              if (user == stateUser && Config.Input.MouseEmulationEnabled) {
                SetPosition(pixelX, pixelY);
              }

              //click
              var isGrabbing = hand.GrabStrength > Config.Input.GrabClickThreshold;
              user.SetMouseDownConfidence(hand.GrabStrength);
              // SetMouseDownConfidence(hand.GrabStrength);
              var hoverState = user.HoverState.Value;
              if (isGrabbing && !user.IsButtonDown.Value) {
                if (hoverState == HoverStates.Click) {
                  if (!user.HasClicked) {
                    user.HasClicked = true;
                    user.DoClick();
                  }
                }
                else {
                  user.SetMouseButtonDown(true);
                }
              }
              else if (!isGrabbing && user.IsButtonDown.Value) {
                user.SetMouseButtonDown(false);
              }

              if (!isGrabbing && user.HasClicked) {
                //_hasClicked = false;
                user.HasClicked = false;
              }

              if (user.Client != null) {
                //Ipc.SendUserUpdate(user);
                //user.Client.Send(Msg.Factories.UserUpdate(user.RemoteUserInfo.DeviceId, user.HandCount, pixelX, pixelY, user.IsButtonDown.Value));
              }
            }
          }
        }
        catch (Exception e) {
          Log.Error($"Caught exception while updating input provider: {e}");
        }
      }
    }

    public void DeinitializeInputProvider() {
      _inputProviderTimer.Dispose();
      _inputProviderTimer = null;
      _provider?.Stop();
      lock (RegisteredUsers) {
        if (stateUser != null) {
          stateUser.Hands.Clear();
        }
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
      }
      else {
        var now = DateTime.Now;
        var delta = now - _timeSinceToggleEmulationKeyCombination.Value;
        if (delta.TotalMilliseconds > Config.Input.ToggleEmulationToggleSpeed_ms) {
          DoToggleEmulation();
        }
      }
    }

    private void OnGlobalMouseUp(object sender, MouseEventArgs e) {
      if (stateUser == null) return;

      if (stateUser.IsButtonDown.Value) {
        stateUser.IsButtonDown.Value = false;
      }
    }

    private void OnGlobalMouseDown(object sender, MouseEventArgs e) {
      if (stateUser == null) return;

      if (!stateUser.IsButtonDown.Value) {
        stateUser.IsButtonDown.Value = true;
      }
    }

    private void DoToggleEmulation() {
      _timeSinceToggleEmulationKeyCombination = DateTime.Now;
      var v = IsEmulationEnabled.Value;
      if (stateUser != null && stateUser.IsButtonDown.Value) {
        //HandleStateUserButtonDown(false);
        stateUser.SetMouseButtonDown(false);
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

    public void HandleStateUserButtonDown(bool isDown) {
      if (!IsEmulationEnabled.Value) return;
      if (!Config.Input.ClickEnabled) return;
      //if (stateUser.IsButtonDown.Value == isDown) return;
      //stateUser.IsButtonDown.Value = isDown;
      //if (_isClicking && !isDown) {
      //  StopClickCountdown();
      //}
      var status = isDown ? MouseEventFlags.LeftDown : MouseEventFlags.LeftUp;
      mouse_event((uint)status, 0, 0, 0, 0);
    }

    //public void DoClick() {
    //  if (!Config.Input.ClickEnabled) return;
    //  if (_isClicking) return;
    //  HandleStateUserButtonDown(true);
    //  StartClickCountdown();
    //}

    public bool GetIsClicking() {
      return stateUser != null && stateUser.IsClicking;
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
    public enum MouseEventFlags : uint {
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
    public enum MouseEventDataXButtons : uint {
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

    private struct CursorInfo {
      public bool IsShown;
      public int X, Y;
    }

    private const int CURSOR_SHOWING = 0x00000001;
    private const int CURSOR_SUPPRESSED = 0x00000002;

    [StructLayout(LayoutKind.Sequential)]
    private struct POINT {
      public int x;
      public int y;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct CURSORINFO {
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
      }
      else {
        return new CursorInfo();
      }
    }

    private static void GetPosition(out int x, out int y) {
      if (GetCursor(out var cursor)) {
        x = cursor.ptScreenPos.x;
        y = cursor.ptScreenPos.y;
      }
      else {
        x = y = 0;
      }
    }

    private static bool IsCursorShowing {
      get {
        if (GetCursor(out var cursor)) {
          return cursor.flags == CURSOR_SHOWING;
        }
        else {
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
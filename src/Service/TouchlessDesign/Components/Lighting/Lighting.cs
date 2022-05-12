using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using TouchlessDesign.Components.Lighting.Animation;

namespace TouchlessDesign.Components.Lighting {
  public class Lighting : AppComponent {

    private const string FadeCandyDir = "bin/Service/Components/Lighting/FadeCandyServer/";

    public Property<Color> CurrentColor { get; } = new Property<Color>();

    private bool _didStartEnabled;

    protected override void DoStart() {
      if (!Config.Display.LightingEnabled) return;
      _didStartEnabled = true;
      InitializeServer();
      InitializeNetworking();
      InitializeRendering();
      InitializeStateHandling();
    }

    protected override void DoStop() {
      if (_didStartEnabled) {
        DeInitializeStateHandling();
        DeInitializeRendering();
        DeInitializeNetworking();
        DeInitializeServer();
        _didStartEnabled = false;
      }
    }

    #region State Handling

    private static readonly Color NormalColor = Color.White;
    private static readonly Color HoverColor = new Color(255, 255, 1);
    private static readonly Color ClickColor = new Color(41, 234, 41);
    private static readonly Color NoTouchColor = new Color(255, 2, 1);
    private static readonly ColorLerpAnimation ColorLerpAnimation = new ColorLerpAnimation(500, 16);

    private enum States {
      None,
      Normal,
      Hover,
      Click,
      NoTouch
    }

    private States _state;

    private void InitializeStateHandling() {
      //Input.HoverState.AddChangedListener(HandleHoverStateChanged, false);
      Input.StateUserHoverChanged += HandleHoverStateChanged;
      Input.IsNoTouch.AddChangedListener(HandleIsNoTouchChanged, false);
      //Input.IsButtonDown.AddChangedListener(HandleInputButtonDownChanged, true);
      Input.StateUserButtonChanged += HandleInputButtonDownChanged;
    }

    private void DeInitializeStateHandling() {
      //Input.HoverState.RemoveChangedListener(HandleHoverStateChanged);
      Input.IsNoTouch.RemoveChangedListener(HandleIsNoTouchChanged);
      //Input.IsButtonDown.RemoveChangedListener(HandleInputButtonDownChanged);
      Input.StateUserButtonChanged -= HandleInputButtonDownChanged;
    }

    private void HandleHoverStateChanged(HoverStates oldState, HoverStates newState) {
      var state = MapHoverStateToState(newState, Input.stateUser.IsButtonDown.Value, Input.IsNoTouch.Value);
      SetState(state);
    }

    

    //private void HandleHoverStateChanged(Property<HoverStates> property, HoverStates oldValue, HoverStates value) {
    //  var state = MapHoverStateToState(value, Input.IsButtonDown.Value, Input.IsNoTouch.Value);
    //  SetState(state);
    //}


    private void HandleIsNoTouchChanged(Property<bool> property, bool oldValue, bool value) {
      if(Input.stateUser == null) {
        Log.Error("No touch invoked, but there's no user");
        return;
      }
      var state = MapHoverStateToState(Input.stateUser.HoverState.Value, Input.stateUser.IsButtonDown.Value, value);
      SetState(state);
    }

    //private void HandleInputButtonDownChanged(Property<bool> property, bool oldValue, bool value) {
    //  var state = MapHoverStateToState(Input.HoverState.Value, value, Input.IsNoTouch.Value);
    //  SetState(state);
    //}

    private void HandleInputButtonDownChanged(bool oldValue, bool newValue) {
      var state = MapHoverStateToState(Input.stateUser.HoverState.Value, newValue, Input.IsNoTouch.Value);
    }

    private void SetState(States state) {
      if (_state == state) return;
      _state = state;
      var color = Color.Black;
      switch (state) {
        case States.None:
          break;
        case States.Normal:
          color = NormalColor;
          break;
        case States.Hover:
          color = HoverColor;
          break;
        case States.Click:
          color = ClickColor;
          break;
        case States.NoTouch:
          color = NoTouchColor;
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
      ColorLerpAnimation.SetColors(CurrentColor.Value, color);
      PlayAnimation(ColorLerpAnimation);
    }

    private States MapHoverStateToState(HoverStates hover, bool isDown, bool isNoTouch) {
      if (isNoTouch) return States.NoTouch;
      switch (hover) {
        case HoverStates.None:
          return isDown ? States.Click : States.Normal;
        case HoverStates.Click:
          return isDown ? States.Click : States.Hover;
        case HoverStates.Drag:
        case HoverStates.DragHorizontal:
        case HoverStates.DragVertical:
          return isDown ? States.Click : States.Hover;
        default:
          throw new ArgumentOutOfRangeException(nameof(hover), hover, null);
      }
    }

    #endregion

    #region Rendering

    private Thread _renderThread;
    private bool _shouldRenderThreadBeAlive;
    private AutoResetEvent _renderThreadChangeEvent;
    private ManualResetEvent _renderThreadShutdownEvent;
    private readonly object _animationLock = new object();
    private IAnimation _animation;
    private int _animationTime;
    private Color[] _animationBuffer;

    private void InitializeRendering() {
      _animationBuffer = new Color[LightCount];
      lock (_animationLock) {
        _animationTime = 0;
      }
      _shouldRenderThreadBeAlive = true;
      _renderThreadChangeEvent = new AutoResetEvent(false);
      _renderThreadShutdownEvent = new ManualResetEvent(false);
      _renderThread = new Thread(RenderLoop);
      _renderThread.Start();
    }

    private void DeInitializeRendering() {
      lock (_animationLock) {
        _animation = null;
        _animationTime = 0;
        SolidColorAnimation.Black.GetPixels(0, _animationBuffer);
        SetPixels(_animationBuffer, Channel);
      }
      _shouldRenderThreadBeAlive = false;
      _renderThreadShutdownEvent.Set();
      if (!_renderThread.Join(500)) {
        _renderThread.Abort();
      }
      _renderThreadChangeEvent.Dispose();
      _renderThreadShutdownEvent.Dispose();
    }

    private void RenderLoop() {
      WaitHandle[] handles = { _renderThreadChangeEvent, _renderThreadShutdownEvent };
      try {
        while (_shouldRenderThreadBeAlive) {
          int timeToWait_ms;
          if (_animation != null) {
            lock (_animation) {
              timeToWait_ms = _animation.UpdateInterval_ms;
              if (_animationTime < _animation.Duration_ms || _animation.Duration_ms<=0) {
                _animationTime += _animation.UpdateInterval_ms;
              }
              else {
                _animationTime = _animation.Duration_ms;
              }
              _animation.GetPixels(_animationTime, _animationBuffer);
              SetPixels(_animationBuffer, Channel);
            }
          }
          else {
            timeToWait_ms = 1000;
          }
          var t = WaitHandle.WaitAny(handles, timeToWait_ms);
          if (t == WaitHandle.WaitTimeout) {
            continue;
          }
          else if (t == 1) {
            break;
          }
          else if (t == 0) {
            continue;
          }
          else {
            Log.Error($"Invalid value from WaitHandle.WaitAny: {t}");
          }
        }
      }
      catch (Exception e) {
        Log.Error($"Exception caught in LED main loop: {e}");
      }
    }

    public void PlayAnimation(IAnimation anim) {
      lock (_animationLock) {
        _animation = anim;
        _animationTime = 0;
      }
      _renderThreadChangeEvent?.Set();
    }

    #endregion

    #region Server Process Management

    #region Settings

    private const string FadeCandyTemplate = "config_template.json_template";
    private const string FadeCandyConfigFilename = "device_mapping.json";

    private void UpdateFadeCandySettings() {
      var accessedFile = false;
      try {
        var templatePath = Path.Combine(DataDir, FadeCandyDir, FadeCandyTemplate);
        var s = File.ReadAllText(templatePath);
        accessedFile = true;
        s = s.Replace("$(data_port)", Config.Network.FadeCandyData.Port.ToString());
        s = s.Replace("$(relay_port)", Config.Network.FadeCandyRelay.Port.ToString());
        s = s.Replace("$(channel)", Channel.ToString());
        s = s.Replace("$(led_count)", LightCount.ToString());
        var configPath = Path.Combine(DataDir, FadeCandyDir, FadeCandyConfigFilename);
        File.WriteAllText(configPath, s);
      }
      catch (Exception e) {
        if (accessedFile) {
          Log.Error($"Error writing config json {e}");
        }
        else {
          Log.Error($"Error reading config template json {e}");
        }
      }
    }

    [DllImport("user32.dll")]
    static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    private const string ServerFilename = "fcserver.exe";

    private string _serverPath, _serverConfigPath;
    private Process _serverProcess;

    private void InitializeServer() {
      _serverPath = Path.Combine(DataDir, FadeCandyDir, ServerFilename);
      _serverConfigPath = Path.Combine(DataDir, FadeCandyDir, FadeCandyConfigFilename);
      UpdateFadeCandySettings();
      if (!File.Exists(_serverPath)) {
        Log.Error($"LED Server exe does not exist at: {_serverPath}");
        return;
      }
      _serverProcess = new Process { StartInfo = new ProcessStartInfo(_serverPath, _serverConfigPath) };
      _serverProcess.Start();
      ThreadPool.QueueUserWorkItem(HideServerProcessWindow);
    }

    private void HideServerProcessWindow(object state) {
      while (_serverProcess != null && !_serverProcess.HasExited && _serverProcess.MainWindowHandle == IntPtr.Zero) {
        Thread.Sleep(10);
      }

      if (_serverProcess != null && !_serverProcess.HasExited && _serverProcess.MainWindowHandle != IntPtr.Zero) {
        var t = _serverProcess.MainWindowHandle;
        ShowWindow(t, 0);
      }
    }

    private void DeInitializeServer() {
      if (_serverProcess!=null && !_serverProcess.HasExited) {
        _serverProcess?.Kill();
      }
      _serverProcess = null;
    }

    #endregion

    #endregion

    #region Networking

    public enum NetworkStates {
      Connecting,
      Connected,
      ShuttingDown
    }

    public NetworkStates NetworkState { get; private set; } = NetworkStates.Connecting;

    private Socket _socket;

    private void InitializeNetworking() {
      TryConnect();
    }

    private void DeInitializeNetworking() {
      NetworkState = NetworkStates.ShuttingDown;
      CloseSocket();
    }

    private void CloseSocket() {
      _socket?.Close();
      _socket?.Dispose();
      _socket = null;
    }

    private void TryConnect() {
      if (NetworkState == NetworkStates.ShuttingDown) return;
      ThreadPool.QueueUserWorkItem(TryConnectLoop);
    }

    private void TryConnectLoop(object state) {
      NetworkState = NetworkStates.Connecting;
      Thread.Sleep(Config.Network.FadeCandyInitialConnectInterval_ms);
      while (NetworkState == NetworkStates.Connecting && _didStartEnabled) {
        try {
          var endPoint = Config.Network.FadeCandyData.GetEndPoint();
          _socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
          _socket.Connect(endPoint);
          if (_socket.Connected && NetworkState == NetworkStates.Connecting) {
            NetworkState = NetworkStates.Connected;
            Log.Info($"Connected to FadeCandy Server.");
          }
          else {
            CloseSocket();
          }
        }
        catch (Exception e) {
          Log.Error($"Caught exception while trying to connect to FadeCandy server: {e}");
        }

        if (NetworkState == NetworkStates.Connecting) {
          Thread.Sleep(Config.Network.FadeCandyReconnectInterval_ms);
        }
      }
    }

    private void SetPixels(Color[] leds, int channel = 0) {
      if (_socket == null || !_socket.Connected) {
        return;
      }

      if (!_socket.Connected && NetworkState == NetworkStates.Connected) {
        CloseSocket();
        TryConnect();
        return;
      }

      var highByte = LightCount * 3 / 256;
      var lowByte = (LightCount * 3) % 256;

      var parts = new List<byte> {
        Convert.ToByte(channel),
        Convert.ToByte(0), //command to set LEDs using 8Bit pixel size
        Convert.ToByte(highByte),
        Convert.ToByte(lowByte)
      };

      for (var i = 0; i < LightCount; i++) {
        Color color;
        if (leds.Length <= 0) {
          color = Color.Black;
        }
        else if (i < leds.Length) {
          color = leds[i];
        }
        else {
          color = leds[leds.Length - 1];
        }
        if (i == 0) {
          if (i == 0) {
            CurrentColor.Value = color;
          }
        }

        color.Scale(Config.Display.LightingIntensity);

        parts.Add(color.R);
        parts.Add(color.G);
        parts.Add(color.B);

      }

      try {
        _socket.Send(parts.ToArray());
      }
      catch (Exception e) {
        Log.Error($"Caught exception while trying to set LEDs: {e}");
        CloseSocket();
        TryConnect();
      }
    }

    #endregion

    #region Shortcuts

    public int Channel {
      get { return Config.Display.FadeCandyChannel; }
    }

    public int LightCount {
      get { return Config.Display.FadeCandyLightCount; }
    }

    #endregion
  }
}
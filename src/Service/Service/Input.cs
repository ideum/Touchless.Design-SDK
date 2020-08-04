using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Gma.System.MouseKeyHook;

namespace TouchlessDesign {

  public class Input : App.Component, ICursor {

    private const string ProvidersRootDir = "bin/Service/InputProviders/";

    public Property<bool> IsEmulationEnabled { get; } = new Property<bool>(true);

    public Property<HoverStates> HoverState { get; } = new Property<HoverStates>(HoverStates.None);
    
    public Property<bool> IsButtonDown { get; } = new Property<bool>(false);
    
    public Property<bool> IsNoTouch { get; } = new Property<bool>(false);

    public HotspotsCollection Hotspots { get; } = new HotspotsCollection();

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

    private InputSettings _settings;
    private IKeyboardMouseEvents _hook;
    private DateTime? _timeSinceToggleEmulationKeyCombination;
    
    public override void Start() {
      _settings = InputSettings.Get(DataDir);
      InitializeHooks();
      InitializeInputProvider();
    }

    public override void Stop() {
      DeinitializeInputProvider();
      DeInitializeHooks();
    }

    #region Input Provider
    private IInputProvider _provider;

    private void InitializeInputProvider() {
      var providerInterfaceType = typeof(IInputProvider);
      var path = Path.Combine(App.DataDir, ProvidersRootDir, _settings.InputProviderPath);
      try {
        if (!File.Exists(path)) {
          Log.Error($"Could not load input provider. File does not exist at {path}");
          return;
        }
      }
      catch (Exception e) {
        Log.Error($"Caught file exception related to input provider: {e}");
        return;
      }

      if (!TryLoadAssemblyFrom(path, out var a)) {
        Log.Error("Failed to load assembly for provider.");
        return;
      }

      Log.Info("Provider assembly loaded. Searching for instances.");
      Type providerType = null;
      foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
        foreach (var type in assembly.GetTypes()) {
          if (!providerInterfaceType.IsAssignableFrom(type) || !type.IsClass || type.IsAbstract) continue;
          providerType = type;
          break;
        }
      }

      if (providerType == null) {
        Log.Error($"No {providerInterfaceType.Name} implementation could be found inside the loaded assembly {path}");
        return;
      }

      object instance = null;
      try {
        instance = Activator.CreateInstance(providerType);
        _provider = (IInputProvider) instance;
      }
      catch (Exception e) {
        Log.Error($"Exception thrown when instantiating or casting provider '{instance.GetType().Name}': {e}");
        return;
      }

      if (_provider == null) {
        Log.Error($"Unknown error. Provider could not be instantiated/cast to {providerInterfaceType.Name}");
        return;
      }

      _provider.Cursor = this;
      _provider.DataDir = App.DataDir;
      _provider.Hotspots = Hotspots;

      try {
        _provider.Start();
        Log.Info($"Input provider '{_provider.GetType().Name}'");
      }
      catch (Exception e) {
        Log.Error($"Exception caught while starting {_provider.GetType().Name}. {e}");
      }
    }

    public void DeinitializeInputProvider() {
      _provider?.Stop();
    }

    private static bool TryLoadAssemblyFrom(string path, out Assembly assembly) {
      try {
        assembly = Assembly.UnsafeLoadFrom(path);
        return true;
      }
      catch (Exception e) {
        Log.Error(e);
        assembly = null;
        return false;
      }
    }

    #endregion

    #region Global Mouse / Keyboard Hooks

    private void InitializeHooks() {
      _hook = Hook.GlobalEvents();
      var d = new Dictionary<Combination, Action> {
        {Combination.FromString(_settings.ToggleEmulationKeyCombination), HandleToggleEmulationKeyCombination}
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
        if (delta.TotalMilliseconds > _settings.ToggleEmulationToggleSpeed_ms) {
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
        Cursor.SetMouseButtonDown(false);
      }
      IsEmulationEnabled.Value = v = !v;

      Log.Info($"Toggle Emulation invoked - mouse emulation: {v}");
    }

    #endregion

    #region ICursor

    public ICursor Cursor {
      get { return this; }
    }

    bool ICursor.IsEmulationEnabled {
      get { return IsEmulationEnabled.Value; }
    }

    bool ICursor.IsButtonDown {
      get { return IsButtonDown.Value; }
    }

    int ICursor.BoundsLeft {
      get { return Bounds.X; }
    }

    int ICursor.BoundsRight {
      get { return Bounds.Right; }
    }

    int ICursor.BoundsTop {
      get { return Bounds.Y; }
    }

    int ICursor.BoundsBottom {
      get { return Bounds.Bottom; }
    }

    int ICursor.BoundsWidth {
      get { return Bounds.Width; }
    }

    int ICursor.BoundsHeight {
      get { return Bounds.Height; }
    }

    void ICursor.SetPosition(int x, int y) {
      if (!IsEmulationEnabled.Value) return;
      var b = Bounds;
      if (x < b.Left) x = b.Left;
      if (x > b.Right) x = b.Right;
      if (y > b.Bottom) y = b.Bottom;
      if (y < b.Top) y = b.Top;
      SetCursorPos(x, y);
    }

    void ICursor.SetMouseButtonDown(bool isDown) {
      if (!IsEmulationEnabled.Value) return;
      var status = isDown ? MouseEventFlags.LeftDown : MouseEventFlags.LeftUp;
      if (IsButtonDown.Value == isDown) return;
      IsButtonDown.Value = isDown;
      mouse_event((uint)status, 0, 0, 0, 0);
    }
    
    void ICursor.SetPositionAndButton(int x, int y, bool isDown) {
      if (!IsEmulationEnabled.Value) return;
      var b = Bounds;
      if (x < b.Left) x = b.Left;
      if (x > b.Right) x = b.Right;
      if (y > b.Bottom) y = b.Bottom;
      if (y < b.Top) y = b.Top;
      SetCursorPos(x, y);
      var status = isDown ? MouseEventFlags.LeftDown : MouseEventFlags.LeftUp;
      IsButtonDown.Value = isDown;
      mouse_event((uint)status, 0, 0, 0, 0);
    }

    void ICursor.DoClick() {

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

    #region Settings

    public class InputSettings {

      private const string Filename = "input.json";

      public string ToggleEmulationKeyCombination = "Control+Alt+I";
      public double ToggleEmulationToggleSpeed_ms = 3000d;
      public int ClickDuration_ms = 100;
      public string InputProviderPath = "Leap/Providers.LeapMotion.dll";

      public static InputSettings Get(string dir) {
        var path = Path.Combine(dir, Filename);
        return ConfigFactory.Get(path, Defaults);
      }

      public void Save(string dir) {
        var path = Path.Combine(dir, Filename);
        ConfigFactory.Save(path, this);
      }

      public static InputSettings Defaults() {
        return new InputSettings {
          ToggleEmulationKeyCombination = "Control+Alt+I",
          ToggleEmulationToggleSpeed_ms = 3000d,
          ClickDuration_ms = 100,
          InputProviderPath = "Leap/Providers.LeapMotion.dll"
        };
      }
    }

    #endregion
  }
}
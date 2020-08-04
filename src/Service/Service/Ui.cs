using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace TouchlessDesign {
  public class Ui : App.Component {

    private bool? _hasAddOnScreen;

    public bool HasAddOnScreen {
      get {
        if (!_hasAddOnScreen.HasValue) {
          _hasAddOnScreen = Screen.AllScreens.Length > 1;
        }
        return _hasAddOnScreen.Value;
      }
    }

    private Rectangle? _addOnBounds;

    public Rectangle AddOnScreenBounds {
      get {
        if (!_addOnBounds.HasValue) {
          if (HasAddOnScreen) {
            var screen = Screen.AllScreens.First(p => !p.Primary);
            _addOnBounds = screen.Bounds;
          }
          else {
            _addOnBounds = new Rectangle();
          }
        }
        return _addOnBounds.Value;
      }
    }

    public int AddOnWidth_mm {
      get { return _settings.AddOnWidth_mm; }
    }

    public int AddOnHeight_mm {
      get { return _settings.AddOnHeight_mm; }
    }

    public override void Start() {
      _settings = UiSettings.Get(DataDir);
      InitializeNotificationArea();
      InitializeExternalApplications();
    }
    
    public override void Stop() {
      DeInitializeNotificationArea();
      DeInitializeExternalApplications();
    }

    #region Notification Area

    private Icon _icon;
    private NotifyIcon _notify;
    private MenuItem _emulationEnabled, _hoverToggle, _noTouchToggle, _dimLightsToggle, _dragToggle;

    private void InitializeNotificationArea() {
      var assembly = System.Reflection.Assembly.GetExecutingAssembly();
      var iconStream = assembly.GetManifestResourceStream("TouchlessDesign.Resources.icon.ico");
      _icon = iconStream == null ? SystemIcons.Application : new Icon(iconStream);
      _emulationEnabled = new MenuItem("Mouse Emulation", HandleToggleEmulationClicked);
      _emulationEnabled.Checked = true;
      _hoverToggle = new MenuItem("Toggle Click Hover", HandleToggleClickHover);
      _noTouchToggle = new MenuItem("Toggle No Touch", HandleNoTouchClicked);
      _dimLightsToggle = new MenuItem("Dim Lights Toggle", HandleDimLightsClicked);
      _dragToggle = new MenuItem("Toggle Drag", HandleDragToggleClicked);
      var menuItems = new List<MenuItem>();
      menuItems.Add(_emulationEnabled);
      if (_settings.DeveloperMode) {
        menuItems.Add(new MenuItem("-"));
        menuItems.Add(new MenuItem("Open Directory", HandleOpenDataDirClicked));
        menuItems.Add(_hoverToggle);
        menuItems.Add(_dragToggle);
        menuItems.Add(_noTouchToggle);
        menuItems.Add(_dimLightsToggle);
      }
      menuItems.Add(new MenuItem("-"));
      menuItems.Add(new MenuItem("Exit", HandleExitClicked));

      _notify = new NotifyIcon {
        Icon = _icon,
        ContextMenu = new ContextMenu(menuItems.ToArray()),
        Visible = true
      };

      Input.IsEmulationEnabled.AddChangedListener(HandleEmulationChanged);
      Input.HoverState.AddChangedListener(HandleHoverStateChanged);
      Input.IsNoTouch.AddChangedListener(HandleNoTouchChanged);
      Controller.DimLights.AddChangedListener(HandleDimLightsChanged);
    }

    private void DeInitializeNotificationArea() {
      _notify.Visible = false;
      _icon?.Dispose();
      _notify?.Dispose();
      _icon = null;
      _notify = null;
    }

    private void HandleEmulationChanged(Property<bool> property, bool oldValue, bool value) {
      _emulationEnabled.Checked = value;
    }

    private void HandleHoverStateChanged(Property<HoverStates> property, HoverStates oldValue, HoverStates value) {
      _hoverToggle.Checked = value == HoverStates.Click;
      _dragToggle.Checked = value == HoverStates.Drag;
    }

    private void HandleNoTouchChanged(Property<bool> property, bool oldValue, bool value) {
      _noTouchToggle.Checked = value;
    }

    private void HandleDimLightsChanged(Property<bool> property, bool oldValue, bool value) {
      _dimLightsToggle.Checked = value;
    }

    private void HandleOpenDataDirClicked(object sender, EventArgs e) {
      Process.Start(new ProcessStartInfo {
        FileName = App.DataDir,
        UseShellExecute = true
      });
    }

    private void HandleToggleClickHover(object sender, EventArgs e) {
      HoverStates value;
      switch (Input.HoverState.Value) {
        case HoverStates.None:
          value = HoverStates.Click;
          break;
        case HoverStates.Click:
        case HoverStates.Drag:
        case HoverStates.DragHorizontal:
        case HoverStates.DragVertical:
          value = HoverStates.None;
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
      Input.HoverState.Value = value;
    }

    private void HandleDragToggleClicked(object sender, EventArgs e) {
      HoverStates value;
      switch (Input.HoverState.Value) {
        case HoverStates.None:
          value = HoverStates.Drag;
          break;
        case HoverStates.Click:
        case HoverStates.Drag:
        case HoverStates.DragHorizontal:
        case HoverStates.DragVertical:
          value = HoverStates.None;
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
      Input.HoverState.Value = value;
    }

    private void HandleNoTouchClicked(object sender, EventArgs e) {
      Input.IsNoTouch.Value = !Input.IsNoTouch.Value;
    }

    private void HandleToggleEmulationClicked(object sender, EventArgs e) {
      App.Input.IsEmulationEnabled.Value = !App.Input.IsEmulationEnabled.Value;
    }

    private void HandleExitClicked(object sender, EventArgs e) {
      App.Stop();
    }

    private void HandleDimLightsClicked(object sender, EventArgs e) {
      Controller.DimLights.Value = !Controller.DimLights.Value;
    }

    #endregion

    #region External Applications
    private readonly List<Process> _processes = new List<Process>();

    private void InitializeExternalApplications() {
      foreach (var rawPath in _settings.ApplicationPaths) {
        var path = Path.Combine(DataDir, rawPath);
        try {
          if (!File.Exists(path)) {
            Log.Error($"External Exe at {path} does not exist.");
            continue;
          }
          var process = Process.Start(path);
          _processes.Add(process);
        }
        catch (Exception e) {
          Log.Error($"Caught exception while trying to start external application at {path}: {e}");
        }
      }
    }

    private void DeInitializeExternalApplications() {
      foreach (var process in _processes) {
        try {
          if (process.HasExited) continue;
          process.Kill();
        }
        catch (Exception e) {
          Log.Error($"Exception thrown while attempting to kill process '{process.ProcessName}': {e}");
        }
      }
      _processes.Clear();
    }

    #endregion

    #region Settings

    private UiSettings _settings;

    public class UiSettings {

      private const string Filename = "ui.json";

      public string[] ApplicationPaths;
      public bool DeveloperMode = true;
      public int AddOnWidth_mm = 76;
      public int AddOnHeight_mm = 49;

      public static UiSettings Get(string dir) {
        var path = Path.Combine(dir, Filename);
        return ConfigFactory.Get(path, Defaults);
      }

      public void Save(string dir) {
        var path = Path.Combine(dir, Filename);
        ConfigFactory.Save(path, this);
      }

      public static UiSettings Defaults() {
        return new UiSettings {
          ApplicationPaths = new[] {
            "bin/Overlay/TouchlessDesign.FrontEnd.exe",
            "bin/AddOn/TouchlessDesignService.AddOnUI.exe"
          },
          DeveloperMode = true,
          AddOnWidth_mm = 76,
          AddOnHeight_mm = 49
        };
      }
    }

    #endregion
  }
}
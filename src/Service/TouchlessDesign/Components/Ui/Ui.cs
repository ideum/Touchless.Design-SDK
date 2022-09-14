using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Threading;
using IWshRuntimeLibrary;
using TouchlessDesign.Config;
using File = System.IO.File;

namespace TouchlessDesign.Components.Ui {
  public class Ui : AppComponent {


    private bool? _hasAddOnScreen;
    private bool _addOnIsPrimary;

    public bool HasAddOnScreen {
      get {
        if (!_hasAddOnScreen.HasValue) {
          if(Screen.AllScreens.Length < 2) {
            _addOnIsPrimary = (Screen.AllScreens.Length > 0 && !Config.Display.PedestalMode);
          }
          _hasAddOnScreen = Screen.AllScreens.Length > 1 || _addOnIsPrimary;
          Log.Debug($"Has addon: {_hasAddOnScreen.Value}");
        }
        Log.Debug($"Do we have?: {_hasAddOnScreen.Value}");
        return _hasAddOnScreen.Value;
      }
    }

    private Rectangle? _addOnBounds;

    public Rectangle AddOnScreenBounds {
      get {
        if (!_addOnBounds.HasValue) {
          if (HasAddOnScreen) {
            Screen screen = Screen.PrimaryScreen;
            if(!_addOnIsPrimary) {
              screen = Screen.AllScreens.First(p => !p.Primary);
            }
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
      get { return 76; }
    }

    public int AddOnHeight_mm {
      get { return 49; }
    }

    private MainWindow _mainWindow;
    private readonly App _app;

    public Ui(App app) {
      _app = app;
    }

    protected override void DoStart() {
      if (Config.General.UiStartUpDelay <= 0) {
        PerformStartupOperations();
      }
      else {
        ThreadPool.QueueUserWorkItem(InitMe);
      }
    }

    private void InitMe(object state) {
      Thread.Sleep(Config.General.UiStartUpDelay);
      PerformStartupOperations();
    }

    private void PerformStartupOperations() {
      CheckStartup();
      _app.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => {
        NotificationArea.Start(this);
        _mainWindow = new MainWindow();
        _app.MainWindow = _mainWindow;
        if (Config.General.ShowUiOnStartup) {
          _mainWindow.ShowWindow();
        }
        InitializeExternalApplications();
      }));
    }

    protected override void DoStop() {
      NotificationArea.Stop();
      DeInitializeExternalApplications();
    }

    public void ShowUi() {
      _app.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => {
        _mainWindow.ShowWindow();
      }));
    }

    private void CheckStartup() {
      const string startupFilename = "Touchless.Design.Service.lnk";
      var startupPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), startupFilename);
      var shortcutExists = File.Exists(startupPath);
      if (!Config.General.StartOnStartup) {
        if (shortcutExists) {
          try {
            File.Delete(startupPath);
          }
          catch (Exception e) {
            Log.Error(e);
          }
        }
      }
      else {
        if (!shortcutExists) {
          try {
            var shell = new WshShell();
            var windowsApplicationShortcut = (IWshShortcut)shell.CreateShortcut(startupPath);
            windowsApplicationShortcut.Description = "Touchless.Design Service";
            windowsApplicationShortcut.TargetPath = Application.ExecutablePath;
            windowsApplicationShortcut.Save();
          }
          catch (Exception e) {
            Log.Error(e);
          }
        }
      }
    }


    #region External Applications

    private const string OverlayFilename = "bin/Overlay/TouchlessDesign.FrontEnd.exe";
    private const string AddOnFilename = "bin/AddOn/TouchlessDesignService.AddOnUI.exe";

    private readonly List<ProcessWindowManager> _processes = new List<ProcessWindowManager>();

    public ProcessWindowManager OverlayWindowManager, AddOnWindowManager;

    private void InitializeExternalApplications() {
      string path;
      Process process;
      
      if (Config.Display.OverlayEnabled) {
        path = Path.Combine(DataDir, OverlayFilename);
        if (TryStartProcess(path, out process)) {
          OverlayWindowManager = new ProcessWindowManager(process, null, HandleOverlayWindowShown);
          _processes.Add(OverlayWindowManager);
        }
        else {
          Log.Error("Failed to start overlay process.");
        }
      }

      if (Config.Display.AddOnEnabled) {
        path = Path.Combine(DataDir, AddOnFilename);
        if (TryStartProcess(path, out process)) {
          AddOnWindowManager = new ProcessWindowManager(process, null, HandleAddOnWindowShown);
          _processes.Add(AddOnWindowManager);
        }
        else {
          Log.Error("Failed to start add-on process");
        }
      }
    }

    private void HandleOverlayWindowShown(ProcessWindowManager obj) {
      Log.Info("Overlay shown");
      if (!SetScreen(obj, ref Config.Display.OverlayDisplay, DisplayInfo.PrimaryDisplay, () => {
        App.Current.Dispatcher.BeginInvoke(new Action(() => { App.Instance.AppViewModel.Display.RefreshDisplays(); }));
      })) {
        Log.Warn("Failed to set overlay screen.");
      }
    }

    private void HandleAddOnWindowShown(ProcessWindowManager obj) {
      Log.Info("AddOn shown");
      if (!SetScreen(obj, ref Config.Display.AddOnDisplay, DisplayInfo.SecondaryDisplay, () => {
        App.Current.Dispatcher.BeginInvoke(new Action(() => { App.Instance.AppViewModel.Display.RefreshDisplays(); }));
      })) {
        Log.Warn("Failed to set add-on screen.");
      }
    }

    public bool SetScreen(ProcessWindowManager p, ref DisplayInfo displayInfo, DisplayInfo fallback, Action displayInfoChanged = null) {
      var screens = Screen.AllScreens;
      if (screens.Length <= 0) return false;

      if (displayInfo == null && fallback == null) return false;

      if (displayInfo == null) {
        displayInfo = fallback;
      }

      foreach (var s in screens) {
        if (s.IsEqual(displayInfo)) {
          p.SetWindowPosition(s);
          return true;
        }
      }

      foreach (var s in screens) {
        if (s.Primary == displayInfo.Primary) {
          p.SetWindowPosition(s);
          displayInfo = new DisplayInfo(s);
          displayInfoChanged?.Invoke();
          return true;
        }
      }

      return false;
    }

    private bool TryStartProcess(string path, out Process process) {
      try {
        if (!File.Exists(path)) {
          Log.Error($"External Exe at {path} does not exist.");
          process = null;
          return false;
        }
        process = Process.Start(path);
        return true;
      }
      catch (Exception e) {
        Log.Error($"Caught exception while trying to start external application at {path}: {e}");
      }
      process = null;
      return false;
    }

    private void DeInitializeExternalApplications() {
      foreach (var process in _processes) {
        process.Stop();
      }
      _processes.Clear();
    }

    #endregion

    #region Settings

    //private UiSettings _settings;

    //public class UiSettings {

    //  private const string Filename = "ui.json";

    //  public string[] ApplicationPaths;
    //  public bool DeveloperMode = true;
    //  public int AddOnWidth_mm = 76;
    //  public int AddOnHeight_mm = 49;
    //  public bool StartOnStartup = true;

    //  public static UiSettings Get(string dir) {
    //    var path = Path.Combine(dir, Filename);
    //    return Config.Factory.Get(path, Defaults);
    //  }

    //  public void Save(string dir) {
    //    var path = Path.Combine(dir, Filename);
    //    Config.Factory.Save(path, this);
    //  }

    //  public static UiSettings Defaults() {
    //    return new UiSettings {
    //      ApplicationPaths = new[] {
    //        "bin/Overlay/TouchlessDesign.FrontEnd.exe",
    //        "bin/AddOn/TouchlessDesignService.AddOnUI.exe"
    //      },
    //      DeveloperMode = true,
    //      AddOnWidth_mm = 76,
    //      AddOnHeight_mm = 49,
    //      StartOnStartup = true
    //    };
    //  }
    //}

    #endregion
  }
}
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
using File = System.IO.File;

namespace TouchlessDesign.Components.Ui {
  public class Ui : AppComponent {


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

    private readonly List<Process> _processes = new List<Process>();

    private void InitializeExternalApplications() {

      var appPaths = new List<string>();

      if (Config.Display.OverlayEnabled) {
        appPaths.Add(Path.Combine(DataDir, OverlayFilename));
      }

      if (Config.Display.AddOnEnabled) {
        appPaths.Add(Path.Combine(DataDir, AddOnFilename));
      }

      foreach (var rawPath in appPaths) {
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
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using TouchlessDesign.Components;
using TouchlessDesign.Components.Ui.ViewModels;

namespace TouchlessDesign {
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application {

    private string _dataDir;
    
    public string DataDir {
      get {
        if (string.IsNullOrEmpty(_dataDir)) {
          _dataDir = Environment.ExpandEnvironmentVariables("%appdata%/Ideum/TouchlessDesign/");
        }
        return _dataDir;
      }
    }

    public StatusViewModel StatusViewModel;

    public AppViewModel AppViewModel;

    public static void Close() {
      Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => {
        Current.Shutdown(0);
      }));
    }

    public static App Instance {
      get { return Current as App; }
    }

    protected override void OnStartup(StartupEventArgs e) {
      base.OnStartup(e);
      if (!Directory.Exists(DataDir)) {
        try {
          Directory.CreateDirectory(DataDir);
        }
        catch (Exception ex) {
          Trace.TraceError(ex.ToString());
        }
      }
      Log.Init(DataDir);
      AppComponent.InitializeComponents(this);
      StatusViewModel = TryFindResource("StatusViewModel") as StatusViewModel;
      AppViewModel = TryFindResource("AppViewModel") as AppViewModel;
    }

    protected override void OnExit(ExitEventArgs e) {
      base.OnExit(e);
      AppComponent.DeInitializeComponents();
      Log.DeInit();
    }
  }
}

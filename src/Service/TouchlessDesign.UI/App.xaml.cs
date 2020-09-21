using System;
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
          _dataDir = Environment.ExpandEnvironmentVariables("%appdata%/Ideum/TouchlessDesignService/");
        }
        return _dataDir;
      }
    }

    public StatusViewModel StatusViewModel;

    public static void Close() {
      Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => {
        Current.Shutdown(0);
      }));
    }

    protected override void OnStartup(StartupEventArgs e) {
      base.OnStartup(e);
      Log.Init(DataDir);
      AppComponent.InitializeComponents(this);
      StatusViewModel = TryFindResource("StatusViewModel") as StatusViewModel;
    }

    protected override void OnExit(ExitEventArgs e) {
      base.OnExit(e);
      AppComponent.DeInitializeComponents();
      Log.DeInit();
    }
  }
}

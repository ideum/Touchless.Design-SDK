using System.Windows;
using TouchlessDesign.Config;

namespace TouchlessDesign.Components.Ui.ViewModels {
  public class GeneralViewModel : VM<ConfigGeneral> {

    public static readonly DependencyProperty StartOnStartUpProperty = Reg<GeneralViewModel, bool>("StartOnStartUp", true, PropertyTypes.Restart);

    public bool StartOnStartup {
      get { return (bool) GetValue(StartOnStartUpProperty); }
      set { SetValue(StartOnStartUpProperty, value); }
    }

    public static readonly DependencyProperty ShowUiOnStartupProperty = Reg<GeneralViewModel, bool>("ShowUiOnStartup", true, PropertyTypes.Restart);

    public bool ShowUiOnStartup {
      get { return (bool)GetValue(ShowUiOnStartupProperty); }
      set { SetValue(ShowUiOnStartupProperty, value); }
    }

    public static readonly DependencyProperty UiStartupDelayProperty = Reg<GeneralViewModel, int>("UiStartupDelay", 0, PropertyTypes.Restart);

    public int UiStartupDelay {
      get { return (int)GetValue(UiStartupDelayProperty); }
      set { SetValue(UiStartupDelayProperty, value); }
    }

    public static readonly DependencyProperty RemoteProviderModeProperty = Reg<GeneralViewModel, bool>("RemoteProviderMode", false, PropertyTypes.Restart);

    public bool RemoteProviderMode {
      get { return (bool)GetValue(RemoteProviderModeProperty); }
      set { SetValue(RemoteProviderModeProperty, value); }
    }

    public GeneralViewModel() {

    }

    protected override void AssignModel() {
      Model = AppComponent.Config.General;
    }

    public override void ApplyValuesToModel() {
      Model.StartOnStartup = StartOnStartup;
      Model.ShowUiOnStartup = ShowUiOnStartup;
      Model.UiStartUpDelay = UiStartupDelay;
      Model.RemoteProviderMode = RemoteProviderMode;
    }

    public override void UpdateValuesFromModel() {
      StartOnStartup = Model.StartOnStartup;
      ShowUiOnStartup = Model.ShowUiOnStartup;
      UiStartupDelay = Model.UiStartUpDelay;
      RemoteProviderMode = Model.RemoteProviderMode;
    }
  }
}
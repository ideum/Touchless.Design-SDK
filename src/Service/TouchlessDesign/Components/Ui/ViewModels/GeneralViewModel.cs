using System.Windows;
using TouchlessDesign.Config;

namespace TouchlessDesign.Components.Ui.ViewModels {
  public class GeneralViewModel : VM<ConfigGeneral> {

    public static readonly DependencyProperty StartOnStartUpProperty = Reg<GeneralViewModel, bool>("StartOnStartUp", true, PropertyTypes.Restart);

    public bool StartOnStartup {
      get { return (bool) GetValue(StartOnStartUpProperty); }
      set { SetValue(StartOnStartUpProperty, value); }
    }

    public static readonly DependencyProperty UiStartupDelayProperty = Reg<GeneralViewModel, int>("UiStartupDelay", 0, PropertyTypes.Restart);

    public int UiStartupDelay {
      get { return (int)GetValue(UiStartupDelayProperty); }
      set { SetValue(UiStartupDelayProperty, value); }
    }

    public GeneralViewModel() {

    }

    protected override void AssignModel() {
      Model = AppComponent.Config.General;
    }

    public override void ApplyValuesToModel() {
      Model.StartOnStartup = StartOnStartup;
      Model.UiStartUpDelay = UiStartupDelay;
    }

    public override void UpdateValuesFromModel() {
      StartOnStartup = Model.StartOnStartup;
      UiStartupDelay = Model.UiStartUpDelay;
    }
  }
}
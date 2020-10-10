using System.Windows;
using TouchlessDesign.Config;

namespace TouchlessDesign.Components.Ui.ViewModels {
  public class GeneralViewModel : VM<ConfigGeneral> {

    public static readonly DependencyProperty StartOnStartUpProperty = Reg<GeneralViewModel, bool>("StartOnStartUp", true, PropertyTypes.Restart);

    public bool StartOnStartup {
      get { return (bool) GetValue(StartOnStartUpProperty); }
      set { SetValue(StartOnStartUpProperty, value); }
    }

    public GeneralViewModel() {

    }

    protected override void AssignModel() {
      Model = AppComponent.Config.General;
    }

    public override void ApplyValuesToModel() {
      Model.StartOnStartup = StartOnStartup;
    }

    public override void UpdateValuesFromModel() {
      StartOnStartup = Model.StartOnStartup;
    }
  }
}
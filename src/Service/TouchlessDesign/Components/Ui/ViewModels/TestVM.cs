using System.Windows;
using TouchlessDesign.Config;

namespace TouchlessDesign.Components.Ui.ViewModels {
  public class TestVM : VM<ConfigGeneral> {

    public static readonly DependencyProperty StartOnStartupProperty = Reg<TestVM, bool>("StartOnStartup", true);

    public bool StartOnStartup {
      get { return (bool) GetValue(StartOnStartupProperty); }
      set { SetValue(StartOnStartupProperty, value); }
    }

    public override void ApplyValuesToModel() {
      Model.StartOnStartup = StartOnStartup;
    }

    public override void UpdateValuesFromModel() {
      StartOnStartup = Model.StartOnStartup;
    }
  }
}
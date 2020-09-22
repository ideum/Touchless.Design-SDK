using TouchlessDesign.Components.Ui.ViewModels.Properties;

namespace TouchlessDesign.Components.Ui.ViewModels {
  public class GeneralViewModel : PropertyBase {
    public override object BaseValue { get; set; }

    public BoolProperty StartOnStartup { get; } = new BoolProperty {Name = "Start on Start-up", Value = true};
  }
}
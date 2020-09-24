using System.Windows.Forms;
using TouchlessDesign.Components.Ui.ViewModels.Properties;

namespace TouchlessDesign.Components.Ui.ViewModels {
  public class DisplayViewModel : PropertyBase {
    public override object BaseValue { get; set; }

    public ScreenObservableCollection Displays { get; } = new ScreenObservableCollection(); 

    public BoolProperty OverlayEnabled { get; } = new BoolProperty { Name = "Overlay" };

    public IntClampedProperty OverlayIndex { get; } = new IntClampedProperty { Name = "Overlay Target Display" };

    public BoolProperty AddOnEnabled { get; } = new BoolProperty { Name = "Add-On" };

    public IntClampedProperty AddOnIndex { get; } = new IntClampedProperty { Name = "Add-On Target Display" };

    public BoolProperty LEDsEnabled { get; } = new BoolProperty { Name = "LEDs" };

    public DoubleClampedProperty LEDIntensity { get; } = new DoubleClampedProperty {Name = "LED Intensity", Minimum = 0, Maximum = 1, SmallChange = 0.001, LargeChange = 0.01, Value = 0.5};

    public DisplayViewModel() {
      var properties = new IProperty[] {OverlayEnabled, OverlayIndex, AddOnEnabled, AddOnIndex, LEDsEnabled, LEDIntensity};
      foreach (var p in properties) {
        p.Changed += HandlePropertyChanged;
      }
    }

    private void HandlePropertyChanged(IProperty obj) {
      InvokeChanged();
    }

    public void RefreshDisplays() {
      Log.Info("Updating Screens");
      Displays.Clear();
      var screens = Screen.AllScreens;
      for (var i = 0; i < screens.Length; i++) {
        var s = screens[i];
        var vm = new ScreenViewModel();
        vm.SetScreen(i, s);
        Displays.Add(vm);
        Log.Info($"Screen '{vm.Name}'");
      }
    }
  }
}
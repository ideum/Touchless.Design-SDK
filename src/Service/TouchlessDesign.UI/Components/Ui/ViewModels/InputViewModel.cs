using TouchlessDesign.Components.Ui.ViewModels.Properties;

namespace TouchlessDesign.Components.Ui.ViewModels {
  public class InputViewModel : PropertyBase {

    public BoolProperty MouseEmulation { get; } = new BoolProperty {Name = "Mouse Emulation"};

    public StringProperty MouseEmulationKeyboardShortcut { get; } = new StringProperty {Name = "Mouse Emulation Keyboard Shortcut"};

    public ClampedDoubleProperty ClickThreshold { get; } = new ClampedDoubleProperty {Name = "Click-Grab Threshold", Minimum = 0, Maximum = 1, Value = 0.75};

    public BoolProperty ClickEmulation { get; } = new BoolProperty{Name = "Click Emulation"};

    public ClampedIntProperty ClickEmulationDuration { get; } = new ClampedIntProperty{Name = "Click Duration", Minimum = 1, Maximum = 2000};

    public StringProperty InputProviderPath { get; } = new StringProperty { Name = "Input Provider Path" };

    public BoolProperty FlipYAxis { get; } = new BoolProperty { Name = "Flip Y Axis" };

    public BoolProperty UseXy { get; } = new BoolProperty { Name = "Use XY" };

    public ClampedDoubleProperty MinX { get; } = new ClampedDoubleProperty { Name = "X-Min", Minimum = -0.5, Maximum = 0.5};
    
    public ClampedDoubleProperty MaxX { get; } = new ClampedDoubleProperty { Name = "X-Max", Minimum = -0.5, Maximum = 0.5 };
    
    public ClampedDoubleProperty MinZ { get; } = new ClampedDoubleProperty { Name = "Z-Min", Minimum = -0.5, Maximum = 0.5 };
    
    public ClampedDoubleProperty MaxZ { get; } = new ClampedDoubleProperty { Name = "Z-Max", Minimum = -0.5, Maximum = 0.5 };
    
    public ClampedDoubleProperty MinY { get; } = new ClampedDoubleProperty { Name = "Y-Min", Minimum = 0.1, Maximum = 1};

    public ClampedDoubleProperty MaxY { get; } = new ClampedDoubleProperty {Name = "Y-Max", Minimum = 0.1, Maximum = 1};

    public ClampedIntProperty TickRate { get; } = new ClampedIntProperty {Name = "Tick Rate", Minimum = 1, Maximum = 250, SmallChange = 1, LargeChange = 2};

    public InputViewModel() {
      var properties = new IProperty[] {MouseEmulation, MouseEmulationKeyboardShortcut, FlipYAxis, UseXy, ClickThreshold, ClickEmulation, ClickEmulationDuration,  MinX, MaxX, MinZ, MaxZ, MinY, MaxY, TickRate, InputProviderPath};
      foreach (var p in properties) {
        p.Changed += HandlePropertyChanged;
      }
    }

    private void HandlePropertyChanged(IProperty obj) {
      InvokeChanged();
    }
  }
}
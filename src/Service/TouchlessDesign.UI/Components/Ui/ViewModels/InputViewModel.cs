using TouchlessDesign.Components.Ui.ViewModels.Properties;

namespace TouchlessDesign.Components.Ui.ViewModels {
  public class InputViewModel : PropertyBase {

    public BoolProperty MouseEmulation { get; } = new BoolProperty {Name = "Mouse Emulation", Value = true};

    public StringProperty MouseEmulationKeyboardShortcut { get; } = new StringProperty {Name = "Mouse Emulation Keyboard Shortcut", Value = "Control+Alt+I"};

    public DoubleClampedProperty ClickThreshold { get; } = new DoubleClampedProperty {Name = "Click-Grab Threshold", Minimum = 0, Maximum = 1, Value = 0.75};

    public BoolProperty ClickEmulation { get; } = new BoolProperty{Name = "Click Emulation"};

    public IntClampedProperty ClickEmulationDuration { get; } = new IntClampedProperty{Name = "Click Duration", Minimum = 1, Maximum = 2000};

    public StringProperty InputProviderPath { get; } = new StringProperty { Name = "Input Provider Path" };

    public BoolProperty FlipYAxis { get; } = new BoolProperty { Name = "Flip Y Axis" };

    public BoolProperty UseXy { get; } = new BoolProperty { Name = "Use XY" };

    public DoubleClampedProperty MinX { get; } = new DoubleClampedProperty { Name = "X-Min", Minimum = -0.5, Maximum = 0.5};
    
    public DoubleClampedProperty MaxX { get; } = new DoubleClampedProperty { Name = "X-Max", Minimum = -0.5, Maximum = 0.5 };
    
    public DoubleClampedProperty MinZ { get; } = new DoubleClampedProperty { Name = "Z-Min", Minimum = -0.5, Maximum = 0.5 };
    
    public DoubleClampedProperty MaxZ { get; } = new DoubleClampedProperty { Name = "Z-Max", Minimum = -0.5, Maximum = 0.5 };
    
    public DoubleClampedProperty MinY { get; } = new DoubleClampedProperty { Name = "Y-Min", Minimum = 0.1, Maximum = 1};

    public DoubleClampedProperty MaxY { get; } = new DoubleClampedProperty {Name = "Y-Max", Minimum = 0.1, Maximum = 1};

    public IntClampedProperty TickRate { get; } = new IntClampedProperty {Name = "Tick Rate", Minimum = 1, Maximum = 250, SmallChange = 1, LargeChange = 2};

    public InputViewModel() {
      var properties = new IProperty[] {MouseEmulation, MouseEmulationKeyboardShortcut, FlipYAxis, UseXy, ClickThreshold, ClickEmulation, ClickEmulationDuration,  MinX, MaxX, MinZ, MaxZ, MinY, MaxY, TickRate, InputProviderPath};
      foreach (var p in properties) {
        p.Changed += HandlePropertyChanged;
      }
    }

    private void HandlePropertyChanged(IProperty obj) {
      InvokeChanged();
    }

    public override object BaseValue { get; set; }
  }
}
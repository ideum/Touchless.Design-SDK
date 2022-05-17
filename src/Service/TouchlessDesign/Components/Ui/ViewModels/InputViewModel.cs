using System;
using System.Windows;
using TouchlessDesign.Config;

namespace TouchlessDesign.Components.Ui.ViewModels {
  public class InputViewModel : VM<ConfigInput> {

    //public BoolProperty MouseEmulation { get; set; } = new BoolProperty {Name = "Mouse Emulation", Prop = true};

    //public StringProperty MouseEmulationKeyboardShortcut { get; set; } = new StringProperty {Name = "Keyboard Shortcut", Prop = "Control+Alt+I"};

    //public DoubleClampedProperty ClickThreshold { get; set; } = new DoubleClampedProperty {Name = "Click-Grab Threshold", Minimum = 0, Maximum = 1, Prop = 0.75};

    //public BoolProperty ClickEmulation { get; set; } = new BoolProperty{Name = "Click Emulation"};

    //public IntClampedProperty ClickEmulationDuration { get; set; } = new IntClampedProperty{Name = "Click Duration (ms)", Minimum = 1, Maximum = 2000};

    //public DoubleClampedProperty MinHandConfidence { get; set; } = new DoubleClampedProperty{ Name = "Min Hand Confidence", Minimum = 0, Maximum = 1, Prop = 0.5};

    //public StringProperty InputProvider { get; set; } = new StringProperty { Name = "Input Provider" };

    //public BoolProperty FlipYAxis { get; set; } = new BoolProperty { Name = "Invert Vertical Axis" };

    //public BoolProperty UseXy { get; set; } = new BoolProperty { Name = "Use XY" };

    //public DoubleClampedProperty MaxX { get; set; } = new DoubleClampedProperty { Name = "Max (m)", Minimum = -0.5, Maximum = 0.5 };

    //public DoubleClampedProperty MinZ { get; set; } = new DoubleClampedProperty { Name = "Min (m)", Minimum = -0.5, Maximum = 0.5 };

    //public DoubleClampedProperty MaxZ { get; set; } = new DoubleClampedProperty { Name = "Max (m)", Minimum = -0.5, Maximum = 0.5 };

    //public DoubleClampedProperty MinY { get; set; } = new DoubleClampedProperty { Name = "Min (m)", Minimum = 0.1, Maximum = 1};

    //public DoubleClampedProperty MaxY { get; set; } = new DoubleClampedProperty {Name = "Max (m)", Minimum = 0.1, Maximum = 1};

    //public IntClampedProperty TickRate { get; set; } = new IntClampedProperty {Name = "Update Rate (ms)", Minimum = 1, Maximum = 250, SmallChange = 1, LargeChange = 2};

    //public IntClampedProperty MinHue { get; set; } = new IntClampedProperty { Name = "Min", Minimum = 0, Maximum = 255, Prop = 0 };

    //public IntClampedProperty MaxHue { get; set; } = new IntClampedProperty { Name = "Max", Minimum = 0, Maximum = 255, Prop = 255 };

    //public IntClampedProperty MinSaturation { get; set; } = new IntClampedProperty { Name = "Min", Minimum = 0, Maximum = 255, Prop = 0 };

    //public IntClampedProperty MaxSaturation { get; set; } = new IntClampedProperty { Name = "Max", Minimum = 0, Maximum = 255, Prop = 255 };

    //public IntClampedProperty MinValue { get; set; } = new IntClampedProperty { Name = "Min", Minimum = 0, Maximum = 255, Prop = 0 };

    //public IntClampedProperty MaxValue { get; set; } = new IntClampedProperty { Name = "Max", Minimum = 0, Maximum = 255, Prop = 255 };

    public static readonly DependencyProperty MouseEmulationProperty = Reg<InputViewModel, bool>("MouseEmulation", true);

    public bool MouseEmulation {
      get { return (bool) GetValue(MouseEmulationProperty); }
      set { SetValue(MouseEmulationProperty, value); }
    }

    public static readonly DependencyProperty ClickEmulationDurationProperty = Reg<InputViewModel, int>("ClickEmulationDuration", 0, PropertyTypes.Restart);
    public int ClickEmulationDuration {
      get { return (int)GetValue(ClickEmulationDurationProperty); }
      set { SetValue(ClickEmulationDurationProperty, value); }
    }

    public static readonly DependencyProperty MouseEmulationKeyboardShortcutProperty = Reg<InputViewModel, string>("MouseEmulationKeyboardShortcut", "", PropertyTypes.Restart);

    public string MouseEmulationKeyboardShortcut {
      get { return (string)GetValue(MouseEmulationKeyboardShortcutProperty); }
      set { SetValue(MouseEmulationKeyboardShortcutProperty, value); }
    }

    public static readonly DependencyProperty InputProviderProperty = Reg<InputViewModel, bool>("InputProvider", false, PropertyTypes.Restart);

    public bool InputProvider {
      get { return (bool)GetValue(InputProviderProperty); }
      set { SetValue(InputProviderProperty, value); }
    }

    public static readonly DependencyProperty ClickThresholdProperty = Reg<InputViewModel, double>("ClickThreshold", 0.0, PropertyTypes.Save);

    public double ClickThreshold {
      get { return (double)GetValue(ClickThresholdProperty); }
      set { SetValue(ClickThresholdProperty, value); }
    }

    public static readonly DependencyProperty ClickEmulationProperty = Reg<InputViewModel, bool>("ClickEmulation", false, PropertyTypes.Save);

    public bool ClickEmulation {
      get { return (bool)GetValue(ClickEmulationProperty); }
      set { SetValue(ClickEmulationProperty, value); }
    }

    public static readonly DependencyProperty MinHandConfidenceProperty = Reg<InputViewModel, double>("MinHandConfidence", 0.0, PropertyTypes.Save);

    public double MinHandConfidence {
      get { return (double)GetValue(MinHandConfidenceProperty); }
      set { SetValue(MinHandConfidenceProperty, value); }
    }

    public static readonly DependencyProperty FlipYAxisProperty = Reg<InputViewModel, bool>("FlipYAxis", false, PropertyTypes.Save);

    public bool FlipYAxis {
      get { return (bool) GetValue(FlipYAxisProperty); }
      set { SetValue(FlipYAxisProperty, value); }
    }

    public static readonly DependencyProperty UseXyProperty = Reg<InputViewModel, bool>("UseXy", false, PropertyTypes.Save);

    public bool UseXy {
      get { return (bool)GetValue(UseXyProperty); }
      set { SetValue(UseXyProperty, value); }
    }

    public static readonly DependencyProperty MinXProperty = Reg<InputViewModel, double>("MinX", 0.0, PropertyTypes.Save);

    public double MinX {
      get { return (double)GetValue(MinXProperty); }
      set { SetValue(MinXProperty, value); }
    }

    public static readonly DependencyProperty MaxXProperty = Reg<InputViewModel, double>("MaxX", 0.0, PropertyTypes.Save);

    public double MaxX {
      get { return (double)GetValue(MaxXProperty); }
      set { SetValue(MaxXProperty, value); }
    }

    public static readonly DependencyProperty MinZProperty = Reg<InputViewModel, double>("MinZ", 0.0, PropertyTypes.Save);

    public double MinZ {
      get { return (double)GetValue(MinZProperty); }
      set { SetValue(MinZProperty, value); }
    }

    public static readonly DependencyProperty MaxZProperty = Reg<InputViewModel, double>("MaxZ", 0.0, PropertyTypes.Save);

    public double MaxZ {
      get { return (double)GetValue(MaxZProperty); }
      set { SetValue(MaxZProperty, value); }
    }

    public static readonly DependencyProperty MinYProperty = Reg<InputViewModel, double>("MinY", 0.0, PropertyTypes.Save);

    public double MinY {
      get { return (double)GetValue(MinYProperty); }
      set { SetValue(MinYProperty, value); }
    }

    public static readonly DependencyProperty MaxYProperty = Reg<InputViewModel, double>("MaxY", 0.0, PropertyTypes.Save);

    public double MaxY {
      get { return (double)GetValue(MaxYProperty); }
      set { SetValue(MaxYProperty, value); }
    }

    public static readonly DependencyProperty TickRateProperty = Reg<InputViewModel, int>("TickRate", 0, PropertyTypes.Save);

    public int TickRate {
      get { return (int)GetValue(TickRateProperty); }
      set { SetValue(TickRateProperty, value); }
    }

    public static readonly DependencyProperty IsHSVSupportedProperty = Reg<InputViewModel, bool>("IsHSVSupported", false);

    public bool IsHSVSupported {
      get { return (bool) GetValue(IsHSVSupportedProperty); }
      set { SetValue(IsHSVSupportedProperty, value); }
    }

    public static readonly DependencyProperty MinHueProperty = Reg<InputViewModel, int>("MinHue", 0, PropertyTypes.Save);

    public int MinHue {
      get { return (int) GetValue(MinHueProperty); }
      set { SetValue(MinHueProperty, value); }
    }

    public static readonly DependencyProperty MaxHueProperty = Reg<InputViewModel, int>("MaxHue", 0, PropertyTypes.Save);

    public int MaxHue {
      get { return (int)GetValue(MaxHueProperty); }
      set { SetValue(MaxHueProperty, value); }
    }

    public static readonly DependencyProperty MinSaturationProperty = Reg<InputViewModel, int>("MinSaturation", 0, PropertyTypes.Save);

    public int MinSaturation {
      get { return (int)GetValue(MinSaturationProperty); }
      set { SetValue(MinSaturationProperty, value); }
    }

    public static readonly DependencyProperty MaxSaturationProperty = Reg<InputViewModel, int>("MaxSaturation", 0, PropertyTypes.Save);

    public int MaxSaturation {
      get { return (int)GetValue(MaxSaturationProperty); }
      set { SetValue(MaxSaturationProperty, value); }
    }

    public static readonly DependencyProperty MinValueProperty = Reg<InputViewModel, int>("MinValue", 0, PropertyTypes.Save);

    public int MinValue {
      get { return (int)GetValue(MinValueProperty); }
      set { SetValue(MinValueProperty, value); }
    }

    public static readonly DependencyProperty MaxValueProperty = Reg<InputViewModel, int>("MaxValue", 0, PropertyTypes.Save);

    public int MaxValue {
      get { return (int)GetValue(MaxValueProperty); }
      set { SetValue(MaxValueProperty, value); }
    }

    public InputViewModel() {
      AppComponent.OnLoaded(HandleAppComponentOnLoaded);
    }

    private void HandleAppComponentOnLoaded() {
      AppComponent.Input.IsEmulationEnabled.Changed += IsEmulationEnabled_Changed;
    }

    private void IsEmulationEnabled_Changed(Property<bool> property, bool oldValue, bool value) {
      Application.Current.Dispatcher.BeginInvoke(new Action(() => { MouseEmulation = value; }));
    }

    public override void UpdateRealTimeProperties() {
      Model.GrabClickThreshold = (float)ClickThreshold;
      Model.ClickDuration_ms = ClickEmulationDuration;
      Model.ClickEnabled = ClickEmulation;

      Model.FlipYAxis = FlipYAxis;
      Model.UseXY = UseXy;
      Model.MinX = (float)MinX;
      Model.MaxX = (float)MaxX;
      Model.MinY = (float)MinY;
      Model.MaxY = (float)MaxY;
      Model.MinZ = (float)MinZ;
      Model.MaxZ = (float)MaxZ;

      Model.MinConfidence = (float)MinHandConfidence;
      Model.MinH = MinHue;
      Model.MaxH = MaxHue;
      Model.MinS = MinSaturation;
      Model.MaxS = MaxSaturation;
      Model.MinV = MinValue;
      Model.MaxV = MaxValue;
    }

    protected override void AssignModel() {
      Model = AppComponent.Config.Input;
    }

    public override void ApplyValuesToModel() {
      Model.ToggleEmulationKeyCombination = MouseEmulationKeyboardShortcut;
      Model.GrabClickThreshold = (float) ClickThreshold;
      Model.MinConfidence = (float)MinHandConfidence;
      Model.ClickDuration_ms = ClickEmulationDuration;
      Model.MouseEmulationEnabled = MouseEmulation;
      Model.ClickEnabled = ClickEmulation;
      Model.UpdateRate_ms = TickRate;
      Model.InputProvider = InputProvider ? 1 : 0;

      Model.FlipYAxis = FlipYAxis;
      Model.UseXY = UseXy;
      Model.MinX = (float) MinX;
      Model.MaxX = (float) MaxX;
      Model.MinY = (float) MinY;
      Model.MaxY = (float) MaxY;
      Model.MinZ = (float) MinZ;
      Model.MaxZ = (float) MaxZ;

      Model.MinH = MinHue;
      Model.MaxH = MaxHue;
      Model.MinS = MinSaturation;
      Model.MaxS = MaxSaturation;
      Model.MinV = MinValue;
      Model.MaxV = MaxValue;
    }

    public override void UpdateValuesFromModel() {
      MouseEmulation = Model.MouseEmulationEnabled;
      MouseEmulationKeyboardShortcut = Model.ToggleEmulationKeyCombination;
      ClickThreshold = Model.GrabClickThreshold;
      MinHandConfidence = Model.MinConfidence;
      ClickEmulationDuration = Model.ClickDuration_ms;
      ClickEmulation = Model.ClickEnabled;
      TickRate = Model.UpdateRate_ms;
      InputProvider = Model.InputProvider==0 ? false : true;
      FlipYAxis = Model.FlipYAxis;
      UseXy = Model.UseXY;
      MinX = Model.MinX;
      MaxX = Model.MaxX;
      MinY = Model.MinY;
      MaxY = Model.MaxY;
      MinZ = Model.MinZ;
      MaxZ = Model.MaxZ;
      MinHue = Model.MinH;
      MaxHue = Model.MaxH;
      MinSaturation = Model.MinS;
      MaxSaturation = Model.MaxS;
      MinValue = Model.MinV;
      MaxValue = Model.MaxV;
    }
  }
}
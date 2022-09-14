using System.Windows;
using System.Windows.Forms;
using TouchlessDesign.Config;

namespace TouchlessDesign.Components.Ui.ViewModels {
  public class DisplayViewModel : VM<ConfigDisplay> {

    public static readonly DependencyProperty DisplaysProperty = Reg<DisplayViewModel, ScreenObservableCollection>("Displays", null, PropertyTypes.None);

    public ScreenObservableCollection Displays {
      get { return (ScreenObservableCollection)GetValue(DisplaysProperty); }
      set { SetValue(DisplaysProperty, value); }
    }


    public static readonly DependencyProperty OverlayEnabledProperty = Reg<DisplayViewModel, bool>("OverlayEnabled", true, PropertyTypes.Save);

    public bool OverlayEnabled {
      get { return (bool)GetValue(OverlayEnabledProperty); }
      set { SetValue(OverlayEnabledProperty, value); }
    }

    public static readonly DependencyProperty OverlayDisplayProperty = Reg<DisplayViewModel, ScreenViewModel>("OverlayDisplay", null, PropertyTypes.Save, ValueChangedHandler);

    private static void ValueChangedHandler(DisplayViewModel arg1, string arg2, object arg3) {
      Log.Info($"{arg2}: {arg3}");
    }

    public ScreenViewModel OverlayDisplay {
      get { return (ScreenViewModel)GetValue(OverlayDisplayProperty); }
      set {
        if (value == OverlayDisplay) return;
        SetValue(OverlayDisplayProperty, value);
      }
    }

    public static readonly DependencyProperty CursorEnabledProperty = Reg<DisplayViewModel, bool>("CursorEnabled", true, PropertyTypes.Save);

    public bool CursorEnabled {
      get { return (bool)GetValue(CursorEnabledProperty); }
      set { SetValue(CursorEnabledProperty, value); }
    }

    public static readonly DependencyProperty NoTouchEnabledProperty = Reg<DisplayViewModel, bool>("NoTouchEnabled", true, PropertyTypes.Save);

    public bool NoTouchEnabled {
      get { return (bool)GetValue(NoTouchEnabledProperty); }
      set { SetValue(NoTouchEnabledProperty, value); }
    }

    public static readonly DependencyProperty OnboardingEnabledProperty = Reg<DisplayViewModel, bool>("OnboardingEnabled", true, PropertyTypes.Save);

    public bool OnboardingEnabled {
      get { return (bool)GetValue(OnboardingEnabledProperty); }
      set { SetValue(OnboardingEnabledProperty, value); }
    }

    public static readonly DependencyProperty OnboardingUIScaleProperty = Reg<DisplayViewModel, double>("OnboardingUIScale", 1.0, PropertyTypes.Save);

    public double OnboardingUIScale {
      get { return (double)GetValue(OnboardingUIScaleProperty); }
      set { SetValue(OnboardingUIScaleProperty, value); }
    }

    public static readonly DependencyProperty OnboardingStatusBarScaleProperty = Reg<DisplayViewModel, double>("OnboardingStatusBarScale", 1.0, PropertyTypes.Save);

    public double OnboardingStatusBarScale {
      get { return (double)GetValue(OnboardingStatusBarScaleProperty); }
      set { SetValue(OnboardingStatusBarScaleProperty, value); }
    }

    public static readonly DependencyProperty OnboardingStatusBarXOffsetProperty = Reg<DisplayViewModel, double>("OnboardingStatusBarXOffset", 0.0, PropertyTypes.Save);

    public double OnboardingStatusBarXOffset {
      get { return (double)GetValue(OnboardingStatusBarXOffsetProperty); }
      set { SetValue(OnboardingStatusBarXOffsetProperty, value); }
    }

    public static readonly DependencyProperty OnboardingNewUserTimeoutProperty = Reg<DisplayViewModel, int>("OnboardingNewUserTimeout", 60, PropertyTypes.Save);

    public int OnboardingNewUserTimeout {
      get { return (int)GetValue(OnboardingNewUserTimeoutProperty); }
      set { SetValue(OnboardingNewUserTimeoutProperty, value); }
    }

    public static readonly DependencyProperty OnboardingNoHandTimeoutProperty = Reg<DisplayViewModel, int>("OnboardingNoHandTimeout", 15, PropertyTypes.Save);

    public int OnboardingNoHandTimeout {
      get { return (int)GetValue(OnboardingNoHandTimeoutProperty); }
      set { SetValue(OnboardingNoHandTimeoutProperty, value); }
    }

    public static readonly DependencyProperty Onboarding1EnabledProperty = Reg<DisplayViewModel, bool>("Onboarding1Enabled", true, PropertyTypes.Save);

    public bool Onboarding1Enabled {
      get { return (bool)GetValue(Onboarding1EnabledProperty); }
      set { SetValue(Onboarding1EnabledProperty, value); }
    }

    public static readonly DependencyProperty Onboarding2EnabledProperty = Reg<DisplayViewModel, bool>("Onboarding2Enabled", true, PropertyTypes.Save);

    public bool Onboarding2Enabled {
      get { return (bool)GetValue(Onboarding2EnabledProperty); }
      set { SetValue(Onboarding2EnabledProperty, value); }
    }

    public static readonly DependencyProperty Onboarding3EnabledProperty = Reg<DisplayViewModel, bool>("Onboarding3Enabled", true, PropertyTypes.Save);

    public bool Onboarding3Enabled {
      get { return (bool)GetValue(Onboarding3EnabledProperty); }
      set { SetValue(Onboarding3EnabledProperty, value); }
    }

    public static readonly DependencyProperty AddOnEnabledProperty = Reg<DisplayViewModel, bool>("AddOnEnabled", true, PropertyTypes.Save);

    public bool AddOnEnabled {
      get { return (bool)GetValue(AddOnEnabledProperty); }
      set { SetValue(AddOnEnabledProperty, value); }
    }

    public static readonly DependencyProperty PedestalModeProperty = Reg<DisplayViewModel, bool>("PedestalMode", true, PropertyTypes.Save);

    public bool PedestalMode {
      get { return (bool)GetValue(PedestalModeProperty); }
      set { SetValue(PedestalModeProperty, value); }
    }

    public static readonly DependencyProperty AddOnDisplayProperty = Reg<DisplayViewModel, ScreenViewModel>("AddOnDisplay", null, PropertyTypes.Save);

    public ScreenViewModel AddOnDisplay {
      get { return (ScreenViewModel)GetValue(AddOnDisplayProperty); }
      set {
        if (value == AddOnDisplay) return;
        SetValue(AddOnDisplayProperty, value);
      }
    }

    public static readonly DependencyProperty LEDsEnabledProperty = Reg<DisplayViewModel, bool>("LEDsEnabled", true, PropertyTypes.Save);

    public bool LEDsEnabled {
      get { return (bool)GetValue(LEDsEnabledProperty); }
      set { SetValue(LEDsEnabledProperty, value); }
    }

    public static readonly DependencyProperty LEDIntensityProperty = Reg<DisplayViewModel, double>("LEDIntensity", 0.0, PropertyTypes.Save);

    public double LEDIntensity {
      get { return (double)GetValue(LEDIntensityProperty); }
      set { SetValue(LEDIntensityProperty, value); }
    }

    public DisplayViewModel() {
      Displays = new ScreenObservableCollection();
    }

    private bool _preventMessagesSending = false;

    protected override void DoPropertyValueChanged(string name, object value) {
      base.DoPropertyValueChanged(name, value);
      SendMessage();
    }

    private void SendMessage() {
      if (Model == null || _preventMessagesSending) return;
      UpdateRealTimeProperties();
      AppComponent.Ipc.SendSettingsMessage(AppComponent.Config.Display);
    }

    public void RefreshDisplays() {
      var sav = NeedsSave;
      SuppressSaveChanged = true;
      _preventMessagesSending = true;
      if (Displays == null) {
        Displays = new ScreenObservableCollection();
      }
      Displays.Clear();
      var screens = Screen.AllScreens;
      for (var i = 0; i < screens.Length; i++) {
        var s = screens[i];
        var vm = new ScreenViewModel();
        vm.SetScreen(i, s);
        Displays.Add(vm);
      }

      OverlayDisplay = FindClosestDisplay(Model.OverlayDisplay);
      AddOnDisplay = FindClosestDisplay(Model.AddOnDisplay);
      _preventMessagesSending = false;
      NeedsSave = sav;
      SuppressSaveChanged = false;
    }

    protected override void AssignModel() {
      Model = AppComponent.Config.Display;
    }

    public override void UpdateRealTimeProperties() {
      base.UpdateRealTimeProperties();
      SetDisplay(OverlayDisplay, ref Model.OverlayDisplay, DisplayInfo.PrimaryDisplay);
      SetDisplay(AddOnDisplay, ref Model.AddOnDisplay, DisplayInfo.SecondaryDisplay);
      Model.LightingIntensity = (float) LEDIntensity;
      Model.CursorEnabled = CursorEnabled;
      Model.NoTouchEnabled = NoTouchEnabled;
      Model.OnboardingEnabled = OnboardingEnabled;
      Model.OnboardingUIScale = (float)OnboardingUIScale;
      Model.OnboardingStatusBarScale = (float)OnboardingStatusBarScale;
      Model.OnboardingStatusBarXOffset = (float)OnboardingStatusBarXOffset;
      Model.OnboardingNewUserTimeout_s = OnboardingNewUserTimeout;
      Model.OnboardingNoHandTimeout_s = OnboardingNoHandTimeout;
      Model.Onboarding1Enabled = Onboarding1Enabled;
      Model.Onboarding2Enabled = Onboarding2Enabled;
      Model.Onboarding3Enabled = Onboarding3Enabled;
    }

    public override void ApplyValuesToModel() {
      SetDisplay(OverlayDisplay, ref Model.OverlayDisplay, DisplayInfo.PrimaryDisplay);
      SetDisplay(AddOnDisplay, ref Model.AddOnDisplay, DisplayInfo.SecondaryDisplay);
      Model.OverlayEnabled = OverlayEnabled;
      Model.CursorEnabled = CursorEnabled;
      Model.NoTouchEnabled = NoTouchEnabled;
      Model.OnboardingEnabled = OnboardingEnabled;
      Model.OnboardingUIScale = (float) OnboardingUIScale;
      Model.OnboardingStatusBarScale = (float) OnboardingStatusBarScale;
      Model.OnboardingStatusBarXOffset = (float) OnboardingStatusBarXOffset;
      Model.OnboardingNewUserTimeout_s = OnboardingNewUserTimeout;
      Model.OnboardingNoHandTimeout_s = OnboardingNoHandTimeout;
      Model.Onboarding1Enabled = Onboarding1Enabled;
      Model.Onboarding2Enabled = Onboarding2Enabled;
      Model.Onboarding3Enabled = Onboarding3Enabled;
      Model.AddOnEnabled = AddOnEnabled;
      Model.PedestalMode = PedestalMode;
      Model.LightingEnabled = LEDsEnabled;
      Model.LightingIntensity = (float) LEDIntensity;
    }

    public override void UpdateValuesFromModel() {
      RefreshDisplays();
      _preventMessagesSending = true;
      OverlayEnabled = Model.OverlayEnabled;
      OverlayDisplay = FindClosestDisplay(Model.OverlayDisplay);
      CursorEnabled = Model.CursorEnabled;
      NoTouchEnabled = Model.NoTouchEnabled;
      OnboardingEnabled = Model.OnboardingEnabled;
      OnboardingUIScale = Model.OnboardingUIScale;
      OnboardingStatusBarScale = Model.OnboardingStatusBarScale;
      OnboardingStatusBarXOffset = Model.OnboardingStatusBarXOffset;
      OnboardingNewUserTimeout = Model.OnboardingNewUserTimeout_s;
      OnboardingNoHandTimeout = Model.OnboardingNoHandTimeout_s;
      Onboarding1Enabled = Model.Onboarding1Enabled;
      Onboarding2Enabled = Model.Onboarding2Enabled;
      Onboarding3Enabled = Model.Onboarding3Enabled;
      AddOnEnabled = Model.AddOnEnabled;
      PedestalMode = Model.PedestalMode;
      AddOnDisplay = FindClosestDisplay(Model.AddOnDisplay);
      LEDsEnabled = Model.LightingEnabled;
      LEDIntensity = Model.LightingIntensity;
      _preventMessagesSending = false;
      SendMessage();
    }

    private void SetDisplay(ScreenViewModel vm, ref DisplayInfo info, DisplayInfo def = null) {
      if (info == null) {
        if (vm == null) {
          info = def;
        }
        else {
          info = new DisplayInfo(vm.Screen);
        }
      }
      else {
        if (vm == null) {
          //info = info.
        }
        else if(!vm.IsEqual(info)){
          info = new DisplayInfo(vm.Screen);
        }
      }
    }

    private ScreenViewModel FindClosestDisplay(DisplayInfo d, ScreenViewModel def = null) {
      if (d == null) {
        return def;
      }
      else {
        foreach (var display in Displays) {
          if (display.IsEqual(d)) return display;
        }
        return def;
      }
    }
  }
}
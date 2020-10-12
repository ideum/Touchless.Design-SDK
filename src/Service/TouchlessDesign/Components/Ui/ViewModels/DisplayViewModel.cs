using System.Windows;
using System.Windows.Forms;
using TouchlessDesign.Config;

namespace TouchlessDesign.Components.Ui.ViewModels {
  public class DisplayViewModel : VM<ConfigDisplay> {

    public static readonly DependencyProperty DisplaysProperty = Reg<DisplayViewModel, ScreenObservableCollection>("Displays", null, PropertyTypes.Save);

    public ScreenObservableCollection Displays {
      get { return (ScreenObservableCollection)GetValue(DisplaysProperty); }
      set { SetValue(DisplaysProperty, value); }
    }

    public static readonly DependencyProperty OverlayEnabledProperty = Reg<DisplayViewModel, bool>("OverlayEnabled", true, PropertyTypes.Save);

    public bool OverlayEnabled {
      get { return (bool)GetValue(OverlayEnabledProperty); }
      set { SetValue(OverlayEnabledProperty, value); }
    }

    public static readonly DependencyProperty OverlayIndexProperty = Reg<DisplayViewModel, int>("OverlayIndex", 0, PropertyTypes.Save);

    public int OverlayIndex {
      get { return (int)GetValue(OverlayIndexProperty); }
      set { SetValue(OverlayIndexProperty, value); }
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

    public static readonly DependencyProperty OnboardingNewUserTimeoutProperty = Reg<DisplayViewModel, int>("OnboardingNewUserTimeout", 60, PropertyTypes.Restart);

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

    public static readonly DependencyProperty AddOnIndexProperty = Reg<DisplayViewModel, int>("AddOnIndex", 0, PropertyTypes.Save);

    public int AddOnIndex {
      get { return (int)GetValue(AddOnIndexProperty); }
      set { SetValue(AddOnIndexProperty, value); }
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

    public void RefreshDisplays() {
      Log.Info("Updating Screens");
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
        Log.Info($"Screen '{vm.Name}'");
      }
    }

    protected override void AssignModel() {
      Model = AppComponent.Config.Display;
    }

    public override void UpdateRealTimeProperties() {
      base.UpdateRealTimeProperties();
      Model.LightingIntensity = (float) LEDIntensity;
    }

    public override void ApplyValuesToModel() {
      Model.OverlayEnabled = OverlayEnabled;
      if (OverlayIndex >= 0 && OverlayIndex < Displays.Count) {
        Model.OverlayDisplay = new DisplayInfo(Displays[OverlayIndex].Screen, OverlayIndex);
      }
      Model.AddOnEnabled = AddOnEnabled;
      if (AddOnIndex >= 0 && AddOnIndex < Displays.Count) {
        Model.AddOnDisplay = new DisplayInfo(Displays[AddOnIndex].Screen, AddOnIndex);
      }
      Model.LightingEnabled = LEDsEnabled;
      Model.LightingIntensity = (float) LEDIntensity;
    }

    public override void UpdateValuesFromModel() {
      RefreshDisplays();
      OverlayEnabled = Model.OverlayEnabled;
      OverlayIndex = FindClosestDisplay(Model.OverlayDisplay);
      AddOnEnabled = Model.AddOnEnabled;
      AddOnIndex = FindClosestDisplay(Model.AddOnDisplay);
      LEDsEnabled = Model.LightingEnabled;
      LEDIntensity = Model.LightingIntensity;
    }

    private int FindClosestDisplay(DisplayInfo d) {
      if (d != null) {
        for (var i = 0; i < Displays.Count; i++) {
          var sInfo = Displays[i];
          var s = sInfo.Screen;
          var b = s.Bounds;
          if (d.Width == b.Width && d.Height == b.Height && d.Primary==s.Primary) {
            return i;
          }
        }
      }
      Log.Error($"No matching display found.");
      return -1;
    }
  }
}
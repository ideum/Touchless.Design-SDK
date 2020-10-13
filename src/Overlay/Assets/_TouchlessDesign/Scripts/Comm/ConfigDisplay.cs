using System.IO;

namespace TouchlessDesign.Config {
  public class ConfigDisplay {

    public bool OverlayEnabled = true;

    public DisplayInfo OverlayDisplay = new DisplayInfo {
      Primary = true,
      Index = 0
    };

    public bool CursorEnabled = true;

    public bool NoTouchEnabled = true;

    public bool OnboardingEnabled = true;

    public float OnboardingUIScale = 1.0f;

    public float OnboardingStatusBarScale = 1.0f;

    public float OnboardingStatusBarXOffset = 0.0f;

    public int OnboardingNewUserTimeout_s = 60;

    public int OnboardingNoHandTimeout_s = 15;

    public bool Onboarding1Enabled = true;

    public bool Onboarding2Enabled = true;

    public bool Onboarding3Enabled = true;

    public bool AddOnEnabled = true;

    public DisplayInfo AddOnDisplay = new DisplayInfo {
      Primary = false,
      Index = 1
    };

    public bool LightingEnabled = true;

    public float LightingIntensity = 0.667f;

    public int FadeCandyChannel = 0;

    public int FadeCandyLightCount = 64;

    private const string Filename = "display.json";


  }
}
using System.IO;

namespace TouchlessDesign.Config {
  public class ConfigDisplay : ConfigBase<ConfigDisplay> {

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

    //public bool PedestalMode = false;

    public DisplayInfo AddOnDisplay = new DisplayInfo {
      Primary = false,
      Index = 1
    };

    public bool LightingEnabled = true;

    public float LightingIntensity = 0.667f;

    public int LightingComPort = 1; 

    //public int FadeCandyChannel = 0;

    //public int FadeCandyLightCount = 64;

    private const string Filename = "display.json";

    public static ConfigDisplay Get(string dir) {
      var path = Path.Combine(dir, Filename);
      return Factory.Get(path, () => new ConfigDisplay {
        FilePath = path
      });
    }

    public override void Apply(ConfigDisplay i) {
      OverlayEnabled = i.OverlayEnabled;
      OverlayDisplay = i.OverlayDisplay;
      AddOnEnabled = i.AddOnEnabled;
      AddOnDisplay = i.AddOnDisplay;
      LightingEnabled = i.LightingEnabled;
      LightingIntensity = i.LightingIntensity;
      LightingComPort = i.LightingComPort;
      //FadeCandyChannel = i.FadeCandyChannel;
      //FadeCandyLightCount = i.FadeCandyLightCount;
    }


  }
}
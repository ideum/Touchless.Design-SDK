using System.IO;

namespace TouchlessDesign.Config {
  public class ConfigDisplay {
    public bool OverlayEnabled = true;

    public DisplayInfo OverlayDisplay = new DisplayInfo {
      Primary = true,
      Index = 0
    };

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

    private static ConfigDisplay Defaults { get; } = new ConfigDisplay {

    };

    public static ConfigDisplay Get(string dir) {
      var path = Path.Combine(dir, Filename);
      return Factory.Get(path, () => new ConfigDisplay());
    }

    public void Save(string dir) {
      var path = Path.Combine(dir, Filename);
      Factory.Save(path, this);
    }

    public void Apply(ConfigDisplay i) {
      OverlayEnabled = i.OverlayEnabled;
      OverlayDisplay = i.OverlayDisplay;
      AddOnEnabled = i.AddOnEnabled;
      AddOnDisplay = i.AddOnDisplay;
      LightingEnabled = i.LightingEnabled;
      LightingIntensity = i.LightingIntensity;
      FadeCandyChannel = i.FadeCandyChannel;
      FadeCandyLightCount = i.FadeCandyLightCount;
    }
  }
}
using System.IO;

namespace TouchlessDesign.Config {
  public class ConfigGeneral : ConfigBase<ConfigGeneral> {

    public bool StartOnStartup = true;

    private const string Filename = "general.json";

    public static ConfigGeneral Get(string dir) {
      var path = Path.Combine(dir, Filename);
      return Factory.Get(path, () => new ConfigGeneral {
        FilePath = path
      });
    }

    public void Save(string dir) {
      var path = Path.Combine(dir, Filename);
      Factory.Save(path, this);
    }

    public override void Apply(ConfigGeneral i) {
      StartOnStartup = i.StartOnStartup;
    }
  }
}
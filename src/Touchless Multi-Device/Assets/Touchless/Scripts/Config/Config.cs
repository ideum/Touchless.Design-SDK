using System;
using System.Drawing;
using Newtonsoft.Json;

namespace TouchlessDesign.Config
{

  public interface IConfig
  {
    string FilePath { get; set; }

    void Save();

    void Reload();
  }

  public interface IConfig<in T> : IConfig where T : IConfig
  {
    void Apply(T config);
  }

  public abstract class ConfigBase<T> : IConfig<T> where T : IConfig, new()
  {

    [JsonIgnore]
    public string FilePath { get; set; }

    public void Save()
    {
      Factory.Save(FilePath, this);
    }

    public void Reload()
    {
      var t = Factory.Get<T>(FilePath);
      Apply(t);
    }

    public abstract void Apply(T config);
  }

  public class Config
  {
    public ConfigDisplay Display;
    public ConfigGeneral General;
    public ConfigInput Input;
    public ConfigNetwork Network;

    public Config(string dataDir)
    {
      Display = ConfigDisplay.Get(dataDir);
      General = ConfigGeneral.Get(dataDir);
      Input = ConfigInput.Get(dataDir);
      Network = ConfigNetwork.Get(dataDir);
    }
  }
}
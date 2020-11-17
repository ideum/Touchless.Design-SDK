using System;
using System.IO;
using Leap;
using TouchlessDesign.Config;

namespace TouchlessDesign
{
  public class LeapSettings : IConfig<LeapSettings>
  {
    private const string Filename = "leap.json";

    public ulong VerificationDuration_ms = 1000;
    public float MaxVerificationHandVelocity = 0.1f;
    public float VerificationTimeout = 3f;

    public string FilePath { get => Environment.ExpandEnvironmentVariables("%appdata%/Ideum/TouchlessDesign/"); set => Console.WriteLine("Should not set filepath"); }

    public void Save(string dir)
    {
      var path = Path.Combine(dir, Filename);
      Config.Factory.Save(path, this);
    }

    public static LeapSettings Get()
    {
      var path = Path.Combine(Environment.ExpandEnvironmentVariables("%appdata%/Ideum/TouchlessDesign/"), Filename);
      var result = Config.Factory.Get(path, Defaults);
      return result;
    }

    public static LeapSettings Defaults()
    {
      return new LeapSettings
      {
        VerificationDuration_ms = 1000,
        MaxVerificationHandVelocity = 0.1f,
        VerificationTimeout = 1.5f
      };
    }

    public void Apply(LeapSettings config)
    {
      VerificationDuration_ms = config.VerificationDuration_ms;
      MaxVerificationHandVelocity = config.MaxVerificationHandVelocity;
    }

    public void Save()
    {
      var path = Path.Combine(FilePath, Filename);
      Directory.CreateDirectory(FilePath); // Create Directory in case it doesn't exist for save file. 
      Config.Factory.Save(path, this);
    }

    public void Reload()
    {
      var tempSettings = Get();
      if (tempSettings == null)
      {
        tempSettings = LeapSettings.Defaults();
        tempSettings.Save();
      }
    }
  }
}
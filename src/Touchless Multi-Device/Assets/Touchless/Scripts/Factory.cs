using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;


namespace TouchlessDesignCore.Config
{

  public class Factory
  {

    public static T Get<T>(string path) where T : IConfig, new()
    {
      return Get(path, () => new T());
    }

    public static T Get<T>(string path, Func<T> defaults) where T : IConfig
    {
      try
      {
        if (File.Exists(path))
        {
          var raw = File.ReadAllText(path);
          // Debug.Log($"Loaded Config at {path}. Parsing...");
          var i = JsonConvert.DeserializeObject<T>(raw);
          i.FilePath = path;
          return i;
        }
      }
      catch (Exception e)
      {
        Debug.LogError($"Error loading/deserializing file at {path}. {e}");
      }

      var d = defaults();
      d.FilePath = path;
      Debug.LogWarning($"Could not load {d.GetType().Name} from {path}. Creating and using defaults.");
      try
      {
        var s = JsonConvert.SerializeObject(d, Formatting.Indented);
        File.WriteAllText(path, s);
      }
      catch (Exception e)
      {
        Debug.LogError($"Error saving default settings: {e}");
      }

      return d;
    }

    public static void Save<T>(string path, T t)
    {
      try
      {
        var s = JsonConvert.SerializeObject(t, Formatting.Indented);
        File.WriteAllText(path, s);
      }
      catch (Exception e)
      {
        Debug.LogError($"Error saving {typeof(T).Name} settings to {path}: {e}");
      }
    }
  }
}
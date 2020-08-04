using System;
using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace Ideum.Data {
  public class AppSettings {

    public const string DefaultDirectory = "%appdata%/Ideum/TouchlessDesignService";
    private const string Filename = "config.json";

    private static readonly PathInfo DefaultFilePathInfo = new PathInfo {
      Path = Filename,
      Type = PathInfo.Types.StreamingAssets
    };

    public PathInfo DataDirectory;

    public static AppSettings Get() {
      var path = DefaultFilePathInfo.GetPath();
      return ConfigFactory.Get(path, Defaults);
    }

    public static AppSettings Defaults() {
      return new AppSettings {
        DataDirectory = new PathInfo {
          Type = PathInfo.Types.FileSystem,
          Path = DefaultDirectory
        }
      };
    }

#if UNITY_EDITOR
    [MenuItem("Ideum/App/Create App Settings")]
    private static void CreateDefaultConfig() {

      try {
        var streamingAssetsPath = Application.streamingAssetsPath;
        if (!Directory.Exists(streamingAssetsPath)) {
          Directory.CreateDirectory(streamingAssetsPath);
        }
        var path = DefaultFilePathInfo.GetPath();
        if (File.Exists(path)) {
          Log.Warn($"Overwriting the contents of {path}");
        }

        var d = Defaults();
        var raw = JsonConvert.SerializeObject(d, Formatting.Indented);
        File.WriteAllText(path, raw);
      }
      catch (Exception e) {
        Log.Error(e);
      }
    }
#endif

  }
}

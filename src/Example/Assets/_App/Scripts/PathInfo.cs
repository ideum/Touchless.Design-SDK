using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

namespace Ideum.Data {
  public class PathInfo {

    public enum Types {
      FileSystem,
      StreamingAssets,
      Url
    }

    public string Path;
    
    [JsonConverter(typeof(StringEnumConverter))]
    public Types Type;
    public string GetPath() {
      switch (Type) {
        case Types.FileSystem:
          if (string.IsNullOrEmpty(Path)) {
            return string.Empty;
          }
          else {
            return Environment.ExpandEnvironmentVariables(Path);
          }
        case Types.StreamingAssets:
          if (string.IsNullOrEmpty(Path)) {
            return Application.streamingAssetsPath;
          }
          else {
            return System.IO.Path.Combine(Application.streamingAssetsPath, Path);
          }
        case Types.Url:
          return Path;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }
  }
}
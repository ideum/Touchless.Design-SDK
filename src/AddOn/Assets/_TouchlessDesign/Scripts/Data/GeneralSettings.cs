using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Ideum.Data {
  public class GeneralSettings {
    private const string Filename = "general.json";
   // public int DeviceID; Moved to network.json

    public static GeneralSettings Get(string dir) {
      var path = Path.Combine(dir, Filename);
      return ConfigFactory.Get(path, Defaults);
    }

    public static GeneralSettings Defaults() {
      return new GeneralSettings {
       // DeviceID = 0
      };
    }
  }
}
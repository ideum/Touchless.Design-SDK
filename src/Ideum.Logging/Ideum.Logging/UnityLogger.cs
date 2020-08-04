using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ideum.Logging {
  public class UnityLogger : ILogger {

    private delegate void FormatDelegate(string format, params object[] args);

    private const string
       ColorTrace = "#AAAAAA",
       ColorDebug = "#FFFFFF",
       ColorInfo = "#33FFBB",
       ColorWarn = "#FFFF66",
       ColorError = "#FF4444",
       ColorFatal = "#FF55FF",
       ColorBodyText = "#FFFFFF";

    private static Dictionary<Levels, string> _levelMap, _headerColorsMap, _bodyColorsMap;

    static UnityLogger() {
      _levelMap = new Dictionary<Levels, string>() {
        {Levels.Trace,    "[TRACE] " },
        {Levels.Debug,    "[DEBUG] " },
        {Levels.Info,     "[INFO]  " },
        {Levels.Warn,     "[WARN]  " },
        {Levels.Error,    "[ERROR] " },
        {Levels.Fatal,    "[FATAL] " }
      };
      _headerColorsMap = new Dictionary<Levels, string> {
        {Levels.Trace,    ColorTrace},
        {Levels.Debug,    ColorDebug},
        {Levels.Info,     ColorInfo },
        {Levels.Warn,     ColorWarn },
        {Levels.Error,    ColorError},
        {Levels.Fatal,    ColorFatal}
      };
      _bodyColorsMap = new Dictionary<Levels, string> {
        {Levels.Trace,    ColorTrace},
        {Levels.Debug,    ColorDebug},
        {Levels.Info,     ColorBodyText },
        {Levels.Warn,     ColorBodyText },
        {Levels.Error,    ColorBodyText },
        {Levels.Fatal,    ColorBodyText }
      };
    }

    public void Write(Levels level, object o) {
      var deltaTime = DateTime.Now - Logger.StartingTimestamp;
      var timestamp = deltaTime.TotalSeconds;
      var tag = _levelMap[level];
      var headerColor = _headerColorsMap[level]; 
      var bodyColor = _bodyColorsMap[level];

      FormatDelegate f;
      
      if ((level & Levels.Warn) != 0) {
        f = Debug.LogWarningFormat;
      }
      else if ((level & Levels.Error) != 0 || (level & Levels.Fatal) != 0) {
        f = Debug.LogErrorFormat;
      }
      else {
        f = Debug.LogFormat;
      }

      if (Application.isEditor) {
        f("<color={0}>{1}</color> <color={2}>{3}</color> \n<color={4}>»{5:000.000}s</color>", headerColor, tag, bodyColor, o, ColorTrace, timestamp);
      }
      else {
        f("{0} »{1:0000.000}s {2}", tag, timestamp, o);
      }
    }
  }
}
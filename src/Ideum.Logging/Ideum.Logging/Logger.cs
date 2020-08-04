using System;
using System.Collections.Generic;

namespace Ideum.Logging {

  [Flags]
  public enum Levels {
    None  = 0,
    Trace = 1,
    Debug = 2,
    Info  = 4,
    Warn  = 8,
    Error = 16,
    Fatal = 32,
    Bad = Warn | Error | Fatal,
    All = Trace | Debug | Info | Warn | Error | Fatal
  }

  public static class Logger {

    public static Levels EnabledLevels {
      get { return _enabledLevels; }
      set {
        Write(Levels.Info, $"Changing Log level to '{value}'.");
        _enabledLevels = value;
      }
    }

    private static Levels _enabledLevels;
    private static HashSet<ILogger> _loggers;

    public static ILogger DefaultLogger { get; }

    public static DateTime StartingTimestamp { get; }

    static Logger() {
      StartingTimestamp = DateTime.Now;
      _enabledLevels = Levels.All;
      _loggers = new HashSet<ILogger>();
      DefaultLogger = new UnityLogger();
      _loggers.Add(DefaultLogger);
    }

    public static void AddLogger(ILogger l) {
      lock (_loggers) {
        _loggers.Add(l);
      }
    }

    public static void RemoveLogger(ILogger l) {
      lock (_loggers) {
        _loggers.Remove(l);
      }
    }

    public static void Write(Levels l, object o) {

      if ((EnabledLevels & l) == 0) return;

      lock (_loggers) {
        foreach (var logger in _loggers) {
          logger.Write(l, o);
        }
      }
    }
  }
}
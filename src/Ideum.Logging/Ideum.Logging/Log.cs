
using L = Ideum.Logging;

public static class Log {
  public static void Trace(object o) { L.Logger.Write(L.Levels.Trace, o); }
  public static void Debug(object o) { L.Logger.Write(L.Levels.Debug, o); }
  public static void Info (object o) { L.Logger.Write(L.Levels.Info , o); }
  public static void Warn (object o) { L.Logger.Write(L.Levels.Warn , o); }
  public static void Error(object o) { L.Logger.Write(L.Levels.Error, o); }
  public static void Fatal(object o) { L.Logger.Write(L.Levels.Fatal, o); }
}
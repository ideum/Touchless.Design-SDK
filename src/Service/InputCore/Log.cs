using System;

namespace TouchlessDesign {
  public static class Log {

    public interface ILogger {
      void Trace(object o);
      void Debug(object o);
      void Info(object o);
      void Warn(object o);
      void Error(object o);

      void Init(string dir);
      void DeInit();
    }

    private static ILogger _instance;

    private static ILogger Instance {
      get {
        if (_instance != null) return _instance;
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
          foreach (var type in assembly.GetTypes()) {
            if (!typeof(ILogger).IsAssignableFrom(type) || !type.IsClass || type.IsAbstract) continue;
            try {
              var instance = Activator.CreateInstance(type);
              _instance = (ILogger) instance;
              return _instance;
            }
            catch (Exception) {
              return null;
            }
          }
        }
        return null;
      }
    }
    
    public static void Trace(object o) {
      Instance?.Trace(o);
    }

    public static void Debug(object o) {
      Instance?.Debug(o);
    }

    public static void Info(object o) {
      Instance?.Info(o);
    }

    public static void Warn(object o) {
      Instance?.Warn(o);
    }

    public static void Error(object o) {
      Instance?.Error(o);
    }

    public static void Init(string dir) {
      Instance?.Init(dir);
    }

    public static void DeInit() {
      Instance?.DeInit();
    }
  }
}
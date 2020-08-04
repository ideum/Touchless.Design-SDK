using System;

namespace TouchlessDesign {
  
  public static class ConfigFactory {

    public interface IConfigFactory {
      T Get<T>(string path) where T : new();
      T Get<T>(string path, Func<T> defaults);
      void Save<T>(string path, T t);
    }

    private static IConfigFactory _instance;

    public static T Get<T>(string path) where T: new() {
      if (!InsureInstance()) {
        return default;
      }
      return _instance.Get<T>(path);
    }

    public static T Get<T>(string path, Func<T> defaults) {
      if (!InsureInstance()) {
        return defaults();
      }
      return _instance.Get(path, defaults);
    }

    public static void Save<T>(string path, T t) {
      if (!InsureInstance()) {
        return;
      }
      _instance.Save(path, t);
    }

    private static bool InsureInstance() {
      if (_instance != null) return true;
      foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
        foreach (var type in assembly.GetTypes()) {
          if (!typeof(IConfigFactory).IsAssignableFrom(type) || !type.IsClass || type.IsAbstract) continue;
          try {
            var instance = Activator.CreateInstance(type);
            _instance = (IConfigFactory) instance;
            return true;
          }
          catch (Exception) {
            return false;
          }
        }
      }
      return false;
    }
  }
}
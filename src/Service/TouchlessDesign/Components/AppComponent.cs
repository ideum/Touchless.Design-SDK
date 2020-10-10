using System;
using System.Collections.Generic;

namespace TouchlessDesign.Components {

  public abstract class AppComponent {

    public static Config.Config Config { get; set; }

    public static AppComponent[] Components { get; set; }
    
    public static Ui.Ui Ui { get; set; }
    
    public static Input.Input Input { get; set; }
    
    public static Ipc.Ipc Ipc { get; set; }

    public static Lighting.Lighting Lighting { get; set; }

    public event Action OnStarted;
    public event Action OnStopped;

    public bool IsStarted { get; private set; }
    public string DataDir { get; private set; }


    public void Start() {
      if (IsStarted) return;
      IsStarted = true;
      DoStart();
      OnStarted?.Invoke();
    }

    public void Stop() {
      if (!IsStarted) return;
      IsStarted = false;
      DoStop();
      OnStopped?.Invoke();
    }

    protected abstract void DoStart();

    protected abstract void DoStop();

    private static bool _isLoaded = false;
    private static readonly List<Action> OnLoadedHandlers = new List<Action>();

    public static void OnLoaded(Action onLoaded) {
      if (_isLoaded) {
        onLoaded?.Invoke();
        return;
      }
      OnLoadedHandlers.Add(onLoaded);
    }

    public static void InitializeComponents(App app) {
      Config = new Config.Config(app.DataDir);
      Ui = new Ui.Ui(app);
      Input = new Input.Input();
      Lighting = new Lighting.Lighting();
      Ipc = new Ipc.Ipc();
      Components = new AppComponent[] {
        Ui,
        Input,
        Lighting,
        Ipc
      };
      foreach (var c in Components) {
        try {
          c.DataDir = app.DataDir;
          c.Start();
        }
        catch (Exception e) {
          Log.Error($"Error starting AppComponent of type {c.GetType().Name}: {e}");
        }
      }
      _isLoaded = true;
      foreach (var onLoadedHandler in OnLoadedHandlers) {
        onLoadedHandler?.Invoke();
      }
      OnLoadedHandlers.Clear();
    }


    public static void DeInitializeComponents() {
      foreach (var c in Components) {
        try {
          c.Stop();
        }
        catch (Exception e) {
          Log.Error(e);
        }
      }
      Components = null;
      Ui = null;
      Input = null;
      Lighting = null;
      Ipc = null;
    }
  }
}
using System;
using System.Collections.Generic;
using UnityEngine;

namespace TouchlessDesign.Components
{

  public abstract class AppComponent : MonoBehaviour
  {
    public static TouchlessApp TouchlessApp => TouchlessApp.Instance;

    public static Config.Config Config { get; set; }

    public static AppComponent[] Components { get; set; }

    public static TouchlessInput Input { get; set; }

    public event Action OnStarted;
    public event Action OnStopped;

    public bool IsStarted { get; private set; }
    public string DataDir { get; private set; }


    public void Init()
    {
      if (IsStarted) return;
      IsStarted = true;
      DoStart();
      OnStarted?.Invoke();
    }

    public void Stop()
    {
      if (!IsStarted) return;
      IsStarted = false;
      DoStop();
      OnStopped?.Invoke();
    }

    protected abstract void DoStart();

    protected abstract void DoStop();

    private static bool _isLoaded = false;
    private static readonly List<Action> OnLoadedHandlers = new List<Action>();

    public static void OnLoaded(Action onLoaded)
    {
      if (_isLoaded)
      {
        onLoaded?.Invoke();
        return;
      }
      OnLoadedHandlers.Add(onLoaded);
    }

    public static void InitializeComponents(TouchlessApp app)
    {
      Config = new Config.Config(app.DataDir);
      Input = TouchlessApp.Instance.TouchlessInput;
      Components = new AppComponent[] {
        Input
      };
      foreach (var c in Components)
      {
        try
        {
          c.DataDir = app.DataDir;
          c.Init();
        }
        catch (Exception e)
        {
          Debug.LogError($"Error starting AppComponent of type {c.GetType().Name}: {e}");
        }
      }
      _isLoaded = true;
      foreach (var onLoadedHandler in OnLoadedHandlers)
      {
        onLoadedHandler?.Invoke();
      }
      OnLoadedHandlers.Clear();
    }

    public static void DeInitializeComponents()
    {
      foreach (var c in Components)
      {
        try
        {
          c.Stop();
        }
        catch (Exception e)
        {
          Debug.LogError(e);
        }
      }
      Components = null;
      Input = null;
    }
  }
}
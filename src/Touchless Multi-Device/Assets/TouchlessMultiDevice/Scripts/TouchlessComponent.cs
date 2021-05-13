using System;
using System.Collections.Generic;
using UnityEngine;

namespace TouchlessDesignCore.Components
{

  public abstract class TouchlessComponent : MonoBehaviour
  {
    public static Config.Config Config { get; set; }

    public static TouchlessComponent[] Components { get; set; }

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

    public static void InitializeInputComponent(TouchlessDesign app)
    {
      Config = new Config.Config(app.DataDir);
      Input = TouchlessDesign.Instance.TouchlessInput;
      try
      {
        Input.DataDir = app.DataDir;
        Input.Init();
      }
      catch (Exception e)
      {
        Debug.LogError($"Error starting Touchless Input Component: {e}");
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
      Input.Stop();
      Input = null;
    }
  }
}
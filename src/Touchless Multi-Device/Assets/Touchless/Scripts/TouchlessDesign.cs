using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using TouchlessDesignCore.Components;
using TouchlessDesignCore.Components.Input;
using TouchlessDesignCore.Components.Ipc;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

namespace TouchlessDesignCore
{
  public class TouchlessDesign : MonoBehaviour
  {
    public Canvas Canvas;
    public GraphicRaycaster GraphicRaycaster;
    public TouchlessInput TouchlessInput;
    public bool DebugHands;
    public bool AddTouchlessInputModule;

    public string DataDir
    {
      get
      {
        if (string.IsNullOrEmpty(_dataDir))
        {
          _dataDir = Path.Combine(Application.streamingAssetsPath, "TouchlessDesign");
        }
        return _dataDir;
      }
    }

    public static TouchlessDesign Instance;
    public TouchlessUser TouchlessUserPrefab;
    public Dictionary<string, TouchlessUser> Users = new Dictionary<string, TouchlessUser>();
    public event Action OnStarted;
    public event Action OnStopped;

    private string _dataDir;

    public void Awake()
    {
      if (Instance == null)
      {
        Instance = this;
        if(AddTouchlessInputModule)
        {
          EventSystem e = FindObjectOfType<EventSystem>();
          if(!e.TryGetComponent<TouchlessInputModule>(out var module))
          {
            e.gameObject.AddComponent<TouchlessInputModule>();
          }
        }
      }
      else
      {
        Destroy(gameObject);
      }
      _syncThreadId = Thread.CurrentThread.ManagedThreadId;
      _isSyncSet = true;
    }

    public void Start()
    {
      if (!Directory.Exists(DataDir))
      {
        try
        {
          Directory.CreateDirectory(DataDir);
        }
        catch (Exception ex)
        {
          Debug.LogError(ex.ToString());
        }
      }

      TouchlessComponent.InitializeInputComponent(this);

      foreach (TouchlessUserInfo info in TouchlessComponent.Config.Input.Players)
      {
        TouchlessUser user = Instantiate(TouchlessUserPrefab, transform);
        user.SetUserData(info);
        Users.Add(user.UserInfo.IpAddress, user);
      }

      OnStarted?.Invoke();
    }

    private void Update()
    {
      if (EnableSync)
      {
        InvokeSync();
      }
    }

    public void LogIt(string message)
    {
      Debug.Log(message);
    }

    public void OnDestroy()
    {
      TouchlessComponent.DeInitializeComponents();
      OnStopped?.Invoke();
    }

    internal void HandleUserMessage(Msg msg, IPEndPoint endpoint)
    {
      // Debug.Log("Receiving message from endpoint " + endpoint.Address);
      Users.TryGetValue(endpoint.Address.ToString(), out var TargetUser);
      if (TargetUser == null)
      {
        Debug.LogError("Message received from user that isn't in the players list");
      }
      else
      {
        TargetUser.DataMessageReceived(msg);
      }
    }

    #region Sync
    /// <summary>
    /// Toggles the <see cref="Sync"/> functionality. If false, the sync capability will be disabled.
    /// </summary>
    public static bool EnableSync = true;

    private static bool _isSyncSet = false;
    private static volatile int _syncThreadId;
    private static object _syncLock = new object();
    private static Queue<Action> _syncQueue = new Queue<Action>();

    /// <summary>
    /// Queues an Action to be executed on Unity's main thread. 
    /// If this method is called from the assigned main thread, the method will be executed immediately. 
    /// Otherwise, the method will be executed in the next Update loop of the App instance.
    /// 
    /// This feature is designed specifically for non-MonoBehaviour classes, where instances may need to access / change unity-thread protected properties
    /// 
    /// This method is thread-safe.
    /// </summary>
    /// <param name="a">The action to execute</param>
    public static void Sync(Action a)
    {
      if (!EnableSync)
      {
        Debug.LogError("Attempting to queue an action for Sync when EnableSync is false. This action will not be queued or executed.");
        return;
      }
      if (a == null) return;
      if (_isSyncSet && Thread.CurrentThread.ManagedThreadId == _syncThreadId)
      {
        a();
      }
      else
      {
        lock (_syncLock)
        {
          _syncQueue.Enqueue(a);
        }
      }
    }

    /// <summary>
    /// Called from the App Update method. Calls all items in the _syncQueue as the queue is emptied.
    /// </summary>
    private static void InvokeSync()
    {
      if (!EnableSync) return;
      lock (_syncLock)
      {
        while (_syncQueue.Count > 0)
        {
          var a = _syncQueue.Dequeue();
          try
          {
            a();
          }
          catch (Exception e)
          {
            Debug.LogException(e);
            throw e;
          }
        }
      }
    }

    public bool TryGetUserFromEventData(PointerEventData eventData, out TouchlessUser user)
    {
      user = Users.Values.FirstOrDefault((u) => u.PointerEventData == eventData);
      return user != null;
    }
    #endregion

  }
}
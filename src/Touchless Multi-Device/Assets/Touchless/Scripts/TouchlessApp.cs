using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using TouchlessDesign.Components;
using TouchlessDesign.Components.Ipc;
using UnityEngine;
using UnityEngine.UI;

namespace TouchlessDesign
{
  public class TouchlessApp : MonoBehaviour
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

    public static TouchlessApp Instance;
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

      AppComponent.InitializeComponents(this);

      foreach (TouchlessUserInfo info in AppComponent.Config.Input.Players)
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
      AppComponent.DeInitializeComponents();
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
    #endregion

    /// <summary>
    /// Arguments dispatched with TouchManager events.
    /// </summary>
    public class PointerEventArgs : EventArgs
    {
      /// <summary>
      /// Gets list of pointers participating in the event.
      /// </summary>
      /// <value>List of pointers added, changed or removed this frame.</value>
      public IList<TouchlessUser> Pointers { get; private set; }

      private static PointerEventArgs instance;

      /// <summary>
      /// Initializes a new instance of the <see cref="PointerEventArgs"/> class.
      /// </summary>
      private PointerEventArgs() { }

      /// <summary>
      /// Returns cached instance of EventArgs.
      /// This cached EventArgs is reused throughout the library not to alocate new ones on every call.
      /// </summary>
      /// <param name="pointers">A list of pointers for event.</param>
      /// <returns>Cached EventArgs object.</returns>
      public static PointerEventArgs GetCachedEventArgs(IList<TouchlessUser> pointers)
      {
        if (instance == null) instance = new PointerEventArgs();
        instance.Pointers = pointers;
        return instance;
      }
    }
  }
}
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Ideum.Data;

namespace Ideum {
  public static class TouchlessDesign {

    #region General

    private const string DefaultDirectory = "%appdata%/Ideum/TouchlessDesignService";

    private static bool _isInitialized;

    public static void Initialize(string dataDir = null) {
      if (_isInitialized) return;
      _isInitialized = true;
      if (string.IsNullOrEmpty(dataDir)) {
        dataDir = DefaultDirectory;
      }
      dataDir = Environment.ExpandEnvironmentVariables(dataDir);
      MonoBehaviourHooks.InitializeHooks();
      InitializeNetworking(dataDir);
    }

    public static void DeInitialize() {
      if (!_isInitialized) return;
      _isInitialized = false;
      DeInitializeNetworking();
      MonoBehaviourHooks.DeInitializeHooks();
    }

    #endregion

    #region Networking

    private static ClientManager _connectionManager;
    private static NetworkSettings _networkSettings;

    public static bool IsConnected { get; private set; }

    private static void InitializeNetworking(string dataDir = null) {
      _networkSettings = NetworkSettings.Get(dataDir);
      _connectionManager = new ClientManager(_networkSettings);
      _connectionManager.OnConnected += OnConnected;
      _connectionManager.OnDisconnected += OnDisconnected;
      _connectionManager.OnMessage += OnMessageReceived;
    }

    private static void DeInitializeNetworking() {
      _connectionManager?.Dispose();
      _connectionManager = null;
    }

    private static void OnConnected() {
      IsConnected = true;
      Connected?.Invoke();
    }

    private static void OnDisconnected() {
      IsConnected = false;
      Disconnected?.Invoke();
    }

    private static void OnMessageReceived(Msg msg) {
      switch (msg.Type) {
        case Msg.Types.None:
          break;
        case Msg.Types.Hover:
          break;
        case Msg.Types.HoverQuery:
          if (msg.ContainsIncomingClientSideData) {
            HoverQueries.SyncClearInvoke(msg);
          }
          break;
        case Msg.Types.Quit:
          break;
        case Msg.Types.Options:
          break;
        case Msg.Types.DimensionsQuery:
          if (msg.ContainsIncomingClientSideData) {
            DimsQueries.SyncClearInvoke(msg);
          }
          break;
        case Msg.Types.Position:
          break;
        case Msg.Types.Click:
          break;
        case Msg.Types.ClickQuery:
          if (msg.ContainsIncomingClientSideData) {
            ClickQueries.SyncClearInvoke(msg);
          }
          break;
        case Msg.Types.ClickAndHoverQuery:
          if (msg.ContainsIncomingClientSideData) {
            ClickAndHoverQueries.SyncClearInvoke(msg);
          }
          break;
        case Msg.Types.Ping:
          break;
        case Msg.Types.NoTouch:
          break;
        case Msg.Types.NoTouchQuery:
          if (msg.ContainsIncomingClientSideData) {
            NoTouchQueries.SyncClearInvoke(msg);
          }
          break;
        case Msg.Types.AddOnQuery:
          if (msg.ContainsIncomingClientSideData) {
            AddOnQueries.SyncClearInvoke(msg);
          }
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    private static void SyncClearInvoke(this Msg.Callback c, Msg msg) {
      Sync(() => { c.ClearInvoke(msg); });
    }

    #endregion

    #region API

    public static event Action Connected, Disconnected;

    private static readonly Msg.Callback HoverQueries = new Msg.Callback(Msg.Types.HoverQuery);
    private static readonly Msg.Callback DimsQueries = new Msg.Callback(Msg.Types.DimensionsQuery);
    private static readonly Msg.Callback ClickQueries = new Msg.Callback(Msg.Types.ClickQuery);
    private static readonly Msg.Callback ClickAndHoverQueries = new Msg.Callback(Msg.Types.ClickAndHoverQuery);
    private static readonly Msg.Callback NoTouchQueries = new Msg.Callback(Msg.Types.NoTouchQuery);
    private static readonly Msg.Callback AddOnQueries = new Msg.Callback(Msg.Types.AddOnQuery);

    public static void QueryDimensions(Msg.QueryDimsDelegate callback) {
      DimsQueries.Add(callback);
      _connectionManager.Send(Msg.Factories.DimensionsQuery());
    }

    /// <summary>
    /// Sets the hover-state
    /// </summary>
    /// <param name="hover"></param>
    public static void SetHoverState(HoverStates hover) {
      _connectionManager.Send(Msg.Factories.Hover(hover));
    }

    /// <summary>
    /// Sets the hover-state of the cursor. 
    /// </summary>
    /// <param name="isHovering">If isHovering is true, the hover-state will be set to HoverStates.Click. If false, the hover-state will be set to HoverStates.None</param>
    public static void SetHoverState(bool isHovering) {
      _connectionManager.Send(Msg.Factories.Hover(isHovering ? HoverStates.Click : HoverStates.None));
    }

    public static void QueryHoverState(Msg.BoolDelegate callback) {
      HoverQueries.Add(callback);
      _connectionManager.Send(Msg.Factories.HoverQuery());
    }

    public static void QueryHoverState(Msg.HoverStateDelegate callback) {
      HoverQueries.Add(callback);
      _connectionManager.Send(Msg.Factories.HoverQuery());
    }

    public static void SetPosition(int x, int y) {
      _connectionManager.Send(Msg.Factories.Position(x, y));
    }

    public static void SetClickState(bool isDown) {
      _connectionManager.Send(Msg.Factories.Click(isDown));
    }

    public static void SetNoTouchState(bool value) {
      _connectionManager.Send(Msg.Factories.NoTouch(value));
    }

    public static void QueryClickState(Msg.BoolDelegate callback) {
      ClickQueries.Add(callback);
      _connectionManager.Send(Msg.Factories.ClickQuery());
    }

    public static void QueryClickAndHoverState(Msg.ClickAndHoverDelegate callback) {
      ClickAndHoverQueries.Add(callback);
      _connectionManager.Send(Msg.Factories.ClickAndHoverQuery());
    }

    public static void QueryClickAndHoverState(Msg.ClickAndHoverBoolDelegate callback) {
      ClickAndHoverQueries.Add(callback);
      _connectionManager.Send(Msg.Factories.ClickAndHoverQuery());
    }

    public static void QueryNoTouchState(Msg.NoTouchDelegate callback) {
      NoTouchQueries.Add(callback);
      _connectionManager.Send(Msg.Factories.NoTouchQuery());
    }

    public static void QueryAddOn(Msg.AddOnQueryDelegate callback) {
      AddOnQueries.Add(callback);
      _connectionManager.Send(Msg.Factories.AddOnQuery());
    }

    #endregion

    #region Sync

    private static readonly object SyncLock = new object();
    private static readonly Queue<Action> SyncQueue = new Queue<Action>();
    private static int? _unityThreadId;
    private static volatile bool _hasSyncItems = false;

    /// <summary>
    /// Queues an object to be executed on the main thread.
    /// </summary>
    /// <param name="action"></param>
    public static void Sync(Action action) {
      var doExecute = false;
      lock (SyncLock) {
        if (_unityThreadId.HasValue && _unityThreadId.Value == Thread.CurrentThread.ManagedThreadId) {
          doExecute = true;
        }
        else {
          SyncQueue.Enqueue(action);
          _hasSyncItems = true;
        }
      }

      if (!doExecute) return;

      ExecuteSyncMethod(action);
    }

    private static void ExecuteSyncMethod(Action action) {
      try {
        action();
      }
      catch (Exception e) {
        Log.Warn("Caught exception while executing action in Sync");
        Log.Error(e);
      }
    }

    #endregion

    #region MonoBehaviour Hooks

    private class MonoBehaviourHooks : MonoBehaviour {

      private static MonoBehaviourHooks _instance;

      public static void InitializeHooks() {
        var go = new GameObject("[Touchless.Design Hooks]");
        DontDestroyOnLoad(go);
        _instance = go.AddComponent<MonoBehaviourHooks>();
        lock (SyncLock) {
          _unityThreadId = Thread.CurrentThread.ManagedThreadId;
        }
      }

      public static void DeInitializeHooks() {
        if (_instance == null) return;
        Destroy(_instance.gameObject);
        _instance = null;
      }

      private readonly List<Action> _syncBuffer = new List<Action>();

      void OnApplicationQuit() {
        DeInitialize();
      }

      void Update() {
        if (!_hasSyncItems) return;
        lock (SyncLock) {
          _syncBuffer.AddRange(SyncQueue);
          SyncQueue.Clear();
        }

        foreach (var a in _syncBuffer) {
          ExecuteSyncMethod(a);
        }
        _syncBuffer.Clear();
      }
    }

    #endregion
  }
}
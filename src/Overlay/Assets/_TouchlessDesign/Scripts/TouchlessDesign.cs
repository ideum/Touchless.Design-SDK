using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Ideum.Data;

namespace Ideum {
  public static class TouchlessDesign {

    #region General

    public static event Action<Msg> SettingChanged;
    public static Dictionary<int, TouchlessUser> Users;
    private static int _mouseDriverId;
    public static event Action<TouchlessUser> UserAdded;
    public static event Action<TouchlessUser> UserRemoved;

    private const string DefaultDirectory = "%appdata%/Ideum/TouchlessDesign";

    private static bool _isInitialized;

    public static void Initialize(string dataDir = null) {
      if (_isInitialized) return;
      _isInitialized = true;
      if (string.IsNullOrEmpty(dataDir)) {
        dataDir = DefaultDirectory;
      }
      Users = new Dictionary<int, TouchlessUser>();
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
    private static GeneralSettings _generalSettings;

    public static bool IsConnected { get; private set; }

    private static void InitializeNetworking(string dataDir = null) {
      _networkSettings = NetworkSettings.Get(dataDir);
      _generalSettings = GeneralSettings.Get(dataDir);
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

    public static TouchlessUser GetTouchlessUserFromPointerID(int pointerId) {
      return pointerId == -1 ? Users[_mouseDriverId] : Users[pointerId];
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
        case Msg.Types.SubscribeToDisplaySettings:
          SettingChanged?.Invoke(msg);
          break;
        case Msg.Types.DisplaySettingsChanged:
          SettingChanged.Invoke(msg);
          break;
        case Msg.Types.HandCountQuery:
          HandQueries.SyncClearInvoke(msg);
          break;
        case Msg.Types.OnboardingQuery:
          OnboardingQueries.SyncClearInvoke(msg);
          break;
        case Msg.Types.SubscribeToUserUpdates:
          break;
        case Msg.Types.UserAdded:
          Sync(() => AddUser(msg.DeviceId));
          break;
        case Msg.Types.UserRemoved:
          Sync(() => RemoveUser(msg.DeviceId));
          break;
        case Msg.Types.UserUpdate:
          Sync(() => { Msg message = msg; int msgId = message.DeviceId; UpdateUser(msgId, message); });
          break;
        case Msg.Types.UsersQuery:
          Sync(() => {
            // Debug.Log($"Users: {msg.Users}");
            if (msg.TouchlesUserInfo != null) {
              foreach (var user in msg.TouchlesUserInfo) {
                AddUser(user.DeviceId);
                UpdateUser(user.DeviceId, user);
              }
              // Remove all users that no longer exist.
              foreach (var deviceId in msg.Users) {
                if (!Users.ContainsKey(deviceId)) {
                  RemoveUser(deviceId);
                }
              }
            }
            _mouseDriverId = msg.MouseDriverId;
          });
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    private static void AddUser(int deviceId) {
      if (!Users.ContainsKey(deviceId)) {
        TouchlessUser user = new TouchlessUser();
        user.DeviceId = deviceId;
        Users.Add(deviceId, user);
        Log.Info($"User {deviceId} connected.");
        UserAdded?.Invoke(user);
      }
    }

    private static void RemoveUser(int deviceId) {
      bool canremove = Users.TryGetValue(deviceId, out TouchlessUser userToRemove);
      if (canremove) {
        Users.Remove(deviceId);
        UserRemoved?.Invoke(userToRemove);
        Log.Info($"User {deviceId} has disconnected!");
      }
    }

    private static void UpdateUser(int deviceId, Msg msg) {
      bool userFound = Users.TryGetValue(deviceId, out TouchlessUser user);
      if(!userFound) {
        Log.Error($"Device ID for user update not found");
      }
      else {
        if(deviceId == 0) {
          Debug.Log($"Updating user {deviceId}");
        }
        user.Update(msg);
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
    private static readonly Msg.Callback HandQueries = new Msg.Callback(Msg.Types.HandCountQuery);
    private static readonly Msg.Callback OnboardingQueries = new Msg.Callback(Msg.Types.OnboardingQuery);
    private static readonly Msg.Callback StateUserQueries = new Msg.Callback(Msg.Types.QueryStateUserId);
    private static readonly Msg.Callback UsersQueries = new Msg.Callback(Msg.Types.UsersQuery);

    public static void QueryDimensions(Msg.QueryDimsDelegate callback) {
      DimsQueries.Add(callback);
      Msg msg = Msg.Factories.DimensionsQuery();
      msg.DeviceId = _generalSettings.DeviceID;
      _connectionManager.Send(msg);
    }

    /// <summary>
    /// Sets the hover-state
    /// </summary>
    /// <param name="hover"></param>
    public static void SetHoverState(int deviceId, HoverStates hover, int priority = 0) {
      Msg msg = Msg.Factories.Hover(hover);
      msg.DeviceId = deviceId;
      msg.Priority = priority;
      _connectionManager.Send(msg);
    }

    /// <summary>
    /// Sets the hover-state of the cursor. 
    /// </summary>
    /// <param name="isHovering">If isHovering is true, the hover-state will be set to HoverStates.Click. If false, the hover-state will be set to HoverStates.None</param>
    public static void SetHoverState(bool isHovering, int priority = 0) {
      Msg msg = Msg.Factories.Hover(isHovering ? HoverStates.Click : HoverStates.None);
      msg.Priority = priority;
      msg.DeviceId = _generalSettings.DeviceID;
      _connectionManager.Send(msg);
    }

    public static void QueryHoverState(Msg.BoolDelegate callback) {
      HoverQueries.Add(callback);
      var msg = Msg.Factories.HoverQuery();
      msg.DeviceId = _generalSettings.DeviceID;
      _connectionManager.Send(msg);
    }

    public static void QueryHoverState(Msg.HoverStateDelegate callback) {
      HoverQueries.Add(callback);
      var msg = Msg.Factories.HoverQuery();
      msg.DeviceId = _generalSettings.DeviceID;
      _connectionManager.Send(msg);
    }

    public static void SetPosition(int x, int y, int priority = 0) {
      Msg msg = Msg.Factories.Position(x, y);
      msg.Priority = priority;
      msg.DeviceId = _generalSettings.DeviceID;
      _connectionManager.Send(msg);
    }

    public static void SetClickState(bool isDown, int priority = 0) {
      Msg msg = Msg.Factories.Click(isDown);
      msg.Priority = priority;
      msg.DeviceId = _generalSettings.DeviceID;
      _connectionManager.Send(msg);
    }

    public static void SetNoTouchState(bool value, int priority = 0) {
      Msg msg = Msg.Factories.NoTouch(value);
      msg.Priority = priority;
      msg.DeviceId = _generalSettings.DeviceID;
      _connectionManager.Send(msg);
    }

    public static void QueryClickState(Msg.BoolDelegate callback) {
      ClickQueries.Add(callback);
      Msg msg = Msg.Factories.ClickQuery();
      msg.DeviceId = _generalSettings.DeviceID;
      _connectionManager.Send(msg);
    }

    public static void QueryClickAndHoverState(Msg.ClickAndHoverDelegate callback) {
      ClickAndHoverQueries.Add(callback);
      Msg msg = Msg.Factories.ClickAndHoverQuery();
      msg.DeviceId = _generalSettings.DeviceID;
      _connectionManager.Send(msg);
    }

    public static void QueryClickAndHoverState(Msg.ClickAndHoverBoolDelegate callback) {
      ClickAndHoverQueries.Add(callback);
      Msg msg = Msg.Factories.ClickAndHoverQuery();
      msg.DeviceId = _generalSettings.DeviceID;
      _connectionManager.Send(msg);
    }

    public static void QueryNoTouchState(Msg.NoTouchDelegate callback) {
      NoTouchQueries.Add(callback);
      _connectionManager.Send(Msg.Factories.NoTouchQuery());
    }

    public static void QueryAddOn(Msg.AddOnQueryDelegate callback) {
      AddOnQueries.Add(callback);
      Msg msg = Msg.Factories.AddOnQuery();
      msg.DeviceId = _generalSettings.DeviceID;
      _connectionManager.Send(msg);
    }

    public static void QueryHandCount(Msg.HandCountQueryDelegate callback) {
      HandQueries.Add(callback);
      Msg msg = Msg.Factories.HandCountQuery();
      msg.DeviceId = _generalSettings.DeviceID;
      _connectionManager.Send(msg);
    }

    public static void SubscribeToDisplayConfig() {
      _connectionManager.Send(Msg.Factories.SubscribeMessage());
    }

    public static void SubscribeToUserUpdates() {
      _connectionManager.Send(Msg.Factories.UserSubscribeMessage());
    }

    public static void SetPriority(int priority) {
      _connectionManager.Send(Msg.Factories.PriorityMessage(priority));
    }

    public static void SetOnboarding(bool onboarding) {
      _connectionManager.Send(Msg.Factories.SetOnboardingMessage(onboarding));
    }

    public static void QueryOnboarding(Msg.OnboardingQueryDelegate callback) {
      OnboardingQueries.Add(callback);
      _connectionManager.Send(Msg.Factories.OnboardingQueryMessage());
    }

    public static void QueryStateUserId(Msg.StateUserQueryDelegate callback) {
      StateUserQueries.Add(callback);
      _connectionManager.Send(Msg.Factories.QueryStateUserId());
    }

    public static void QueryUsers() {
      _connectionManager.Send(Msg.Factories.UsersQuery());
    }

    public static int GetMouseDriverId() {
      return _mouseDriverId;
    }

    public static TouchlessUser GetUserFromPointerId(int pointerId) {
      if(pointerId == -1) {
        if(GetMouseDriverId() == -1) {
          return null;
        }
        return Users[GetMouseDriverId()];
      }
      return Users[pointerId];
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
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using TouchlessDesign.Components.Input;
using TouchlessDesign.Config;

namespace TouchlessDesign.Components.Ipc {
  public class Msg {

    #region Properties

    #region Type

    public enum Types {
      None,
      Hover,
      HoverQuery,
      Quit,
      Options,
      DimensionsQuery,
      Position,
      Click,
      ClickQuery,
      ClickAndHoverQuery,
      Ping,
      NoTouch,
      NoTouchQuery,
      AddOnQuery,
      SubscribeToDisplaySettings,
      DisplaySettingsChanged,
      HandCountQuery,
      Hands,
      SetPriority,
      OnboardingQuery,
      SetOnboarding,
      OverlayCursorVisibilityQuery,
      SetOverlayCursorVisible,
      RegisterRemoteClient,
      UsersQuery,
      SubscribeToUserUpdates,
      UserUpdate,
      UserAdded,
      UserRemoved,
      QueryStateUserId,
    }

    [JsonProperty("T")]
    private int _serializedType;

    [JsonIgnore]
    public Types Type {
      get { return (Types)_serializedType; }
      set { _serializedType = (int)value; }
    }

    #endregion

    #region Hover States

    [JsonProperty("S")]
    private int? _serializedHoverState;

    [JsonIgnore]
    public HoverStates HoverState {
      get { return _serializedHoverState.HasValue ? (HoverStates)_serializedHoverState : HoverStates.None; }
      set { _serializedHoverState = (int)value; }
    }

    #endregion

    [JsonProperty("Priority")]
    public int Priority = 0;

    [JsonProperty("B")]
    public bool? Bool = null;

    [JsonProperty("C")]
    public bool? Bool2 = null;

    [JsonProperty("X")]
    public int? X = null;

    [JsonProperty("Y")]
    public int? Y = null;

    [JsonProperty("W")]
    public int? W = null;

    [JsonProperty("H")]
    public int? H = null;

    [JsonProperty("String")]
    public string S = null;

    [JsonProperty("F1")]
    public float? F1 = null;

    [JsonProperty("Config")]
    public ConfigDisplay Config;

    [JsonProperty("Hands")]
    public Hand[] Hands;

    [JsonProperty("HandCount")]
    public int HandCount;

    [JsonProperty("IsClicking")]
    public bool IsClicking;

    [JsonProperty("UserIds")]
    public int[] UserIds;

    [JsonProperty("DeviceId")]
    public int DeviceId;

    [JsonProperty("TouchlessUserInfo")]
    public Msg[] TouchlessUserInfo;

    [JsonProperty("MouseDriverId")]
    public int MouseDriverId = -1;

    #endregion

    #region Constructors

    public Msg() {
    }

    public Msg(Types type, int deviceId = 0, bool? boolValue = null, int? x = null, int? y = null, int? w = null, int? h = null, string s = null, float? f1 = null, int priority = 0) {
      Type = type;
      Bool = boolValue;
      DeviceId = deviceId;
      X = x;
      Y = y;
      W = w;
      H = h;
      S = s;
      F1 = f1;
      Priority = priority;
    }

    #endregion

    #region Serialization

    private static readonly JsonSerializerSettings SerializationSettings = new JsonSerializerSettings {
      NullValueHandling = NullValueHandling.Ignore
    };

    public string Serialize() {
      return JsonConvert.SerializeObject(this, SerializationSettings);
    }

    public static bool TryDeserialize(string s, out Msg msg) {
      try {
        msg = JsonConvert.DeserializeObject<Msg>(s, SerializationSettings);
        return true;
      }
      catch (Exception) {
        msg = null;
        return false;
      }
    }

    public override string ToString() {
      return Serialize();
    }

    #endregion

    #region Factory Methods

    public static class Factories {
      public static Msg Hover(HoverStates hover) {
        return new Msg {
          Type = Types.Hover,
          HoverState = hover
        };
      }

      public static Msg HoverQuery() {
        return new Msg {
          Type = Types.HoverQuery
        };
      }

      public static Msg HoverQuery(int deviceId, HoverStates hover) {
        return new Msg {
          Type = Types.HoverQuery,
          HoverState = hover,
          DeviceId = deviceId
        };
      }

      public static Msg Quit() {
        return new Msg { Type = Types.Quit };
      }

      public static Msg Options(bool isShowing) {
        return new Msg {
          Type = Types.Options,
          Bool = isShowing
        };
      }

      public static Msg DimensionsQuery() {
        return new Msg { Type = Types.DimensionsQuery };
      }

      public static Msg DimensionsQuery(int x, int y, int w, int h) {
        return new Msg {
          Type = Types.DimensionsQuery,
          X = x,
          Y = y,
          W = w,
          H = h
        };
      }

      public static Msg Position(int x, int y) {
        return new Msg {
          Type = Types.Position,
          X = x,
          Y = y
        };
      }

      public static Msg Click(bool isDown) {
        return new Msg() {
          Type = Types.Click,
          Bool = isDown
        };
      }

      public static Msg ClickQuery() {
        return new Msg {
          Type = Types.ClickQuery
        };
      }

      public static Msg ClickQuery(bool value) {
        return new Msg {
          Type = Types.ClickQuery,
          Bool = value
        };
      }

      public static Msg ClickAndHoverQuery(int deviceId) {
        return new Msg { Type = Types.ClickAndHoverQuery, DeviceId = deviceId };
      }

      public static Msg ClickAndHoverQuery(int deviceId, bool click, HoverStates hover) {
        return new Msg {
          Type = Types.ClickAndHoverQuery,
          Bool = click,
          HoverState = hover,
          DeviceId = deviceId
        };
      }

      public static Msg Ping() {
        return new Msg { Type = Types.Ping };
      }

      public static Msg NoTouch(bool isNoTouch) {
        return new Msg { Type = Types.NoTouch, Bool = isNoTouch };
      }

      public static Msg NoTouchQuery(int deviceId) {
        return new Msg(Types.NoTouchQuery, deviceId);
      }

      public static Msg NoTouchQuery(bool isNoTouch) {
        return new Msg { Type = Types.NoTouchQuery, Bool = isNoTouch };
      }

      public static Msg AddOnQuery() {
        return new Msg { Type = Types.AddOnQuery };
      }

      public static Msg AddOnQuery(bool hasSecondScreen, bool hasLEDs, int width_px = 0, int height_px = 0, int width_mm = 0, int height_mm = 0) {
        return new Msg { Type = Types.AddOnQuery, Bool = hasSecondScreen, Bool2 = hasLEDs, X = width_px, Y = height_px, W = width_mm, H = height_mm };
      }

      public static Msg SettingsMessage(ConfigDisplay config) {
        return new Msg { Type = Types.DisplaySettingsChanged, Config = config };
      }

      public static Msg HandCountQuery(int handCount) {
        return new Msg { Type = Types.HandCountQuery, X = handCount };
      }

      public static Msg HandCountQuery() {
        return new Msg { Type = Types.HandCountQuery };
      }

      public static Msg SubscribeMessage() {
        return new Msg { Type = Types.SubscribeToDisplaySettings };
      }

      public static Msg HandMessage() {
        return new Msg { Type = Types.Hands };
      }

      public static Msg PriorityMessage(int priority = 0) {
        return new Msg { Type = Types.SetPriority, Priority = priority };
      }

      public static Msg OnboardingQueryMessage(bool onboardingActive) {
        return new Msg { Type = Types.OnboardingQuery, Bool = onboardingActive };
      }

      public static Msg OnboardingQueryMessage() {
        return new Msg { Type = Types.OnboardingQuery };
      }

      public static Msg SetOnboardingMessage(bool onboardingActive) {
        return new Msg { Type = Types.SetOnboarding, Bool = onboardingActive };
      }

      public static Msg OverlayCursorVisibilityQuery(bool cursorVisible) {
        return new Msg { Type = Types.OverlayCursorVisibilityQuery, Bool = cursorVisible };
      }

      public static Msg RegistrationMessage() {
        return new Msg { Type = Types.RegisterRemoteClient };
      }

      public static Msg UsersQuery(int[] userIds, TouchlessUser[] touchlessUsers, int stateUserId, bool usingMouseEmulation) {

        List<Msg> userUpdates = new List<Msg>();
        foreach (TouchlessUser u in touchlessUsers) {
          userUpdates.Add(UserUpdate(u.RemoteUserInfo.DeviceId, u.HoverState.Value, u.HandCount, u.ScreenX, u.ScreenY, u.IsButtonDown.Value, u.InitialPress, u.InitialRelease));
        }

        return new Msg { Type = Types.UsersQuery, UserIds = userIds, TouchlessUserInfo = userUpdates.ToArray(), MouseDriverId = usingMouseEmulation ? stateUserId : -1 };
      }

      public static Msg UserUpdate(int deviceId, HoverStates hoverstate, int handCount, int screenPosX, int screenPosY, bool isClicking, bool pressed, bool released) {
        return new Msg { Type = Types.UserUpdate, DeviceId = deviceId, HoverState = hoverstate, HandCount = handCount, X = screenPosX, Y = screenPosY, IsClicking = isClicking, Bool = pressed, Bool2 = released };
      }

      public static Msg UserAdded(int deviceId) {
        return new Msg {  Type = Types.UserAdded, DeviceId = deviceId };
      }

      public static Msg UserRemoved(int deviceId) {
        return new Msg { Type = Types.UserRemoved, DeviceId = deviceId };
      }

      public static Msg StateUserQuery(int deviceId) {
        return new Msg { Type = Types.QueryStateUserId, DeviceId = deviceId };
      }
    }

    #endregion

    #region Callback Handling

    public delegate void EmptyDelegate();

    public delegate void BoolDelegate(bool isShown);

    public delegate void QueryDimsDelegate(int x, int y, int w, int h);

    public delegate void SetMousePosDelegate(int x, int y);

    public delegate void ClickAndHoverDelegate(bool click, HoverStates hover);

    public delegate void ClickAndHoverBoolDelegate(bool click, bool hover);

    public delegate void HoverStateDelegate(HoverStates hover);

    public delegate void NoTouchDelegate(bool noTouch);

    public delegate void AddOnQueryDelegate(bool hasScreen, bool hasLEDs, int width_px, int height_px, int width_mm, int height_mm);

    public delegate void HandCountQueryDelegate(int handCount);

    public delegate void OnboardingQueryDelegate(bool onboardingActive);

    public class Callback {

      public readonly Types Type;

      private readonly HashSet<object> _callbacks;

      public Callback(Types type) {
        Type = type;
        _callbacks = new HashSet<object>();
      }

      public Callback(Types type, params EmptyDelegate[] callbacks) {
        Type = type;
        _callbacks = new HashSet<object>(callbacks);
      }

      public Callback(Types type, params BoolDelegate[] callbacks) {
        Type = type;
        _callbacks = new HashSet<object>(callbacks);
      }

      public Callback(Types type, params QueryDimsDelegate[] callbacks) {
        Type = type;
        _callbacks = new HashSet<object>(callbacks);
      }

      public Callback(Types type, params SetMousePosDelegate[] callbacks) {
        Type = type;
        _callbacks = new HashSet<object>(callbacks);
      }

      public Callback(Types type, params ClickAndHoverDelegate[] callbacks) {
        Type = type;
        _callbacks = new HashSet<object>(callbacks);
      }

      public Callback(Types type, params ClickAndHoverBoolDelegate[] callbacks) {
        Type = type;
        _callbacks = new HashSet<object>(callbacks);
      }

      public Callback(Types type, params HoverStateDelegate[] callbacks) {
        Type = type;
        _callbacks = new HashSet<object>(callbacks);
      }

      public Callback(Types type, params NoTouchDelegate[] callbacks) {
        Type = type;
        _callbacks = new HashSet<object>(callbacks);
      }

      public Callback(Types type, params AddOnQueryDelegate[] callbacks) {
        Type = type;
        _callbacks = new HashSet<object>(callbacks);
      }

      public Callback(Types type, params HandCountQueryDelegate[] callbacks) {
        Type = type;
        _callbacks = new HashSet<object>(callbacks);
      }

      public Callback(Types type, params OnboardingQueryDelegate[] callbacks) {
        Type = type;
        _callbacks = new HashSet<object>(callbacks);
      }

      public Callback Invoke(Msg msg) {
        lock (_callbacks) {
          return Operate(msg, _callbacks);
        }
      }

      public Callback ClearInvoke(Msg msg) {
        object[] copy;
        lock (_callbacks) {
          copy = _callbacks.ToArray();
          _callbacks.Clear();
        }
        return Operate(msg, copy);
      }

      private Callback Operate(Msg msg, IEnumerable<object> collection) {
        switch (Type) {
          case Types.None:
          case Types.Quit:
            Operate(collection.OfType<EmptyDelegate>(), p => p());
            break;
          case Types.Hover:
          case Types.HoverQuery:
            Operate(collection.OfType<HoverStateDelegate>(), p => p(msg.HoverState));
            Operate(collection.OfType<BoolDelegate>(), p => p(msg.HoverState == HoverStates.Click));
            break;
          case Types.Options:
          case Types.Click:
          case Types.ClickQuery:
            Operate(collection.OfType<BoolDelegate>(), p => p(msg.Bool.Value));
            break;
          case Types.DimensionsQuery:
            Operate(collection.OfType<QueryDimsDelegate>(), p => p(msg.X.Value, msg.Y.Value, msg.W.Value, msg.H.Value));
            break;
          case Types.Position:
            Operate(collection.OfType<SetMousePosDelegate>(), p => p(msg.X.Value, msg.Y.Value));
            break;
          case Types.ClickAndHoverQuery:
            Operate(collection.OfType<ClickAndHoverDelegate>(), p => p(msg.Bool.Value, msg.HoverState));
            Operate(collection.OfType<ClickAndHoverBoolDelegate>(), p => p(msg.Bool.Value, msg.HoverState == HoverStates.Click));
            break;
          case Types.Ping:
            Operate(collection.OfType<EmptyDelegate>(), p => p());
            break;
          case Types.NoTouch:
          case Types.NoTouchQuery:
            Operate(collection.OfType<NoTouchDelegate>(), p => p(msg.Bool.Value));
            break;
          case Types.AddOnQuery:
            Operate(collection.OfType<AddOnQueryDelegate>(),
              p => p(
                msg.Bool.Value,
                msg.Bool2.Value,
                msg.X.Value,
                msg.Y.Value,
                msg.W.Value,
                msg.H.Value
                )
              );
            break;
          case Types.SubscribeToDisplaySettings:
            break;
          case Types.DisplaySettingsChanged:
            break;
          case Types.HandCountQuery:
            Operate(collection.OfType<HandCountQueryDelegate>(), p => p(msg.X.Value));
            break;
          case Types.Hands:
            break;
          case Types.SetPriority:
            break;
          case Types.OnboardingQuery:
            Operate(collection.OfType<OnboardingQueryDelegate>(), p => p(msg.Bool.Value));
            break;
          case Types.SetOnboarding:
            break;
          default:
            throw new ArgumentOutOfRangeException();
        }
        return this;
      }

      public Callback Clear() {
        lock (_callbacks) {
          _callbacks.Clear();
        }
        return this;
      }

      public void Add(EmptyDelegate a) {
        lock (_callbacks) {
          _callbacks.Add(a);
        }
      }

      public void Add(BoolDelegate a) {
        lock (_callbacks) {
          _callbacks.Add(a);
        }
      }

      public void Add(QueryDimsDelegate a) {
        lock (_callbacks) {
          _callbacks.Add(a);
        }
      }

      public void Add(SetMousePosDelegate a) {
        lock (_callbacks) {
          _callbacks.Add(a);
        }
      }

      public void Add(ClickAndHoverDelegate a) {
        lock (_callbacks) {
          _callbacks.Add(a);
        }
      }

      public void Add(ClickAndHoverBoolDelegate a) {
        lock (_callbacks) {
          _callbacks.Add(a);
        }
      }

      public void Add(HoverStateDelegate a) {
        lock (_callbacks) {
          _callbacks.Add(a);
        }
      }

      public void Add(NoTouchDelegate a) {
        lock (_callbacks) {
          _callbacks.Add(a);
        }
      }

      public void Add(AddOnQueryDelegate a) {
        lock (_callbacks) {
          _callbacks.Add(a);
        }
      }

      public void Add(HandCountQueryDelegate a) {
        lock (_callbacks) {
          _callbacks.Add(a);
        }
      }

      public void Add(OnboardingQueryDelegate a) {
        lock (_callbacks) {
          _callbacks.Add(a);
        }
      }

      public void Remove(object a) {
        lock (_callbacks) {
          _callbacks.Remove(a);
        }
      }

      private static void Operate<T>(IEnumerable<T> c, Action<T> a) {
        foreach (var i in c) {
          a(i);
        }
      }
    }

    #endregion

    #region Utility

    [JsonIgnore]
    public bool ContainsIncomingServerSideData {
      get {
        switch (Type) {
          case Types.None:
            return true;
          case Types.Ping:
            return false;
          case Types.Hover:
            return _serializedHoverState.HasValue;
          case Types.Quit:
            return true;
          case Types.Options:
            return Bool.HasValue;
          case Types.DimensionsQuery:
            return true;
          case Types.Position:
            return X.HasValue && Y.HasValue;
          case Types.HoverQuery:
            return true;
          case Types.Click:
            return Bool.HasValue;
          case Types.ClickQuery:
            return true;
          case Types.ClickAndHoverQuery:
            return true;
          case Types.NoTouch:
            return Bool.HasValue;
          case Types.NoTouchQuery:
            return true;
          case Types.AddOnQuery:
            return true;
          case Types.SubscribeToDisplaySettings:
            return false;
          case Types.DisplaySettingsChanged:
            return true;
          case Types.HandCountQuery:
            return true;
          case Types.Hands:
            return false;
          case Types.SetPriority:
            return false;
          case Types.OnboardingQuery:
            return true;
          case Types.SetOnboarding:
            return false;
          case Types.UsersQuery:
            return false;
          default:
            throw new ArgumentOutOfRangeException();
        }
      }
    }

    [JsonIgnore]
    public bool ContainsIncomingClientSideData {
      get {
        switch (Type) {
          case Types.None:
            return false;
          case Types.Ping:
            return true;
          case Types.Hover:
            return true;
          case Types.Quit:
            return true;
          case Types.Options:
            return Bool.HasValue;
          case Types.DimensionsQuery:
            return X.HasValue && Y.HasValue && W.HasValue && H.HasValue;
          case Types.Position:
            return true;
          case Types.HoverQuery:
            return _serializedHoverState.HasValue;
          case Types.Click:
            return true;
          case Types.ClickQuery:
            return Bool.HasValue;
          case Types.ClickAndHoverQuery:
            return Bool.HasValue && _serializedHoverState.HasValue;
          case Types.NoTouch:
            return true;
          case Types.NoTouchQuery:
            return Bool.HasValue;
          case Types.AddOnQuery:
            return Bool.HasValue && Bool2.HasValue && X.HasValue && Y.HasValue && W.HasValue && H.HasValue;
          case Types.SubscribeToDisplaySettings:
            return true;
          case Types.DisplaySettingsChanged:
            return false;
          case Types.HandCountQuery:
            return false;
          case Types.Hands:
            return true;
          case Types.SetPriority:
            return true;
          case Types.OnboardingQuery:
            return Bool.HasValue;
          case Types.SetOnboarding:
            return true;
          case Types.UsersQuery:
            return true;
          default:
            throw new ArgumentOutOfRangeException();
        }
      }
    }

    public bool ContainsData(bool isServerSide = true) {
      return isServerSide ? ContainsIncomingServerSideData : ContainsIncomingClientSideData;
    }

    #endregion
  }
}
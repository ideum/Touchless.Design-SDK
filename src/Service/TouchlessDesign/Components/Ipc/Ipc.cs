using System;
using System.Collections.Generic;
using TouchlessDesign.Components.Ipc.Networking;
using TouchlessDesign.Components.Ipc.Networking.Tcp;
using TouchlessDesign.Config;
using System.Linq;
using TouchlessDesign.Components.Input;

namespace TouchlessDesign.Components.Ipc {
  public class Ipc : AppComponent, Server.IListener {

    protected override void DoStart() {
      InitializeServers();
      _settingsInterestedClients = new List<Client>();
      _usersInterestingClients = new List<Client>();
    }

    protected override void DoStop() {
      DeInitializeServers();
    }

    private List<Client> _settingsInterestedClients;
    private List<Client> _usersInterestingClients;

    #region Message Processing

    /// <summary>
    /// Processes an incoming message from an end point
    /// </summary>
    /// <param name="msg">The message to process</param>
    /// <param name="send">Invoked to send a Msg instance to the desired end point</param>
    public void ProcessMsg(Msg msg, Client c) {
      try {
        bool isRegisteredUser = false;
        TouchlessUser user = null;
        if (Input != null) {
          isRegisteredUser = Input.RegisteredUsers.TryGetValue(msg.DeviceId, out user);
        }
        
        switch (msg.Type) {
          case Msg.Types.None:
            break;
          case Msg.Types.Hover:
            if (msg.ContainsIncomingServerSideData && msg.Priority >= Input.ClientPriority.Value) {
              Log.Warn($"Changing Hover {user.HoverState} to {msg.HoverState}");
              user.HoverState.Value = msg.HoverState;
            }
            if (isRegisteredUser) {
              Log.Warn($"Changing Hover for user {msg.DeviceId} to {msg.HoverState}");
              user.HoverState.Value = msg.HoverState;
            }
            break;
          case Msg.Types.HoverQuery:
            if (isRegisteredUser) {
              c.Send(Msg.Factories.HoverQuery(user.HoverState.Value));
            }
            break;
          case Msg.Types.Quit:

            break;
          case Msg.Types.Options:

            break;
          case Msg.Types.DimensionsQuery:
            if (isRegisteredUser) {
              c.Send(Msg.Factories.DimensionsQuery(user.Bounds.Left, user.Bounds.Top, user.Bounds.Width, user.Bounds.Height));
            }
            break;
          case Msg.Types.Position:
            if (msg.ContainsIncomingServerSideData && msg.Priority >= Input.ClientPriority.Value) {
              Input.SetPosition(msg.X.Value, msg.Y.Value);
            }
            break;
          case Msg.Types.Click:
            if (msg.ContainsIncomingServerSideData) {
              if (isRegisteredUser) {
                // TODO: Send message to actual user client instead of just setting variables for that user.
                user.SetMouseButtonDown(msg.Bool.Value);
              }
              //Input.HandleStateUserButtonDown(msg.Bool.Value);
            }
            break;
          case Msg.Types.ClickQuery:
            if (isRegisteredUser)
              c.Send(Msg.Factories.ClickQuery(user.IsButtonDown.Value));
            break;
          case Msg.Types.ClickAndHoverQuery:
            if (isRegisteredUser) {
              c.Send(Msg.Factories.ClickAndHoverQuery(user.IsButtonDown.Value, user.HoverState.Value));
            }
            break;
          case Msg.Types.Ping:
            c.Send(Msg.Factories.Ping());
            break;
          case Msg.Types.NoTouch:
            if (msg.ContainsIncomingServerSideData && msg.Priority >= Input.ClientPriority.Value) {
              Input.IsNoTouch.Value = msg.Bool.Value;
            }
            break;
          case Msg.Types.NoTouchQuery:
            c.Send(Msg.Factories.NoTouchQuery(Input.IsNoTouch.Value));
            break;
          case Msg.Types.AddOnQuery:
            var dims_px = Ui.AddOnScreenBounds;
            c.Send(Msg.Factories.AddOnQuery(
              Ui.HasAddOnScreen,
              Lighting.NetworkState == TouchlessDesign.Components.Lighting.Lighting.NetworkStates.Connected,
              dims_px.Width,
              dims_px.Height,
              Ui.AddOnWidth_mm,
              Ui.AddOnHeight_mm));
            break;
          case Msg.Types.SubscribeToDisplaySettings:
            if (!_settingsInterestedClients.Contains(c)) {
              _settingsInterestedClients.Add(c);
              Msg settingsMsg = Msg.Factories.SettingsMessage(AppComponent.Config.Display);
              c.Send(settingsMsg);
            }
            break;
          case Msg.Types.DisplaySettingsChanged:
            break;
          case Msg.Types.HandCountQuery:
            if (isRegisteredUser) {
              int handCount = user.HandCount;
              c.Send(Msg.Factories.HandCountQuery(handCount));
            }
            break;
          case Msg.Types.SetPriority:
            Input.ClientPriority.Value = msg.Priority;
            break;
          case Msg.Types.OnboardingQuery:
            c.Send(Msg.Factories.OnboardingQueryMessage(Input.IsOnboardingActive.Value));
            break;
          case Msg.Types.SetOnboarding:
            Input.IsOnboardingActive.Value = msg.Bool.Value;
            break;
          case Msg.Types.RegisterRemoteClient:
            if(Config.Input.InputProvider != 1) {
              Log.Warn($"User attempted to register, but the service is running in local mode");
            }
            if (!Input.RegisteredUsers.ContainsKey(msg.DeviceId)) {

              TouchlessUser newUser = new TouchlessUser(msg.DeviceId, c.Connection.Destination, c);
              Input.RegisterUser(newUser);
              Log.Info($"Registered User {msg.DeviceId}");
              foreach (Client interestedClient in _usersInterestingClients) {
                // Let em know we registered a user
              }
              c.Send(Msg.Factories.Ping());
            }
            else {
              Log.Info($"User {msg.DeviceId} tried to register, but is already in the system.");
            }
            break;
          case Msg.Types.UsersQuery:
            Log.Info($"Querying for users");
            break;
          case Msg.Types.SubscribeToUserChanges:
            if(!_usersInterestingClients.Contains(c)) {
              _usersInterestingClients.Add(c);
            }
            break;
          default:
            throw new ArgumentOutOfRangeException();
        }
      }
      catch (Exception e) {
        Log.Error($"Caught exception at process msg: {e}");
      }
    }



    #endregion

    #region Server

    private readonly List<Server> _servers = new List<Server>();

    private void InitializeServers() {

      if (Config.Network.TcpEnabled) {
        _servers.Add(new TcpServer(Config.Network.TcpData));
      }

      //TODO websocket implementation needs end-to-end testing before we enable this in production!
      //if (Config.Network.WsEnabled) {
      //  var i = Config.Network.WsData;
      //  _servers.Add(new WebsocketServer(i.GetEndPoint().ToString()));
      //}

      foreach (var server in _servers) {
        server.Bind(this);
        server.Start();
      }
    }

    private void DeInitializeServers() {
      foreach (var server in _servers) {
        server.Stop();
        server.Dispose();
      }
    }

    public int ClientsCount {
      get {
        var count = 0;
        foreach (var server in _servers) {
          count += server.ClientsCount;
        }
        return count;
      }
    }

    public void SendSettingsMessage(ConfigDisplay config) {
      Msg msg = Msg.Factories.SettingsMessage(config);
      foreach (Client c in _settingsInterestedClients) {
        c.Send(msg);
      }
    }

    public void SendUsersMessage() {
      Msg msg = Msg.Factories.UsersQuery();
      foreach (Client c in _usersInterestingClients) {
        c.Send(msg);
      }
    }

    public void ServerStarted(Server s) {

    }

    public void ServerStopped(Server s) {

    }

    public void ClientConnected(Client c) {

    }

    public void ClientDisconnected(Client c) {
      if (_settingsInterestedClients.Contains(c)) {
        _settingsInterestedClients.Remove(c);
      }

      if (_usersInterestingClients.Contains(c)) {
        _usersInterestingClients.Remove(c);
      }

      if(Input != null) { 
        var userKeys = Input.RegisteredUsers.Keys;
        foreach (var userKey in userKeys) {
          if (Input.RegisteredUsers[userKey].Client == c) {
            Input.DeregisterUser(Input.RegisteredUsers[userKey]);
            break;
          }
        }
      }
    }

    public void MessageReceived(Client c, Msg msg) {
      ProcessMsg(msg, c);
    }

    public void OnClientException(Client c, Exception e) {
      Log.Error($"{c.Destination} is spewing nonsense: {e}");
    }

    public void OnException(Server s, Exception e) {
      Log.Error($"{s} is spewing hate-fire: {e}");
    }

    #endregion
  }
}
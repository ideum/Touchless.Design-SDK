using System;
using System.Collections.Generic;
using TouchlessDesign.Components.Ipc.Networking;
using TouchlessDesign.Components.Ipc.Networking.Tcp;
using TouchlessDesign.Config;

namespace TouchlessDesign.Components.Ipc {
  public class Ipc : AppComponent, Server.IListener {

    protected override void DoStart() {
      InitializeServers();
      _settingsInterestedClients = new List<Client>();
    }

    protected override void DoStop() {
      DeInitializeServers();
    }

    private List<Client> _settingsInterestedClients;

    #region Message Processing

    /// <summary>
    /// Processes an incoming message from an end point
    /// </summary>
    /// <param name="msg">The message to process</param>
    /// <param name="send">Invoked to send a Msg instance to the desired end point</param>
    public void ProcessMsg(Msg msg, Client c) {
      try {
        switch (msg.Type) {
          case Msg.Types.None:
            break;
          case Msg.Types.Hover:
            if (msg.ContainsIncomingServerSideData) {
              Log.Warn($"Changing Hover {Input.HoverState.Value} to {msg.HoverState}");
              Input.HoverState.Value = msg.HoverState;
            }
            break;
          case Msg.Types.HoverQuery:
            c.Send(Msg.Factories.HoverQuery(Input.HoverState.Value));
            break;
          case Msg.Types.Quit:

            break;
          case Msg.Types.Options:

            break;
          case Msg.Types.DimensionsQuery:
            c.Send(Msg.Factories.DimensionsQuery(Input.Bounds.Left, Input.Bounds.Top, Input.Bounds.Width, Input.Bounds.Height));
            break;
          case Msg.Types.Position:
            if (msg.ContainsIncomingServerSideData) {
              Input.SetPosition(msg.X.Value, msg.Y.Value);
            }
            break;
          case Msg.Types.Click:
            if (msg.ContainsIncomingServerSideData) {
              Input.SetMouseButtonDown(msg.Bool.Value);
            }
            break;
          case Msg.Types.ClickQuery:
            c.Send(Msg.Factories.ClickQuery(Input.IsButtonDown.Value));
            break;
          case Msg.Types.ClickAndHoverQuery:
            c.Send(Msg.Factories.ClickAndHoverQuery(Input.IsButtonDown.Value, Input.HoverState.Value));
            break;
          case Msg.Types.Ping:
            c.Send(Msg.Factories.Ping());
            break;
          case Msg.Types.NoTouch:
            if (msg.ContainsIncomingServerSideData) {
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
            }
            break;
          case Msg.Types.DisplaySettingsChanged:
            break;
          case Msg.Types.HandCountQuery:
            int handCount = Input.HandCount.Value;
            c.Send(Msg.Factories.HandCountQuery(handCount));
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
using System;
using System.Collections.Generic;
using System.IO;
using TouchlessDesign.LEDs;
using TouchlessDesign.Networking;
using TouchlessDesign.Networking.Tcp;

namespace TouchlessDesign {
  public class Ipc : App.Component, Server.IListener {
    
    public override void Start() {
      InitializeServer();
    }

    public override void Stop() {
      DeInitializeServer();
    }

    #region Message Processing

    /// <summary>
    /// Processes an incoming message from an end point
    /// </summary>
    /// <param name="msg">The message to process</param>
    /// <param name="send">Invoked to send a Msg instance to the desired end point</param>
    public void ProcessMsg(Msg msg, Action<Msg> send) {
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
            send(Msg.Factories.HoverQuery(Input.HoverState.Value));
            break;
          case Msg.Types.Quit:

            break;
          case Msg.Types.Options:

            break;
          case Msg.Types.DimensionsQuery:
            send(Msg.Factories.DimensionsQuery(Input.Cursor.BoundsLeft, Input.Cursor.BoundsTop,
              Input.Cursor.BoundsWidth,
              Input.Cursor.BoundsHeight));
            break;
          case Msg.Types.Position:
            if (msg.ContainsIncomingServerSideData) {
              Input.Cursor.SetPosition(msg.X.Value, msg.Y.Value);
            }
            break;
          case Msg.Types.Click:
            if (msg.ContainsIncomingServerSideData) {
              Input.Cursor.SetMouseButtonDown(msg.Bool.Value);
            }
            break;
          case Msg.Types.ClickQuery:
            send(Msg.Factories.ClickQuery(Input.Cursor.IsButtonDown));
            break;
          case Msg.Types.ClickAndHoverQuery:
            send(Msg.Factories.ClickAndHoverQuery(Input.Cursor.IsButtonDown, Input.HoverState.Value));
            break;
          case Msg.Types.Ping:
            send(Msg.Factories.Ping());
            break;
          case Msg.Types.NoTouch:
            if (msg.ContainsIncomingServerSideData) {
              Input.IsNoTouch.Value = msg.Bool.Value;
            }
            break;
          case Msg.Types.NoTouchQuery:
            send(Msg.Factories.NoTouchQuery(Input.IsNoTouch.Value));
            break;
          case Msg.Types.AddOnQuery:
            var dims_px = Ui.AddOnScreenBounds;
            send(Msg.Factories.AddOnQuery(
              Ui.HasAddOnScreen, 
              Controller.NetworkState == Controller.NetworkStates.Connected, 
              dims_px.Width, 
              dims_px.Height, 
              Ui.AddOnWidth_mm,
              Ui.AddOnHeight_mm));
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

    private NetworkSettings _networkSettings;
    private readonly List<Server> _servers = new List<Server>();

    private void InitializeServer() {
      _networkSettings = NetworkSettings.Get(DataDir);
      _servers.Add(new TcpServer(_networkSettings.Server));

      foreach (var server in _servers) {
        server.Bind(this);
        server.Start();
      }
    }

    private void DeInitializeServer() {
      foreach (var server in _servers) {
        server.Stop();
        server.Dispose();
      }
    }

    public void ServerStarted(Server s) {
    }

    public void ServerStopped(Server s) {
    }

    public void ClientConnected(Client c) {
    }

    public void ClientDisconnected(Client c) {
    }

    public void MessageReceived(Client c, Msg msg) {
      ProcessMsg(msg, c.Send);
    }

    public void OnClientException(Client c, Exception e) {
      Log.Error($"{c.Destination} is spewing nonsense: {e}");
    }

    public void OnException(Server s, Exception e) {
      Log.Error($"{s} is spewing hate-fire: {e}");
    }

    public class NetworkSettings {
      private const string Filename = "network.json";

      public string PrimaryMsgType = "msg";
      public string PingMsgType = "ping";
      public int ReconnectClientInterval_ms = 1000;
      public int PingInterval_ms = 5000;
      public IpInfo Server;

      public static NetworkSettings Get(string dir) {
        var path = Path.Combine(dir, Filename);
        return ConfigFactory.Get(path, Defaults);
      }

      public void Save(string dir) {
        var path = Path.Combine(dir, Filename);
        ConfigFactory.Save(path, this);
      }

      public static NetworkSettings Defaults() {
        return new NetworkSettings {
          PrimaryMsgType = "msg",
          PingMsgType = "ping",
          ReconnectClientInterval_ms = 1000,
          PingInterval_ms = 5000,
          Server = new IpInfo {
            Loopback = true,
            Port = 4949
          }
        };
      }
    }

    #endregion
  }
}
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using TouchlessDesignCore.Components.Ipc;
using TouchlessDesignCore.Components.Ipc.Networking.Udp;
using TouchlessDesignCore.Components.Ipc.Networking.Tcp;
using TouchlessDesignCore.Config;
using UnityEngine;
using TouchlessDesignCore.Components.Ipc.Networking;
using System;

namespace TouchlessDesignCore.Components.Input.Providers.Remote
{
  class RemoteProvider : IInputProvider, TcpServer.IListener
  {
    private TcpServer _tcpServer;

    private UdpServer _server;
    private bool _updateFlag = false;

    private object _lock;
    private List<Hand> _handBuffer = new List<Hand>();

    private Timer _broadcastTimer;
    private int _broadcastInterval;
    private IpInfo _broadcastInfo, _commInfo;
    private int _tcpPort;

    public string DataDir { get; set; }

    public void Start()
    {
      ConfigNetwork config = ConfigNetwork.Get(DataDir);
      _broadcastInterval = config.UdpBroadcastInterval_ms;
      _broadcastInfo = config.UdpBroadcast;
      _commInfo = config.UdpData;
      _tcpPort = config.TcpData.Port;

      _tcpServer = new TcpServer(config.TcpData);
      Debug.Log("Starting server on port " + _tcpPort);
      _tcpServer.Bind(this);
      _tcpServer.Start();

      _lock = new object();

      _server = new UdpServer(_broadcastInfo, _commInfo);

      _server.MessageReceived = HandleMessageReceived;
      _server.Start();

      _broadcastTimer = new Timer(SendDiscoveryBroadcast, null, 0, _broadcastInterval);
    }

    public void Stop()
    {
      _server.SendBroadcast(Encoding.UTF8.GetBytes("Touchless_end"));
      _server.Stop();
      _broadcastTimer.Dispose();
      _tcpServer.Stop();
      _tcpServer.Dispose();
    }

    public bool Update(Dictionary<int, Hand> hands)
    {
      if (!_server.Running)
      {
        return false;
      }
      if (!_updateFlag)
      {
        return true;
      }
      _updateFlag = false;
      lock (_lock)
      {
        hands.Clear();
        foreach (Hand h in _handBuffer)
        {
          hands.Add(h.Id, h);
        }
      }
      return true;
    }

    private void SendDiscoveryBroadcast(object state)
    {
      string discovery = "Touchless_discovery " + _commInfo.Port + " " + _tcpPort;
      _server.SendBroadcast(Encoding.UTF8.GetBytes(discovery));
    }

    private void HandleMessageReceived(string messageRaw, IPEndPoint endpoint)
    {
      Msg msg;
      // Debug.Log("Msg recieved from endpoint " + endpoint.Address + ": " + messageRaw);
      if (!Msg.TryDeserialize(messageRaw, out msg))
      {
        Debug.LogWarning("Could not deserialize remote provider message");
        return;
      }

      TouchlessDesign.Sync(() => TouchlessDesign.Instance.HandleUserMessage(msg, endpoint));
      lock (_lock)
      {
        _handBuffer.Clear();
        _handBuffer = msg.Hands.ToList();
        _updateFlag = true;
      }
    }

    public void ServerStarted(Server s)
    {
      Debug.Log("Server Started");

    }

    public void ServerStopped(Server s)
    {
      Debug.Log("Server Stopped");

    }

    public void ClientConnected(Client c)
    {
      Debug.Log($"Client from endpoint {c.Connection.Destination} joined.");
      var userIps = TouchlessDesign.Instance.Users.Keys;
      var targetString = userIps.First((s) => c.Connection.Destination.Contains(s));
      if(targetString != null)
      {
        TouchlessDesign.Instance.Users[targetString].Client = c;
      }

    }

    public void ClientDisconnected(Client c)
    {
      Debug.Log($"Client from endpoint {c.Connection.Destination} disconnected.");

    }

    public void MessageReceived(Client c, Msg msg)
    {
      // Debug.Log($"Message from client at {c.Connection.Destination} of type {msg.Type} received.");
      var user = FindTouchlessUserByClient(c);

      if(user != null)
      {
        user.ClientTcpMessageReceived(msg);
      }
    }

    public void OnClientException(Client c, Exception e)
    {
      Debug.LogError(e);
    }

    public void OnException(Server s, Exception e)
    {
      Debug.LogError(e);
    }

    public TouchlessUser FindTouchlessUserByClient(Client c)
    {
      var userIps = TouchlessDesign.Instance.Users.Values;
      var user = userIps.First((u) => u.Client == c);
      return user;
    }
  }
}

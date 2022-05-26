using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using TouchlessDesign.Components.Ipc;
using TouchlessDesign.Components.Ipc.Networking.Udp;
using TouchlessDesign.Config;

namespace TouchlessDesign.Components.Input.Providers.Remote {
  class RemoteProvider : IInputProvider {

    private UdpServer _server;
    private bool _updateFlag = false;

    private object _lock;
    private Dictionary<int, List<Hand>> _handBuffer = new Dictionary<int, List<Hand>>();

    private Timer _broadcastTimer;
    private int _broadcastInterval;
    private IpInfo _broadcastInfo, _commInfo;
    private int _tcpPort;

    public string DataDir { get; set; }

    public void Start() {
      ConfigNetwork config = ConfigNetwork.Get(DataDir);
      _broadcastInterval = config.UdpBroadcastInterval_ms;
      _broadcastInfo = config.UdpBroadcast;
      _commInfo = config.UdpData;
      _tcpPort = config.TcpData.Port;

      _lock = new object();

      _server = new UdpServer(_broadcastInfo, _commInfo);

      _server.MessageReceived = HandleMessageReceived;
      _server.Start();

      _broadcastTimer = new Timer(SendDiscoveryBroadcast, null, 0, _broadcastInterval);
    }

    public void Stop() {
      _server.SendBroadcast(Encoding.UTF8.GetBytes("Touchless_end"));
      _server.Stop();
    }

    public bool Update(Dictionary<int, TouchlessUser> users) {
      if (!_server.Running) {
        return false;
      }
      if (!_updateFlag) {
        return true;
      }
      _updateFlag = false;
      lock (_lock) {
        //hands.Clear();
        foreach (var userKeyValue in users) {
          if(_handBuffer.ContainsKey(userKeyValue.Key)) {
            var user = userKeyValue.Value;
            user.Hands.Clear();
            foreach (Hand hand in _handBuffer[userKeyValue.Key]) {
              user.Hands.Add(hand);
            }
          }
        }
        //foreach(Hand h in _handBuffer) {
        //  hands.Add(h.Id, h);
        //}
      }
      return true;
    }

    private void SendDiscoveryBroadcast(object state) {
      string discovery = "Touchless_discovery " + _commInfo.Port + " " + _tcpPort;
      _server.SendBroadcast(Encoding.UTF8.GetBytes(discovery));
    }

    private void HandleMessageReceived(string messageRaw, IPEndPoint endpoint) {
      Msg msg;
      if(!Msg.TryDeserialize(messageRaw, out msg)) {
        Log.Error("Could not deserialize remote provider message");
        return;
      }

      lock (_lock) {
        _handBuffer.Clear();
        if(!_handBuffer.ContainsKey(msg.DeviceId)) {
          //Log.Info($"Recieved message from Device {msg.DeviceId}");
          _handBuffer.Add(msg.DeviceId, msg.Hands.ToList());
        }
        else {
          _handBuffer[msg.DeviceId] = msg.Hands.ToList();
        }
        _updateFlag = true;
      }
    }
  }
}

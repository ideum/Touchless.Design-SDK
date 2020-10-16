using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using TouchlessDesign.Components.Input;
using TouchlessDesign.Components.Ipc;
using TouchlessDesign.Config;

namespace TouchlessDesign.Components.Remote {
  public class RemoteClient : AppComponent {

    public bool AvailableToSend {
      get { return _sendReady; }
    }

    private UdpClient _sendClient, _receiveClient;

    private IpInfo _receiveInfo;

    private Thread _thread;
    private AutoResetEvent _endEvt;
    private AutoResetEvent _msgEvt;
    private WaitHandle[] _waitHandles;

    private bool _sendReady = false;
    private bool _running = false;

    public void SendHandData(Hand[] hands) {
      //Log.Debug("SENDING HAND DATA");
      if (!_sendReady) return;

      Msg msg = Msg.Factories.HandMessage();
      msg.Hands = hands;

      var data = Encoding.UTF8.GetBytes(msg.Serialize());

      _sendClient.Send(data, data.Length);
    }

    protected override void DoStart() {
      Log.Debug("Starting remote connection.");
      ConfigNetwork config = ConfigNetwork.Get(DataDir);

      _receiveInfo = config.UdpBroadcast;
      _receiveClient = new UdpClient();
      _receiveClient.ExclusiveAddressUse = false;
      _receiveClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
      _receiveClient.Client.Bind(new IPEndPoint(IPAddress.Any, _receiveInfo.Port));

      _endEvt = new AutoResetEvent(false);
      _msgEvt = new AutoResetEvent(false);
      _waitHandles = new WaitHandle[] { _endEvt, _msgEvt };

      _thread = new Thread(ListenWork);
      _thread.Start();
    }

    protected override void DoStop() {
      _endEvt?.Set();
      if (!_running) {
        Dispose();
      }
    }

    private void ListenWork() {
      _running = true;

      while (_running) {
        _receiveClient.BeginReceive(HandleMessageReceived, new object());
       
        int result = WaitHandle.WaitAny(_waitHandles);
        if(result == 0) {
          _running = false;
          Dispose();
        }
      }
    }

    private void Dispose() {
      _receiveClient?.Close();
      _sendClient?.Close();
    }

    private void HandleMessageReceived(IAsyncResult result) {
      IPEndPoint broadcasterEndpoint = new IPEndPoint(IPAddress.Any, _receiveInfo.Port);

      try {
        var rawResponse = _receiveClient.EndReceive(result, ref broadcasterEndpoint);

        string messageStr = Encoding.UTF8.GetString(rawResponse);
        string[] splitStr = messageStr.Split(' ');
        if(splitStr[0] == "Touchless_discovery") {
          if (_sendReady) return;

          int sendPort, tcpPort;

          if(splitStr.Length < 3 || !int.TryParse(splitStr[1], out sendPort) || !int.TryParse(splitStr[2], out tcpPort)) {
            Log.Error("Could not parse incoming discovery message.");
            return;
          }

          _sendClient = new UdpClient();
          _sendClient.Client.Bind(new IPEndPoint(broadcasterEndpoint.Address, sendPort));

          //SEND TCP PORT TO INPUT

          _sendReady = true;
        } else if(splitStr[0] == "Touchless_end") {
          _sendClient?.Close();
          _sendReady = false;
        }

      } catch (ObjectDisposedException) {
        Log.Debug("Socket was disposed while reading");
      }
      _msgEvt.Set();
    }
  }
}

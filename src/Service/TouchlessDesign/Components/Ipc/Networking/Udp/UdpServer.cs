using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using TouchlessDesign.Config;

namespace TouchlessDesign.Components.Ipc.Networking.Udp {
  class UdpServer {

    public Action<string, IPEndPoint> MessageReceived;

    public bool Running {
      get { return _running; }
    }

    private Thread _thread;
    private AutoResetEvent _endEvt;
    private AutoResetEvent _msgEvt;
    private WaitHandle[] _waitHandles;

    private UdpClient _receiveClient;
    private UdpClient _broadcastClient;

    private bool _running = false;

    private readonly IpInfo _broadcastInfo, _commInfo;

    public UdpServer(IpInfo broadcastInfo, IpInfo commInfo) {
      _broadcastInfo = broadcastInfo;
      _commInfo = commInfo;
    }

    public void Start() {
      if (_running) return;

      _receiveClient = new UdpClient();
      _receiveClient.ExclusiveAddressUse = false;
      _receiveClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
      _receiveClient.Client.Bind(new IPEndPoint(IPAddress.Any, _commInfo.Port));

      _broadcastClient = new UdpClient();
      _broadcastClient.EnableBroadcast = true;

      _endEvt = new AutoResetEvent(false);
      _msgEvt = new AutoResetEvent(false);
      _waitHandles = new WaitHandle[] { _endEvt, _msgEvt };

      _thread = new Thread(ListenWork);
      _thread.Start();
    }

    public void Stop() {
      _endEvt?.Set();
      if (!_running) {
        Dispose();
      }
    }

    public void SendBroadcast(byte[] data) {
      try {
        _broadcastClient.Send(data, data.Length, new IPEndPoint(IPAddress.Broadcast, _broadcastInfo.Port));
      } catch (SocketException e) {
        Log.Error("UDP Socket exception caught during broadcast send attempt: " + e.ToString());
      }
    }

    private void Dispose() {
      _receiveClient?.Close();
      _broadcastClient?.Close();
    }

    private void ListenWork() {
      _running = true;

      while (_running) {
        _receiveClient.BeginReceive(HandleMessageReceived, new object());

        int result = WaitHandle.WaitAny(_waitHandles);
        if (result == 0) {
          _running = false;
          Dispose();
        }
      }
    }

    private void HandleMessageReceived(IAsyncResult result) {
      IPEndPoint clientEndpoint = new IPEndPoint(IPAddress.Any, _commInfo.Port);
      try {
        var rawResponse = _receiveClient.EndReceive(result, ref clientEndpoint);

        string messageStr = Encoding.UTF8.GetString(rawResponse);
        MessageReceived?.Invoke(messageStr, clientEndpoint);

      } catch (ObjectDisposedException) {
        Log.Debug("Socket was disposed while reading");
      }
      _msgEvt.Set();
    }
  }
}

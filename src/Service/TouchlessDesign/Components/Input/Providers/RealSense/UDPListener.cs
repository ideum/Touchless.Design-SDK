using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace TouchlessDesign.Components.Input.Providers.RealSense {
  public class UDPListener : IDisposable {

    public Action<string> MessageReceived;

    public bool ClientFound {
      get { return _clientFound; }
    }

    public bool Running {
      get { return _running; }
    }

    private Thread _thread;
    private AutoResetEvent _endEvt;
    private AutoResetEvent _msgEvt;
    private WaitHandle[] _waitHandles;
    private RealSenseSettings _settings;

    private UdpClient _receiveClient;
    private UdpClient _broadcastClient;

    private bool _running = false;
    private bool _clientFound = false;

    private int _broadcastInterval;
    private Timer _broadcastTimer;

    public UDPListener(RealSenseSettings settings) {
      _settings = settings;
    }

    public void Start() {
      _endEvt = new AutoResetEvent(false);
      _msgEvt = new AutoResetEvent(false);
      _waitHandles = new WaitHandle[] { _endEvt, _msgEvt };

      var tickRate = _settings.UpdateRate_ms;
      if(tickRate <= 0) {
        tickRate = RealSenseSettings.Defaults().UpdateRate_ms;
      }
      _broadcastInterval = _settings.BroadcastIntervalMillis;

      _thread = new Thread(ListenWork);
      _thread.Start();
    }

    public void Stop() {
      _endEvt?.Set();
      _clientFound = false;
      if (!_running) {
        Dispose();
      }
    }

    private void ListenWork() {
      _receiveClient = new UdpClient();
      _receiveClient.ExclusiveAddressUse = false;
      _receiveClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
      _receiveClient.Client.Bind(new IPEndPoint(IPAddress.Any, _settings.CommunicationPort));

      _broadcastClient = new UdpClient();
      _broadcastClient.EnableBroadcast = true;
      _broadcastTimer = new Timer(SendBroadcast, null, 0, _broadcastInterval);

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

    private void SendBroadcast(object state) {
      //Log.Debug("Sending discovery broadcast...");

      var data = Encoding.UTF8.GetBytes("Touchless_discovery " + _settings.CommunicationPort);
      try {
        _broadcastClient.Send(data, data.Length, new IPEndPoint(IPAddress.Broadcast, _settings.BroadcastPort));
      } catch(SocketException e) {
        Log.Error("Socket exception caught: " + e.ToString());
      }
    }

    private void HandleMessageReceived(IAsyncResult result) {
      IPEndPoint clientEndpoint = new IPEndPoint(IPAddress.Any, _settings.CommunicationPort);
      try {
        var rawResponse = _receiveClient.EndReceive(result, ref clientEndpoint);

        string message = Encoding.UTF8.GetString(rawResponse);
        MessageReceived?.Invoke(message);
        _clientFound = true;
      } catch (ObjectDisposedException) {
        Log.Debug("Socket was disposed while reading.");
      }
      _msgEvt.Set();
    }

    public void Dispose() {
      _receiveClient?.Close();
    }
  }
}

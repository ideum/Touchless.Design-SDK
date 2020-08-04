using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using Ideum.Data;
using Newtonsoft.Json;
using Ideum.Networking.Transport;
using System.Text;

namespace Ideum {

  public class ClientManager : IDisposable, IReceiver {

    public enum States {
      Connecting,
      Connected,
      ShuttingDown
    }

    public States State { get; protected set; } = States.Connected;

    public event Action OnConnected;
    public event Action OnDisconnected;
    public event Action<string> OnRawMessage;
    public event Action<Msg> OnMessage;
    public event Action<Exception> OnError;

    private readonly int _reconnectInterval, _pingInterval;
    //private readonly ConnectionInfo _connectionInfo;
    private readonly string _msgType;
    private readonly string _pingMsgType;
    private TcpClient _connection;
    private AutoResetEvent _pingEvent;
    private ManualResetEvent _shutdownEvent;
    private IPEndPoint _endpoint;

    public ClientManager(int endPoint, int reconnectInterval, int pingInterval, string msgType, string pingMsgType) {
      _pingEvent = new AutoResetEvent(false);
      _shutdownEvent = new ManualResetEvent(false);
      _reconnectInterval = reconnectInterval;
      //_connectionInfo = new ConnectionInfo(endPoint);
      _endpoint = new IPEndPoint(IPAddress.Any, endPoint);
      _pingInterval = pingInterval;
      _msgType = msgType;
      _pingMsgType = pingMsgType;
      TryConnect();
    }

    public ClientManager(NetworkSettings settings) {
      _pingEvent = new AutoResetEvent(false);
      _shutdownEvent = new ManualResetEvent(false);
      _reconnectInterval = settings.ReconnectClientInterval_ms;
      _pingInterval = settings.PingInterval_ms;
      //_connectionInfo = new ConnectionInfo(settings.Server.GetEndPoint());
      _endpoint = settings.Server.GetEndPoint();
      _msgType = settings.PrimaryMsgType;
      _pingMsgType = settings.PingMsgType;
      TryConnect();
    }

    private void TryConnect() {
      //Log.Info("Attempting to Connect.");
      ThreadPool.QueueUserWorkItem(TryConnectLoop);
    }

    private void TryConnectLoop(object state) {
      State = States.Connecting;
      while (State == States.Connecting) {
        //Log.Info($"STATE {State}");
        try {
          if (State == States.ShuttingDown) {
            _connection?.Connection.Close();
          }
          else {
            TcpConnection c;
            if (TcpConnection.TryOpen(_endpoint, out c)) {
              State = States.Connected;
              TcpMessageParser parser = new TcpMessageParser();
              ConnectionManager manager = new ConnectionManager(c, parser);
              _connection = new TcpClient();
              _connection.Connection = manager;
              manager.Bind(this, HandleGeneralException, HandleParserError, HandleConnectionClosed, HandleRawReceive);
              ThreadPool.QueueUserWorkItem(PingLoop);
              OnConnected?.Invoke();
            }
          }
        }
        catch (Exception e) {
          Log.Error($"Connection error: {e}");
        }

        if (State == States.Connecting) {
          Thread.Sleep(_reconnectInterval);
        }
      }
    }

    private void HandleRawReceive(ConnectionManager connection, byte[] msg) {
      //Log.Debug(Encoding.UTF8.GetString(msg));
    }

    private void HandleConnectionClosed(ConnectionManager connection) {
      if (connection.Destination != _connection.Connection.Destination) return;
      _connection = null;
      Log.Info("Connection Closed");
      OnDisconnected?.Invoke();
      if (State == States.Connected) {
        TryConnect();
      }
      else if (State == States.ShuttingDown) {
        //kthx BAIIIIIII
      }
    }

    private void HandleParserError(ConnectionManager connection, string msg) {
      Log.Error("PARSING ERROR: " + msg);
    }

    private void HandleGeneralException(ConnectionManager connection, Exception msg) {
      Log.Error("GENERAL CONNECTION ERROR: " + msg);
    }

    private void PingLoop(object state) {
      var events = new WaitHandle[] { _pingEvent, _shutdownEvent };
      var stopwatch = new Stopwatch();
      while (IsConnected) {
        stopwatch.Restart();
        if (_connection == null || !_connection.Connection.IsOpen()) {
          Log.Warn("Connection timeout");
          _connection?.Connection.Close();
          break;
        }
        Msg pingMsg = new Msg() {
          Type = Msg.Types.Ping
        };
        _connection.Send(pingMsg);
        var i = WaitHandle.WaitAny(events, _pingInterval);
        if (i == 0) {
          //Log.Info("Pinged.");
          var s = _pingInterval - (int)stopwatch.ElapsedMilliseconds;
          if (s > 0) {
            Thread.Sleep(s);
          }
          continue;
        }
        else if (i == 1) {
          //Log.Warn("Ping shutdown event");
          break;
        }
        else if (i == _pingInterval) {
          Log.Warn("Ping timed out");
          _connection?.Connection.Close();
          break;
        }
      }
    }

    public void Receive(byte[] payload) {
      string incomingObject = Encoding.UTF8.GetString(payload);
      OnRawMessage?.Invoke(incomingObject);
      try {
        var t = JsonConvert.DeserializeObject<Msg>(incomingObject);
        if (t.Type == Msg.Types.Ping) {
          //Log.Info("PING");
          _pingEvent.Set();
          return;
        }
        else {

        }
        OnMessage?.Invoke(t);
      }
      catch (Exception e) {
        OnError?.Invoke(e);
      }
    }

    //private void HandleIncomingPing(PacketHeader header, Connection connection, string msg) {
    //  _pingEvent.Set();
    //}

    public void Send(Msg msg) {
      if (_connection != null) {
        //_connection.SendObject(_msgType, JsonConvert.SerializeObject(msg));
        _connection.Send(msg);
      }
    }

    public bool IsConnected {
      get { return _connection != null && State == States.Connected; }
    }

    public void Shutdown() {
      State = States.ShuttingDown;
      _shutdownEvent.Set();
      _connection?.Dispose();
    }

    //private void HandleConnectionShutdown(Connection connection) {
    //  _connection = null;
    //  Log.Info("Connection Closed");
    //  OnDisconnected?.Invoke();
    //  if (State == States.Connected) {
    //    TryConnect();
    //  }
    //  else if (State == States.ShuttingDown) {
    //    //do nothing, time to saAAAay goodniIIight
    //  }
    //}

    //private void HandleIncomingPacket(PacketHeader packetHeader, Connection connection, string incomingObject) {
    //  OnRawMessage?.Invoke(incomingObject);
    //  try {
    //    var t = JsonConvert.DeserializeObject<Msg>(incomingObject);
    //    OnMessage?.Invoke(t);
    //  }
    //  catch (Exception e) {
    //    OnError?.Invoke(e);
    //  }
    //}

    public void Dispose() {
      if (State != States.ShuttingDown) {
        Shutdown();
      }
      _pingEvent?.Dispose();
      _shutdownEvent?.Dispose();
      //NetworkComms.Shutdown();
    }
  }
}
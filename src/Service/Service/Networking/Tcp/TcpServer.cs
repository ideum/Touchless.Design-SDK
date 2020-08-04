using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace TouchlessDesign.Networking.Tcp {
  public class TcpServer : Server {

    private readonly int _backlog;
    private readonly IPEndPoint _endPoint;
    private readonly ManualResetEvent _closeEvt;
    private readonly AutoResetEvent _connectionEvt;
    private Socket _socket;
    private Thread _thread;

    public TcpServer(IpInfo info, int backlog = 10) {
      _endPoint = info.GetEndPoint();
      _backlog = backlog;
      _connectionEvt = new AutoResetEvent(false);
      _closeEvt = new ManualResetEvent(false);
    }

    protected override void DoStart() {
      _connectionEvt.Reset();
      _closeEvt.Reset();
      try {
        _socket = new Socket(_endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        _socket.Bind(_endPoint);
        _socket.Listen(_backlog);
        _thread = new Thread(ServerLoop);
        _thread.Start();
      }
      catch (Exception e) {
        DoListenerException(e);
      }
    }

    protected override void DoStop() {
      try {
        _closeEvt.Set();
        if (!_thread.Join(1000)) {
          _thread.Abort();
        }
        if (_socket.Connected) {
          _socket.Shutdown(SocketShutdown.Both);
        }
        _socket.Close(5);
        _thread = null;
        _socket = null;
      }
      catch (SocketException e) {
        Log.Error("TcpServer socket exception caught when trying to close: " + e);
      }
    }

    protected override void DoDispose() {
      _closeEvt.Dispose();
      _connectionEvt.Dispose();
    }

    protected override Parser CreateParser(Connection connection) {
      return new TcpMessageParser();
    }

    private void ServerLoop() {
      try {
        while (IsStarted) {
          _socket.BeginAccept(HandleConnectionRequest, _socket);
          if (WaitHandle.WaitAny(new WaitHandle[] { _closeEvt, _connectionEvt }) == 0) {
            return;
          }
        }
      }
      catch (ObjectDisposedException) {
        Stop();
      }
      catch (SocketException) {
        Stop();
      }
    }

    private void HandleConnectionRequest(IAsyncResult ar) {
      var server = (Socket)ar.AsyncState;
      Connection connection = null;
      try {
        if (!IsStarted) return;
        var socket = server.EndAccept(ar);
        connection = new TcpConnection(socket, socket.RemoteEndPoint.ToString());
        Log.Debug($"Connection received from {connection.Destination}");
        _connectionEvt.Set();
      }
      catch (ObjectDisposedException) {
        Stop();
      }
      catch (SocketException) {
        Stop();
      }
      if (connection != null) {
        CreateClient(connection);
      }
    }
  }
}
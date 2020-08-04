using System;
using System.Net;
using System.Net.WebSockets;
using System.Threading;

namespace TouchlessDesign.Networking.Ws {
  public class WebsocketServer : Server {

    private readonly string _address;
    private HttpListener _listener;
    private Thread _httpListenerThread;

    public WebsocketServer(string address) {
      _address = address;
    }

    protected override void DoStart() {
      _httpListenerThread = new Thread(ThreadListen);
      try {
        _listener = new HttpListener();
        _listener.Prefixes.Add(_address);
        _listener.Start();
        _httpListenerThread.Start();
      }
      catch (Exception e) {
        DoListenerException(e);
      }
    }

    protected override void DoStop() {
      try {
        //TODO graceful kill
        _listener.Stop();
        _httpListenerThread.Abort();
      }
      catch (Exception e) {
        DoListenerException(e);
      }
    }

    protected override void DoDispose() {
      
    }

    protected override Parser CreateParser(Connection connection) {
      return new WebSocketMessageParser(connection as WebsocketConnection);
    }

    private async void ThreadListen() {
      //TODO graceful kill
      while (IsStarted) {
        try {
          var listenerContext = await _listener.GetContextAsync();
          if (listenerContext.Request.IsWebSocketRequest) {
            WebSocketContext webSocketContext = await listenerContext.AcceptWebSocketAsync(subProtocol: null);
            var webSocket = webSocketContext.WebSocket;
            var connection = new WebsocketConnection(webSocket, webSocketContext.Origin);
            CreateClient(connection);
          }
          else {
            listenerContext.Response.StatusCode = 426;
            listenerContext.Response.Close();
          }
        }
        catch (Exception e) {
          DoListenerException(e);
        }
      }
    }
  }
}
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;

namespace TouchlessDesign.Networking.Ws {

  public class WebsocketConnection : Connection {

    private static int BufferSize = 256;

    public bool MessageComplete { get; private set; } = true;

    private readonly WebSocket _client;
    private readonly Thread _writeThread;
    private readonly Thread _readThread;
    private readonly AutoResetEvent _writeEvt;
    private readonly AutoResetEvent _closeEvt;
    private readonly WaitHandle[] _waitHandles;
    private readonly CancellationToken _cancelToken;
    private readonly CancellationTokenSource _cancelTokenSource;
    private readonly object _outLock = new object();

    private bool _running;
    private string _messageOut;

    public WebsocketConnection(WebSocket socket, string destination) {
      _client = socket;
      Destination = destination;

      _writeThread = new Thread(WriteWork);
      _readThread = new Thread(ReadWork);
      _closeEvt = new AutoResetEvent(false);
      _writeEvt = new AutoResetEvent(false);
      _waitHandles = new WaitHandle[] { _writeEvt, _closeEvt };

      _cancelTokenSource = new CancellationTokenSource();
      _cancelToken = _cancelTokenSource.Token;
    }

    public override void Close() {
      _cancelTokenSource.Cancel();
      _client.CloseAsync(WebSocketCloseStatus.EndpointUnavailable, "Server shutting down.", CancellationToken.None);
      _closeEvt.Set();
      _readThread.Join();
      _writeThread.Join();
      _running = false;
    }

    private void WriteWork() {
      while (_running) {
        var waitEvt = WaitHandle.WaitAny(_waitHandles);
        if (waitEvt == 0) {
          string message;
          lock (_outLock) {
            message = _messageOut;
          }
          var messageRaw = Encoding.UTF8.GetBytes(message);
          _client.SendAsync(new ArraySegment<byte>(messageRaw), WebSocketMessageType.Text, true, _cancelToken);
        }
        else if (waitEvt == 1) {
          _running = false;
          return;
        }
      }
    }

    private async void ReadWork() {
      var receiveBuffer = new byte[BufferSize];
      while (_running && _client.State == WebSocketState.Open) {
        try {
          var receiveResult = await _client.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), _cancelToken);
          if (receiveResult.MessageType == WebSocketMessageType.Close) {
            Close();
            Listener?.ConnectionClosed();
            continue;
          }
          MessageComplete = receiveResult.EndOfMessage;
          
          Listener?.MessageReceived(receiveBuffer);
          receiveBuffer = new byte[256];
        }
        catch (WebSocketException e) {
          Listener?.OnException(e);
          if (_client.State != WebSocketState.Open) {
            Listener?.ConnectionClosed();
          }
        }
      }
    }

    public override void Bind(IListener listener) {
      base.Bind(listener);
      _running = true;
      _writeThread.Start();
      _readThread.Start();
    }

    public override void Dispose() {
      Close();
    }

    public override void Send(byte[] bytes) {
      var message = Encoding.UTF8.GetString(bytes);
      lock (_outLock) {
        _messageOut = message;
      }
      _writeEvt.Set();
    }
  }
}

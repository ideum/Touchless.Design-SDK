using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Ideum.Networking.Transport {
  public class TcpConnection : IComm {

    public static IPEndPoint DeepCopy(IPEndPoint endPoint) {
      var ipCopy = new IPAddress(endPoint.Address.GetAddressBytes());
      return new IPEndPoint(ipCopy, endPoint.Port);
    }

    public static bool TryOpen(IPEndPoint endPoint, out TcpConnection connection) {
      connection = null;
      endPoint = DeepCopy(endPoint);
      try {
        //var client = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        var socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
    
        socket.Connect(endPoint);
        if (socket.Connected) {
          connection = new TcpConnection(socket);
          return true;
        }
      } catch (SocketException) {
        return false;
      }
      return false;
    }

    private class ReadState {
      public NetworkStream NetStream { get; private set; }
      public byte[] Buffer = new byte[256];

      public ReadState(NetworkStream netStream) {
        NetStream = netStream;
      }
    }

    private TcpSocket _client;
    private NetworkStream _stream;
    private ManualResetEvent _closeEvt = new ManualResetEvent(false);
    private AutoResetEvent _sendEvt = new AutoResetEvent(false);
    private AutoResetEvent _readEvt = new AutoResetEvent(false);
    private AutoResetEvent _writeEvt = new AutoResetEvent(false);
    private volatile bool _isClosed;
    private bool _initRead;

    private object _sendQueueLock = new object();
    private Queue<byte[]> _sendQueue = new Queue<byte[]>();

    private object _recieverLock = new object();
    private Action<byte[]> _onRecieve;

    private object _closeLock = new object();
    private Action _onClose;

    private object _exceptionLock = new object();
    private Action<Exception> _onException;

    public TcpConnection(Socket socket) {
      _client = new TcpSocket {
        Socket = socket ?? throw new ArgumentNullException(nameof(socket))
      };
      _stream = new NetworkStream(_client.Socket);
      ThreadPool.QueueUserWorkItem(SocketWrite, _stream);
    }

    ~TcpConnection() {
      Close();
    }

    public void Dispose() {
      Close();
    }

    public void Bind(Action<byte[]> reciever = null, Action closed = null, Action<Exception> onException = null) {
      lock (_recieverLock) {
        _onRecieve = reciever;
      }

      lock (_closeLock) {
        _onClose = closed;
      }

      lock (_exceptionLock) {
        _onException = onException;
      }

      InitRead();
    }

    public void OnRecieve(Action<byte[]> recieveAction) {
      lock (_recieverLock) {
        _onRecieve = recieveAction;
      }
      InitRead();
    }

    public void OnException(Action<Exception> exceptionAction) {
      lock (_exceptionLock) {
        _onException = exceptionAction;
      }
    }

    public void OnClose(Action closeAction) {
      lock (_closeLock) {
        _onClose = closeAction;
      }
    }

    private void InitRead() {
      if (_onRecieve == null) return;
      if (_initRead) return;

      _initRead = true;
      ThreadPool.QueueUserWorkItem(SocketRead, _stream);
    }

    public void Close() {
      if (_isClosed) return;
      _isClosed = true;
      _closeEvt.Set();
      _stream.Close(1);
      if (_client.Socket != null && _client.Socket.Connected) {
        _client.Socket.Shutdown(SocketShutdown.Both);
        _client.Socket.Close(1);
      }
      
      _stream = null;
      _client = null;
      _initRead = false;
      _closeEvt.Reset();
      lock (_closeLock) {
        _onClose?.Invoke();
      }
    }

    public void Send(byte[] bytes) {
      if (_stream == null) return;
      if (bytes.Length == 0) return;
      lock (_sendQueueLock) {
        _sendQueue.Enqueue(bytes);
      }
      _sendEvt.Set();
    }

    public bool IsClosed() {
      return _isClosed;
    }

    private void DoException(Exception e) {
      lock (_exceptionLock) {
        _onException?.Invoke(e);
      }
      Close();
    }

    private void SocketRead(object state) {
      try {
        var stream = (NetworkStream)state;
        while (true) {
          var readState = new ReadState(stream);
          // stream.BeginRead(readState.Buffer, 0, readState.Buffer.Length, DoRead, readState);
          var bytes = new byte[readState.Buffer.Length];
          int size = stream.Read(bytes, 0, readState.Buffer.Length);

          if (size == 0) {
            Close();
            return;
          }

          var bytesFinal = new byte[size];
          Array.Copy(bytes, bytesFinal, size);

          _onRecieve(bytesFinal);
          //var signalIndex = WaitHandle.WaitAny(new WaitHandle[] { _closeEvt, _readEvt });
          //if (signalIndex == 0) {
          //  return;
          //}
        }
      } catch (IOException e) {
        DoException(e);
      } catch (ObjectDisposedException e) {
        DoException(e);
      }
    }

    private void DoRead(IAsyncResult ar) {
      try {
        if (_client.Socket == null || !_client.Socket.Connected) return;
        var state = (ReadState)ar.AsyncState;
        int bytesRead = state.NetStream.EndRead(ar);

        if (bytesRead == 0) {
          Close();
          return;
        }
        _readEvt.Set();

        var bytes = new byte[bytesRead];
        Array.Copy(state.Buffer, bytes, bytesRead);

        lock (_recieverLock) {
          if (_onRecieve == null) return;
          //Debug.WriteLine(Encoding.UTF8.GetString(bytes));
          _onRecieve(bytes);
        }
      } catch (IOException e) {
        DoException(e);
      } catch (ObjectDisposedException e) {
        DoException(e);
      }
    }

    private void SocketWrite(object state) {
      var stream = (NetworkStream)state;
      var buffer = new Queue<byte[]>();
      try {
        while (true) {
          var signalIndex = WaitHandle.WaitAny(new WaitHandle[] { _closeEvt, _sendEvt });
          if (signalIndex == 0) {
            return;
          }
          lock (_sendQueueLock) {
            while (_sendQueue.Count > 0) {
              buffer.Enqueue(_sendQueue.Dequeue());
            }
          }
   
          while (buffer.Count > 0) {
            var bytes = buffer.Dequeue();

            stream.BeginWrite(bytes, 0, bytes.Length, DoWrite, stream);
            signalIndex = WaitHandle.WaitAny(new WaitHandle[] { _closeEvt, _writeEvt });
            if (signalIndex == 0) {
              return;
            }
          }
        }
      } catch (IOException e) {
        DoException(e);
      } catch (ObjectDisposedException e) {
        DoException(e);
      }
    }

    private void DoWrite(IAsyncResult ar) {
      var stream = (NetworkStream)ar.AsyncState;
      try {
        var bytesWritten = stream.EndRead(ar);
        if (bytesWritten == 0) {
          Close();
        } else {
          _writeEvt.Set();
        }
      } catch (IOException e) {
        DoException(e);
      } catch (ObjectDisposedException e) {
        DoException(e);
      }
    }

    public ISocket Socket {
      get { return _client; }
    }
  }
}
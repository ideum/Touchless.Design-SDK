using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace TouchlessDesign.Networking.Tcp {
  public class TcpConnection : Connection {

    private static int BufferSize = 256;

    public static IPEndPoint DeepCopy(IPEndPoint endPoint) {
      var ipCopy = new IPAddress(endPoint.Address.GetAddressBytes());
      return new IPEndPoint(ipCopy, endPoint.Port);
    }

    public static bool TryOpen(IPEndPoint endPoint, out TcpConnection connection) {
      connection = null;
      endPoint = DeepCopy(endPoint);
      try {
        var socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        socket.Connect(endPoint);
        if (socket.Connected) {
          connection = new TcpConnection(socket, socket.RemoteEndPoint.ToString());
          return true;
        }
      }
      catch (SocketException e) {
        Log.Error(e);
      }
      return false;
    }

    private Socket _socket;
    private NetworkStream _stream;
    private readonly ManualResetEvent _closeEvt = new ManualResetEvent(false);
    private readonly AutoResetEvent _sendEvt = new AutoResetEvent(false);
    private readonly AutoResetEvent _writeEvt = new AutoResetEvent(false);
    private volatile bool _isClosed;
    private bool _initRead;

    private readonly object _sendQueueLock = new object();
    private readonly Queue<byte[]> _sendQueue = new Queue<byte[]>();

    public TcpConnection(Socket socket, string destination) {
      Destination = destination;
      _socket = socket;
      _stream = new NetworkStream(_socket);
      ThreadPool.QueueUserWorkItem(SocketWrite, _stream);
    }

    ~TcpConnection() {
      Close();
    }

    public override void Dispose() {
      Close();
    }

    public override void Bind(IListener listener) {
      base.Bind(listener);
      InitRead();
    }

    private void InitRead() {
      if (Listener == null) return;
      if (_initRead) return;

      _initRead = true;
      ThreadPool.QueueUserWorkItem(SocketRead, _stream);
    }

    public override void Close() {
      if (_isClosed) return;
      _isClosed = true;
      _closeEvt.Set();
      _stream.Close(1);
      if (_socket != null && _socket.Connected) {
        _socket.Shutdown(SocketShutdown.Both);
        _socket.Close(1);
      }

      _stream = null;
      _socket = null;
      _initRead = false;
      _closeEvt.Reset();
      Listener?.ConnectionClosed();
    }

    public override void Send(byte[] bytes) {
      if (_stream == null) return;
      if (bytes.Length == 0) return;
      lock (_sendQueueLock) {
        _sendQueue.Enqueue(bytes);
      }
      _sendEvt.Set();
    }

    private void DoException(Exception e) {
      Listener?.OnException(e);
      Close();
    }

    private void SocketRead(object state) {
      try {
        var stream = (NetworkStream)state;
        while (true) {

          var bytes = new byte[BufferSize];
          var size = stream.Read(bytes, 0, BufferSize);

          if (size == 0) {
            Close();
            return;
          }

          var bytesFinal = new byte[size];
          Array.Copy(bytes, bytesFinal, size);

          Listener?.MessageReceived(bytesFinal);
        }
      }
      catch (IOException e) {
        DoException(e);
      }
      catch (ObjectDisposedException e) {
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
      }
      catch (IOException e) {
        DoException(e);
      }
      catch (ObjectDisposedException e) {
        DoException(e);
      }
    }

    private void DoWrite(IAsyncResult ar) {
      var stream = (NetworkStream)ar.AsyncState;
      try {
        var bytesWritten = stream.EndRead(ar);
        if (bytesWritten == 0) {
          Close();
        }
        else {
          _writeEvt.Set();
        }
      }
      catch (IOException e) {
        DoException(e);
      }
      catch (ObjectDisposedException e) {
        DoException(e);
      }
    }
  }
}
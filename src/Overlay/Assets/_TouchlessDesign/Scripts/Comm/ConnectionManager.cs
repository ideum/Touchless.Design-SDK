using Ideum.Service.Communication;
using System;
using System.Diagnostics;
using System.Text;

namespace Ideum.Networking.Transport {
  /// <summary>
  /// Handles translation between Msg instances from connection recieving raw bytes.
  /// </summary>
  public class ConnectionManager : ISender {

    public IComm Connection { get; private set; }
    public string Destination { get; }

    private IReceiver _onMsgRecieved;
    private Action<ConnectionManager, byte[]> _onMsgBytesRecieved;
    private Action<ConnectionManager, string> _onParseError;
    private object _parserLock = new object();
    private Parser _parser;
    private Action<ConnectionManager> _connectionClosed;
    private Action<ConnectionManager, Exception> _generalExceptionAction;

    /// <summary>
    /// Creates a Connection Translator.
    /// </summary>
    /// <param name="connection">The Connection instance to use</param>
    public ConnectionManager(IComm connection, Parser parser) {
      _parser = parser;
      _parser.PayloadCallback = PayloadCallback;
      _parser.ExceptionCallback = ExceptionCallback;
      Connection = connection ?? throw new ArgumentNullException(nameof(connection));
      Destination = connection.Socket.GetDestination();
    }

    private void ExceptionCallback(Exception obj) {
      _generalExceptionAction?.Invoke(this, obj);
    }

    private void OnConnectionClosed() {
      if (_connectionClosed == null) return;
      _connectionClosed(this);
    }

    /// <summary>
    /// Translate and queue a Msg instance to send
    /// </summary>
    /// <param name="msg"></param>
    public void Send(byte[] bytes) {
      //Trace.WriteLine("SENDING<" + Destination + ">:" + Encoding.UTF8.GetString(bytes));
      Connection.Send(_parser.CreateMessage(bytes));
    }

    public void Close() {
      Connection?.Close();
    }

    public void Bind(IReceiver onMsgRecieved, Action<ConnectionManager, Exception> generalExceptionAction, Action<ConnectionManager, string> onParseError = null, Action<ConnectionManager> connectionClosed = null, Action<ConnectionManager, byte[]> byteMsgRecieved = null) {
      _onMsgRecieved = onMsgRecieved;
      _generalExceptionAction = generalExceptionAction;
      _onParseError = onParseError;
      _onMsgBytesRecieved = byteMsgRecieved;
      _connectionClosed = connectionClosed;
      Connection.Bind(ConnectionRecieve, OnConnectionClosed);
    }

    private void ConnectionRecieve(byte[] raw) {
      //NOTE: prevent parser state from getting corrupted by multiple threads accessing the data.
      lock (_parserLock) {
        _parser.Consume(raw);
      }
    }

    private void PayloadCallback(byte[] bytes) {
      if (bytes == null) {
        Trace.WriteLine("No bytes are written");
        _onMsgRecieved.Receive(null);
        return;
      }
      if (Connection == null || Connection.Socket == null) {
        Trace.WriteLine("Connection or Socket is null for payload callback");
        return;
      }

      var rawMsg = Encoding.UTF8.GetString(bytes);
      //Trace.WriteLine("RECIEVED RAW<" + Connection.Socket.LocalEndPoint + ">:" + rawMsg);
      _onMsgBytesRecieved?.Invoke(this, bytes);
      _onMsgRecieved?.Receive(bytes);

      //var msg = new Msg();
      //var result = msg.TryDeserialize(bytes, out msg);
      //if (result == Msg.DecodeResult.Decoded) {
      //  _onMsgRecieved.Receive(bytes);
      //} else {
      //  Trace.WriteLine("PARSE FAIL<" + Connection.Socket.LocalEndPoint + ">:" + rawMsg);
      //  _onParseError(this, result, rawMsg);
      //}
    }

    public bool IsOpen() {
      return !Connection.IsClosed();
    }
  }
}
using System;
using System.Diagnostics;
using System.Text;
using Newtonsoft.Json;

namespace TouchlessDesign.Networking {

  public class Client : IDisposable, Parser.IListener, Connection.IListener {
    
    public interface IListener {
      void MessageReceived(Client client, Msg msg);
      void ConnectionClosed(Client client);
      void OnException(Client client, Exception e);
    }

    public Connection Connection { get;  }
    public string Destination { get; set; }

    private readonly object _parserLock = new object();
    private readonly Parser _parser;
    private IListener _listener;

    public Client(Connection connection, Parser parser) {
      Connection = connection;
      Connection.Bind(this);
      _parser = parser;
      _parser.Bind(this);
    }

    public void Dispose() {
      Connection?.Close();
    }

    public void Receive(byte[] bytes) {
      var raw = "";
      if (_listener == null) return;
      try {
        raw = Encoding.UTF8.GetString(bytes);
        var msg = JsonConvert.DeserializeObject<Msg>(raw);
        if (msg != null) {
          _listener.MessageReceived(this, msg);
        }
      }
      catch (Exception e) {
        Log.Error($"Parse error when deserializing msg received from '{Destination}' of {raw}: {e}");
      }
    }

    public void Send(Msg msg) {
      if (msg == null) return;
      var raw = Encoding.UTF8.GetBytes(msg.Serialize());
      byte[] bytes;
      lock (_parserLock) {
        bytes = _parser.CreateMessage(raw);
      }
      Connection.Send(bytes);
    }

    public void Bind(IListener listener) {
      _listener = listener;
    }

    public void OnMessage(byte[] data) {
      if (data == null) {
        Trace.WriteLine("No bytes are written");
        Receive(null);
        return;
      }
      if (Connection == null) {
        Trace.WriteLine("Connection or Socket is null for payload callback");
        return;
      }
      Receive(data);
    }

    public void MessageReceived(byte[] bytes) {
      //NOTE: prevent parser state from getting corrupted by multiple threads accessing the data.
      lock (_parserLock) {
        _parser.Consume(bytes);
      }
    }

    public void ConnectionClosed() {
      _listener?.ConnectionClosed(this);
    }

    public void OnException(Exception e) {
      _listener?.OnException(this, e);

    }
  }
}
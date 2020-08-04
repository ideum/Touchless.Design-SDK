using System;
using System.Collections.Generic;
using System.Linq;

namespace TouchlessDesign.Networking {
  public abstract class Server : Client.IListener, IDisposable {
    
    #region Life Cycle

    public bool IsStarted { get; private set; }

    #region Start

    private bool _isStarting;

    public void Start() {
      if (IsStarted) return;
      if (_isStarting) return;
      _isStarting = true;
      if (TryCatch(DoStart)) {
        IsStarted = true;
        _listener?.ServerStarted(this);
      }
      _isStarting = false;
    }

    protected abstract void DoStart();

    #endregion

    #region Stop

    private bool _isStopping;

    public void Stop() {
      if (!IsStarted) return;
      if (_isStopping) return;
      _isStopping = true;

      Client[] clients;
      lock (_clients) {
        clients = _clients.ToArray();
      }

      foreach (var client in clients) {
        client.Connection.Close();
      }

      if (TryCatch(DoStop)) {
        IsStarted = false;
        _listener?.ServerStopped(this);
      }
      _isStopping = false;
    }

    protected abstract void DoStop();

    #endregion

    #region IDisposable

    private bool _isDisposed;

    public void Dispose() {
      if (_isDisposed) return;
      _isDisposed = true;
      Stop();
      DoDispose();
    }

    protected abstract void DoDispose();

    #endregion

    #endregion

    #region Listener

    public interface IListener {
      void ServerStarted(Server s);
      void ServerStopped(Server s);
      void ClientConnected(Client c);
      void ClientDisconnected(Client c);
      void MessageReceived(Client c, Msg msg);
      void OnClientException(Client c, Exception e);
      void OnException(Server s, Exception e);
    }

    private IListener _listener;

    public void Bind(IListener listener) {
      _listener = listener;
    }

    #endregion

    #region Client Handling

    private readonly HashSet<Client> _clients = new HashSet<Client>();

    protected abstract Parser CreateParser(Connection connection);

    protected void CreateClient(Connection connection) {
      var parser = CreateParser(connection);
      var client = new Client(connection, parser);
      client.Bind(this);
      lock (_clients) {
        _clients.Add(client);
      }

      _listener?.ClientConnected(client);
    }

    void Client.IListener.MessageReceived(Client client, Msg msg) {
      _listener?.MessageReceived(client, msg);
    }

    void Client.IListener.ConnectionClosed(Client client) {
      lock (_clients) {
        _clients.Remove(client);
      }
      _listener?.ClientDisconnected(client);
      client.Dispose();
    }

    void Client.IListener.OnException(Client client, Exception e) {
      _listener?.OnClientException(client, e);
    }

    #endregion

    #region Exception Handling

    protected void DoListenerException(Exception e) {
      _listener?.OnException(this, e);
    }

    protected bool TryCatch(Action a) {
      try {
        a();
        return true;
      }
      catch (Exception e) {
        Log.Error($"Server exception caught: {e}");
        DoListenerException(e);
        return false;
      }
    }

    #endregion

  }
}
using System;

namespace TouchlessDesign.Networking {
  public abstract class Connection : IDisposable {

    public interface IListener {
      void MessageReceived(byte[] bytes);
      void ConnectionClosed();
      void OnException(Exception e);
    }

    public string Destination { get; protected set; }

    protected IListener Listener { get; private set; }

    public virtual void Bind(IListener listener) {
      Listener = listener;
    }

    public abstract void Close();
    public abstract void Send(byte[] bytes);
    public abstract void Dispose();

  }
}

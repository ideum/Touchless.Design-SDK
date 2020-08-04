using System;

namespace TouchlessDesign.Networking {
  public abstract class Parser {
    
    public interface IListener {
      void OnMessage(byte[] data);
      void OnException(Exception e);
    }

    protected IListener Listener { get; private set; }

    public void Bind(IListener listener) {
      Listener = listener;
    }
    
    public abstract void Consume(byte[] raw);
    public abstract byte[] CreateMessage(byte[] payload);

  }
}

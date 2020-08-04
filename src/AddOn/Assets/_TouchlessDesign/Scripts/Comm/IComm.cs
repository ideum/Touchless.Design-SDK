using System;

namespace Ideum.Networking.Transport {
  public interface IComm : IDisposable {
    void Bind(Action<byte[]> reciever = null, Action closed = null, Action<Exception> onException = null);
    void Close();
    void Send(byte[] bytes);
    ISocket Socket { get; }
    bool IsClosed();
  }
}

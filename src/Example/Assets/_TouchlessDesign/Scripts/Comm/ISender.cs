using System;

namespace Ideum.Networking.Transport {
  public interface ISender {
    void Send(byte[] bytes);
    void Close();
    bool IsOpen();
    void Bind(IReceiver onMsgRecieved, Action<ConnectionManager, Exception> generalExceptionAction, Action<ConnectionManager, string> onParseError, Action<ConnectionManager> connectionClosed, Action<ConnectionManager, byte[]> byteMsgRecieved);
    string Destination {
      get;
    }
  }
}
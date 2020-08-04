using Ideum.Data;
using System;
using System.Text;

namespace Ideum.Networking.Transport {
  public class TcpClient : IReceiver, IDisposable, IClient {

    public ISender Connection { get; set; }
    public uint Id { get; set; }

    public Action<TcpClient, object> OnMessageReceieved;

    public virtual void Dispose() {
      Connection?.Close();
    }

    public virtual void Receive(byte[] bytes) {
      OnMessageReceieved(this, bytes);
    }

    public void Send(IPayload payload) {
      Msg message = payload as Msg;

      if (message == null) return;
      byte[] messageRaw = Encoding.UTF8.GetBytes(message.Serialize());
      Connection?.Send(messageRaw);
    }
  }
}

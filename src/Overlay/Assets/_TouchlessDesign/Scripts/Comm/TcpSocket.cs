using System.Net.Sockets;

namespace Ideum.Networking.Transport {
  public class TcpSocket : ISocket {

    public Socket Socket;

    public string GetDestination() {
      return Socket.RemoteEndPoint.ToString();
    }
  }
}
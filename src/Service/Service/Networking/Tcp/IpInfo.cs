using System.Net;

namespace TouchlessDesign.Networking.Tcp {
  public class IpInfo {

    public string Address;
    public bool Loopback;
    public int Port;

    public virtual IPEndPoint GetEndPoint() {
      if (Loopback) {
        return new IPEndPoint(IPAddress.Loopback, Port);
      }

      if (string.IsNullOrEmpty(Address)) {
        return new IPEndPoint(IPAddress.Any, Port);
      }

      return new IPEndPoint(IPAddress.Parse(Address), Port);
    }
  }
}
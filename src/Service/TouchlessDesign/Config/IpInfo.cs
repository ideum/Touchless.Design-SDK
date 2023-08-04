using System.Net;

namespace TouchlessDesign.Config {
  public class IpInfo {

    public string Address;
    public bool RemoteLoopback;
    public bool ServerLoopback;
    public int Port;

    public virtual IPEndPoint GetEndPoint() {
      if (RemoteLoopback) {
        return new IPEndPoint(IPAddress.Loopback, Port);
      }

      if (string.IsNullOrEmpty(Address)) {
        return new IPEndPoint(IPAddress.Any, Port);
      }

      return new IPEndPoint(IPAddress.Parse(Address), Port);
    }
  }
}
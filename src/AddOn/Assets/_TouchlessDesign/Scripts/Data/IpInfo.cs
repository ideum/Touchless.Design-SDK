using Ideum.Data;
using System.Net;

namespace Ideum {
  public class IpInfo {
    
    public string Address;
    public bool RemoteLoopback;
    public bool ServerLoopback;
    public int Port;

    public virtual IPEndPoint GetEndPoint() {

      if (ServerLoopback) {
        return new IPEndPoint(IPAddress.Loopback, Port);
      }

      if (string.IsNullOrEmpty(Address)) {
        return new IPEndPoint(IPAddress.Any, Port);
      }

      return new IPEndPoint(IPAddress.Parse(Address), Port);
    }
  }
}
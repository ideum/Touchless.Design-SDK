using Ideum.Data;
using System.Net;

namespace Ideum {
  public class IpInfo {
    
    public string Address;
    public bool Loopback;
    public bool SdkLoopback;
    public int Port;

    public virtual IPEndPoint GetEndPoint() {

      if (SdkLoopback) {
        return new IPEndPoint(IPAddress.Loopback, Port);
      }

      if (string.IsNullOrEmpty(Address)) {
        return new IPEndPoint(IPAddress.Any, Port);
      }

      return new IPEndPoint(IPAddress.Parse(Address), Port);
    }
  }
}
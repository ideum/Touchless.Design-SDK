using System.IO;

namespace TouchlessDesign.Config {
  public class ConfigNetwork : ConfigBase<ConfigNetwork> {
    
    public int ReconnectClientInterval_ms = 1000;

    public int PingInterval_ms = 5000;

    public bool TcpEnabled = true;

    public IpInfo TcpData = new IpInfo {
      Address = null,
      Loopback = true,
      Port = 4949
    };

    public bool WsEnabled = false;

    public IpInfo WsData = new IpInfo {
      Address = null,
      Loopback = true,
      Port = 4950
    };

    public int UdpBroadcastInterval_ms = 1000;

    public bool UdpEnabled = true;

    public IpInfo UdpBroadcast = new IpInfo {
      Address = null,
      Loopback = false,
      Port = 4951
    };

    public IpInfo UdpData = new IpInfo {
      Address = null,
      Loopback = false,
      Port = 4952
    };

    public IpInfo FadeCandyData = new IpInfo {
      Loopback = true,
      Port = 7890
    };

    public IpInfo FadeCandyRelay = new IpInfo {
      Loopback = true,
      Port = 7891
    };

    public int FadeCandyInitialConnectInterval_ms = 500;

    public int FadeCandyReconnectInterval_ms = 500;

    private const string Filename = "network.json";

    public static ConfigNetwork Get(string dir) {
      var path = Path.Combine(dir, Filename);
      return Factory.Get(path, () => new ConfigNetwork {
        FilePath = path
      });
    }

    public void Save(string dir) {
      var path = Path.Combine(dir, Filename);
      Factory.Save(path, this);
    }

    public override void Apply(ConfigNetwork i) {
      ReconnectClientInterval_ms = i.ReconnectClientInterval_ms;
      PingInterval_ms = i.PingInterval_ms;
      TcpEnabled = i.TcpEnabled;
      TcpData = i.TcpData;
      WsEnabled = i.WsEnabled;
      WsData = i.WsData;
      UdpBroadcastInterval_ms = i.UdpBroadcastInterval_ms;
      UdpEnabled = i.UdpEnabled;
      UdpBroadcast = i.UdpBroadcast;
      UdpData = i.UdpData;
      FadeCandyData = i.FadeCandyData;
      FadeCandyInitialConnectInterval_ms = i.FadeCandyInitialConnectInterval_ms;
      FadeCandyReconnectInterval_ms = i.FadeCandyReconnectInterval_ms;
    }
  }
}
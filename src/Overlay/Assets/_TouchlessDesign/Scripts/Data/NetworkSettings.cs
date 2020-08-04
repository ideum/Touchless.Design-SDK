using System;
using System.IO;

namespace Ideum.Data {
  public class NetworkSettings {
    private const string Filename = "network.json";

    public string PrimaryMsgType = "msg";
    public string PingMsgType = "ping";
    public int ReconnectClientInterval_ms = 1000;
    public int PingInterval_ms = 5000;
    public IpInfo Server;

    public static NetworkSettings Get(string dir) {
      var path = Path.Combine(dir, Filename);
      return ConfigFactory.Get(path, Defaults);
    }

    public void Save(string dir) {
      var path = Path.Combine(dir, Filename);
      this.SaveConfig(path);
    }

    public static NetworkSettings Defaults() {
      return new NetworkSettings {
        PrimaryMsgType = "msg",
        PingMsgType = "ping",
        ReconnectClientInterval_ms = 1000,
        PingInterval_ms = 5000,
        Server = new IpInfo {
          Loopback = true,
          Port = 4949
        }
      };
    }
  }
}
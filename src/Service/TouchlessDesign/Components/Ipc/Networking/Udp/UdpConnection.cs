using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TouchlessDesign.Components.Ipc.Networking.Udp {
  class UdpConnection : Connection {

    private UdpClient _client;
    private IPEndPoint _endpoint;

    public override void Close() {
      
    }

    public override void Dispose() {
      
    }

    public override void Send(byte[] bytes) {
      _client.Send(bytes, bytes.Length, _endpoint);
    }
  }
}

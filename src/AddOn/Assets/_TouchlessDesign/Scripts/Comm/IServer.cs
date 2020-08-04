using System;
using System.Net.Sockets;

namespace Ideum.Service.Communication
{
  public interface IServer : IDisposable {
    void Stop();
    void Start();
    void SetProtocol(ProtocolType protocol);
  }
}

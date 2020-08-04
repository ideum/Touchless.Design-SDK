namespace Ideum.Networking.Transport {
  public interface IReceiver {
    void Receive(byte[] bytes);
  }
}
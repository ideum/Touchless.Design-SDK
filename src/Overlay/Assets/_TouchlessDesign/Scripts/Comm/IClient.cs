namespace Ideum {
  public interface IClient {

    uint Id { get; set; }
    void Send(IPayload payload);
  }
}
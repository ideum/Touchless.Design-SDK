using System;

namespace Ideum.Networking.Transport{
  public abstract class Parser {
    public Action<byte[]> PayloadCallback;
    public Action<Exception> ExceptionCallback;

    public abstract void Consume(byte[] raw);
    public abstract byte[] CreateMessage(byte[] payload);
    //public abstract byte[] CreateMessage(byte[] payload, uint client, uint module, uint message);
    //public abstract bool DecodeMessage(byte[] raw, out uint clientId, out uint moduleId, out uint messageId, out byte[] payload);
  }
}

using System;
using System.Diagnostics;

namespace TouchlessDesign.Networking.Tcp {
  public class TcpMessageParser : Parser {

    public override byte[] CreateMessage(byte[] payload) {
      var headerBytes = BitConverter.GetBytes(payload.Length);
      var msg = new byte[headerBytes.Length + payload.Length];
      Array.Copy(headerBytes, msg, headerBytes.Length);
      Array.Copy(payload, 0, msg, headerBytes.Length, payload.Length);
      return msg;
    }

    private int _headerCount;
    private readonly byte[] _header = new byte[4];
    private int _payloadCount;
    private byte[] _payload;

    public void Clear() {
      _headerCount = 0;
      _payloadCount = 0;
      _payload = null;
    }

    public override void Consume(byte[] raw) {
      try {
        while (raw.Length > 0) {
          var rawReadCount = 0;
          while (rawReadCount < raw.Length && _headerCount < _header.Length) {
            _header[_headerCount] = raw[rawReadCount];
            rawReadCount++;
            _headerCount++;
          }

          if (rawReadCount >= raw.Length) return;
          if (_headerCount != _header.Length) return;

          if (_payload == null) {
            var tLen = BitConverter.ToInt32(_header, 0);
            if (tLen <= 0) {
              Listener?.OnMessage(null);
              Clear();
              return;
            }

            _payload = new byte[tLen];
            _payloadCount = 0;
          }

          var availablePayloadBytes = _payload.Length - _payloadCount;
          var availableRawBytes = raw.Length - rawReadCount;
          var min = Math.Min(availablePayloadBytes, availableRawBytes);

          Array.Copy(raw, rawReadCount, _payload, _payloadCount, min);

          if (min == availablePayloadBytes && min == availableRawBytes) {
            Notify();
            return; //PERFECT SCORE!
          }
          else if (min == availableRawBytes) {
            _payloadCount += min;
            return; //WE REQUIRE MORE BYTES
          }
          else if (min == availablePayloadBytes) {
            rawReadCount += availablePayloadBytes;
            Notify();

            var leftovers = new byte[raw.Length - rawReadCount]; //this should never be 0.
            Array.Copy(raw, rawReadCount, leftovers, 0, leftovers.Length);
            raw = leftovers;
            continue;
          }

          break;
        }
      }
      catch (OutOfMemoryException e) {
        Trace.WriteLine("Out of memory exception thrown on ByteMessageParser. Dumping all data.");
        Clear();
        Listener?.OnException(e);
      }
      catch (Exception e) {
        Listener?.OnException(e);
      }
    }

    private void Notify() {
      var bytes = _payload;
      Clear();
      Listener?.OnMessage(bytes);
    }
  }
}
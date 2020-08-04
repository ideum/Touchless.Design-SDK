using System;
using System.Collections.Generic;
using System.Linq;

namespace TouchlessDesign.Networking.Ws {
  public class WebSocketMessageParser : Parser {

    private readonly WebsocketConnection _comm;

    private byte[] _savedPayload;
    private int _payloadCount;

    public WebSocketMessageParser(WebsocketConnection comm) {
      _comm = comm;
    }

    public override void Consume(byte[] raw) {
      try {
        if (_comm.MessageComplete) {
          if (_payloadCount == 0) {
            _savedPayload = raw;
            Notify();
          }
          else {
            var newArray = new byte[_payloadCount + raw.Length];
            Array.Copy(_savedPayload, 0, newArray, 0, _savedPayload.Length);
            Array.Copy(raw, 0, newArray, _payloadCount, raw.Length);
            _savedPayload = newArray;
            Notify();
          }
        }
        else {
          var newArray = new byte[_payloadCount + raw.Length];
          if (_payloadCount > 0) {
            Array.Copy(_savedPayload, 0, newArray, 0, _savedPayload.Length);
          }
          Array.Copy(raw, 0, newArray, _payloadCount, raw.Length);
          _payloadCount += raw.Length;
          _savedPayload = newArray;
        }
      }
      catch (Exception e) {
        Listener?.OnException(e);
      }
    }

    public override byte[] CreateMessage(byte[] payload) {
      return payload;
    }

    private void Notify() {
      var array = new byte[_savedPayload.Length];
      lock (_savedPayload) {
        Array.Copy(_savedPayload, 0, array, 0, _savedPayload.Length);
      }
      var trimmedArray = TrimMessage(array);
      Listener?.OnMessage(trimmedArray);
      Clear();
    }

    private static byte[] TrimMessage(IReadOnlyList<byte> array) {
      var trimmedList = array.ToList();
      for (var i = trimmedList.Count - 1; i > 0; i--) {
        if (array[i] == '\0') {
          trimmedList.RemoveAt(i);
        }
        else {
          break;
        }
      }
      return trimmedList.ToArray();
    }

    private void Clear() {
      _savedPayload = null;
      _payloadCount = 0;
    }
  }
}

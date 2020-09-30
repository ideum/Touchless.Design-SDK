using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouchlessDesign;

namespace TouchlessDesign.Components.Input.Providers.RealSense {

  public class RealSenseHand {
    [JsonProperty("3dX")]
    public float X;
    [JsonProperty("3dY")]
    public float Y;
    [JsonProperty("3dZ")]
    public float Z;

    [JsonProperty("grabbing")]
    public float Grabbing;
    [JsonProperty("gestureName")]
    public string GestureName;

    [JsonProperty("id")]
    public int Id;
  }

  public class RealSenseMessage {

    public enum MessageType {
      Hand,
      HSV
    }
    
    [JsonProperty("Type")]
    public MessageType Type;

    [JsonProperty("Hands")]
    public RealSenseHand[] Hands;

    [JsonProperty("HMin")]
    public int HMin;
    [JsonProperty("HMax")]
    public int HMax;
    [JsonProperty("SMin")]
    public int SMin;
    [JsonProperty("SMax")]
    public int SMax;
    [JsonProperty("VMin")]
    public int VMin;
    [JsonProperty("VMax")]
    public int VMax;

    public static bool TryDeserialize(string msg, out RealSenseMessage message) {
      message = null;
      try {
        message = JsonConvert.DeserializeObject<RealSenseMessage>(msg);
        return true;

      } catch (JsonReaderException e) {
        Log.Error("RealSense message parsing error: " + e);
        return false;
      } catch (JsonException e) {
        Log.Error("RealSense message parsing error: " + e);
        return false;
      } catch (ArgumentNullException e) {
        Log.Error("RealSense message parsing error: " + e);
        return false;
      }
    }

    public string Serialize() {
      try {
        string jsonString = JsonConvert.SerializeObject(this);
        return jsonString;
      } catch (JsonException e) {
        Log.Error(e);
        return null;
      }
    }
  }
}

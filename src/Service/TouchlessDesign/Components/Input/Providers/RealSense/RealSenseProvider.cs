using Leap;
using System;
using System.Collections.Generic;

//NOTE: Setting HSV values is still in development

namespace TouchlessDesign.Components.Input.Providers.RealSense {
  public class RealSenseProvider : IInputProvider {
    public string DataDir { get; set; }
    public Action<HSVValues> HSVUpdate;

    private const float Conversion = 10f;

    private UDPListener _listener;
    private LeapTransform _xform;
    private bool _handUpdateFlag = false;

    private object _lock;
    private List<RealSenseHand> _handBuffer = new List<RealSenseHand>();

    private readonly List<Hand> _handsToRemoveBuffer = new List<Hand>();

    private int _count = 0;

    public void Start() {
      _lock = new object();
      _xform = new LeapTransform(Vector.Zero, LeapQuaternion.Identity, new Vector(Conversion, Conversion, Conversion));
      _xform.MirrorZ();
      Log.Debug("Starting RealSense UDP listener.");
      _listener = new UDPListener(AppComponent.Config.Input.UpdateRate_ms, AppComponent.Config.Network.UdpBroadcastInterval_ms, AppComponent.Config.Network.UdpBroadcast, AppComponent.Config.Network.UdpData);
      _listener.MessageReceived = HandleMessageReceived;
      _listener.Start();
    }

    public void Stop() {
      Log.Debug("Stopping RealSense UDP listener.");
      _listener.Stop();
    }

    //public void SendHSV(HSVValues values) {
    //  RealSenseMessage msg = new RealSenseMessage();
    //  msg.Type = RealSenseMessage.MessageType.HSV;

    //  msg.HMin = values.HMin;
    //  msg.HMax = values.HMax;
    //  msg.SMin = values.SMin;
    //  msg.SMax = values.SMax;
    //  msg.HMin = values.VMin;
    //  msg.HMax = values.VMax;

    //  string msgStr = msg.Serialize();
    //  _listener.SendHSVBroadcast(Encoding.UTF8.GetBytes("hello"));
    //}

    private void HandleMessageReceived(string message) {
      RealSenseMessage msg;
      if (RealSenseMessage.TryDeserialize(message, out msg)) {
        if(msg.Type == RealSenseMessage.MessageType.Hand) {
          HandleHandData(msg);
        } else if(msg.Type == RealSenseMessage.MessageType.HSV) {
          //HandleHSVData(msg);
        }
      }
    }

    private void HandleHandData(RealSenseMessage m) {
      _handUpdateFlag = true;
      lock (_lock) {
        _handBuffer.Clear();
        for(int i = 0; i < m.Hands.Length; i++) {
          _handBuffer.Add(m.Hands[i]);
        }
      }
    }

    //private void HandleHSVData(RealSenseMessage m) {
    //  HSVValues values = new HSVValues() {
    //    HMax = m.HMax,
    //    HMin = m.HMin,
    //    SMax = m.SMax,
    //    SMin = m.SMin,
    //    VMax = m.VMax,
    //    VMin = m.VMin
    //  };

    //  HSVUpdate?.Invoke(values);
    //}

    public bool Update(Dictionary<int, Hand> hands) {
      if(!_listener.ClientFound || !_listener.Running) {
        return false;
      }
      if (!_handUpdateFlag) {
        return true;
      }
      _handsToRemoveBuffer.AddRange(hands.Values);
      lock (_lock) {
        foreach (var rHand in _handBuffer) {
          if(hands.TryGetValue(rHand.Id, out var hand)) {
            hand.Apply(rHand, _xform);
          } else {
            hand = new Hand(rHand, _xform);
            hands.Add(rHand.Id, hand);
          }
          _handsToRemoveBuffer.Remove(hand);
        }
      }
      foreach(var hand in _handsToRemoveBuffer) {
        hands.Remove(hand.Id);
      }
      _handsToRemoveBuffer.Clear();
      _handUpdateFlag = false;
      return true;
    }
  }
}

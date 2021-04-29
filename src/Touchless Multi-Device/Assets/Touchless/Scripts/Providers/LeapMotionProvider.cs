using System.Collections.Generic;
using Leap;
using UnityEngine;

namespace TouchlessDesign.Components.Input.Providers.LeapMotion
{
  public class LeapMotionProvider : IInputProvider
  {

    private const float MillimetersToMeters = 0.001f;

    public string DataDir { get; set; }

    private LeapTransform _xform;
    private Controller _controller;

    private readonly List<Hand> _handsToRemoveBuffer = new List<Hand>();

    public void Start()
    {
      _xform = new LeapTransform(Vector.Zero, LeapQuaternion.Identity, new Vector(MillimetersToMeters, MillimetersToMeters, MillimetersToMeters));
      _xform.MirrorZ();
      _controller = new Controller();
      _controller.Connect += HandleLeapConnected;
      _controller.Disconnect += HandleLeapDisconnected;
      _controller.StartConnection();
    }

    public void Stop()
    {
      _controller.StopConnection();
      _controller.Connect -= HandleLeapConnected;
      _controller.Disconnect -= HandleLeapDisconnected;
      _controller = null;
    }

    private void HandleLeapConnected(object sender, ConnectionEventArgs e)
    {
      Debug.Log("Leap Connected.");
    }

    private void HandleLeapDisconnected(object sender, ConnectionLostEventArgs e)
    {
      Debug.Log("Leap Disconnected");
    }

    public bool Update(Dictionary<int, Hand> hands)
    {
      if (_controller == null || !_controller.IsConnected)
      {
        return false;
      }
      var f = _controller.Frame(0);
      _handsToRemoveBuffer.AddRange(hands.Values);
      foreach (var leapHand in f.Hands)
      {
        if (hands.TryGetValue(leapHand.Id, out var hand))
        {
          hand.Apply(leapHand, _xform);
        }
        else
        {
          hand = new Hand(leapHand, _xform);
          hands.Add(leapHand.Id, hand);
        }
        _handsToRemoveBuffer.Remove(hand); //prevent this hand from being removed
      }

      foreach (var hand in _handsToRemoveBuffer)
      { //remove all hands that were not added or updated this frame
        hands.Remove(hand.Id);
      }
      _handsToRemoveBuffer.Clear();
      return true;
    }
  }
}
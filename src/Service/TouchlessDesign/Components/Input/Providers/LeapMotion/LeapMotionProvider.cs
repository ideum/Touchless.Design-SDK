using System.Collections.Generic;
using System.Threading;
using System.Linq;
using Leap;

namespace TouchlessDesign.Components.Input.Providers.LeapMotion {
  public class LeapMotionProvider : IInputProvider {

    private const float MillimetersToMeters = 0.001f;

    public string DataDir { get; set; }

    private LeapTransform _xform;
    private Controller _controller;

    private readonly List<Hand> _handsToRemoveBuffer = new List<Hand>();

    public void Start() {
      _xform = new LeapTransform(Vector.Zero, LeapQuaternion.Identity, new Vector(MillimetersToMeters, MillimetersToMeters, MillimetersToMeters));
      _xform.MirrorZ();
      _controller = new Controller();
      _controller.Connect += HandleLeapConnected;
      _controller.Disconnect += HandleLeapDisconnected;
      _controller.StartConnection();
    }

    public void Stop() {
      _controller.StopConnection();
      _controller.Connect -= HandleLeapConnected;
      _controller.Disconnect -= HandleLeapDisconnected;
      _controller = null;
    }

    private void HandleLeapConnected(object sender, ConnectionEventArgs e) {
      Log.Info("Leap Connected.");
    }

    private void HandleLeapDisconnected(object sender, ConnectionLostEventArgs e) {
      Log.Info("Leap Disconnected");
    }

    public bool Update(Dictionary<int, TouchlessUser> users) {
      if (_controller == null || !_controller.IsConnected) {
        return false;
      }

      List<Hand> handList = users.First().Value.Hands; // We're going to assume that the first user is the user that should be assigned hand data from a local input provider.
      Frame f = _controller.Frame(0);
      _handsToRemoveBuffer.AddRange(handList);
      foreach (var leapHand in f.Hands) {
        Hand foundHand = handList.Find(h => h.Id == leapHand.Id);
        if (foundHand != null) {
          foundHand.Apply(leapHand, _xform);
        }
        else {
          foundHand = new Hand(leapHand, _xform);
          handList.Add(foundHand);
        }
        _handsToRemoveBuffer.Remove(foundHand); //prevent this hand from being removed
      }

      foreach (var hand in _handsToRemoveBuffer) { //remove all hands that were not added or updated this frame
        handList.RemoveAll(h => h.Id == hand.Id);
      }
      _handsToRemoveBuffer.Clear();
      return true;
    }

    //private void HandleTimerTick(object state) {
    //  if (_controller == null || !_controller.IsConnected) return;
    //  if (!Cursor.IsEmulationEnabled) return;
    //  var f = _controller.Frame(0);

    //  if (f.Hands.Count <= 0) {
    //    if (Cursor.IsButtonDown) {
    //      Cursor.SetMouseButtonDown(false);
    //    }

    //    if (_hasClicked) {
    //      _hasClicked = false;
    //    }
    //  }
    //  else {
    //    var hand = f.Hands.First();

    //    //position
    //    var p = _xform.TransformPoint(hand.PalmPosition);
    //    _settings.NormalizedPosition(p, out var horizontal, out var vertical);
    //    var pixelX = Cursor.PixelX(horizontal);
    //    var pixelY = Cursor.PixelY(vertical);
    //    Cursor.SetPosition(pixelX, pixelY);

    //    //click
    //    var isGrabbing = hand.GrabStrength > _settings.GrabClickThreshold;
    //    Cursor.SetMouseDownConfidence(hand.GrabStrength);
    //    var hoverState = Cursor.GetHoverState();
    //    if (isGrabbing && !Cursor.IsButtonDown) {
    //      if (hoverState == HoverStates.Click) {
    //        if (!_hasClicked) {
    //          _hasClicked = true;
    //          Cursor.DoClick();
    //        }
    //        else {

    //        }
    //      }
    //      else {
    //        Cursor.SetMouseButtonDown(true);
    //      }
    //    }
    //    else if (!isGrabbing && Cursor.IsButtonDown) {
    //      Cursor.SetMouseButtonDown(false);
    //    }

    //    if (!isGrabbing && _hasClicked) {
    //      _hasClicked = false;
    //    }
    //  }
    //}
  }
}
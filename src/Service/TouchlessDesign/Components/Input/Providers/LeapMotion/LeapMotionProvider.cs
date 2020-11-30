using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Navigation;
using Leap;
using TouchlessDesign.Config;

namespace TouchlessDesign.Components.Input.Providers.LeapMotion
{
  public class LeapMotionProvider : IInputProvider
  {

    private class InvalidHand
    {
      public InvalidHand(Hand hand)
      {
        this.hand = hand;
        this.TimeValidated = 0;
      }

      public Hand hand { get; }
      public float TimeValidated { get; set; }
    }

    private const float MillimetersToMeters = 0.001f;

    public string DataDir { get; set; }

    private LeapTransform _xform;
    private Controller _controller;

    private readonly List<Hand> _handsToRemoveBuffer = new List<Hand>();

    private readonly Dictionary<int, InvalidHand> _handsAwaitingValidation = new Dictionary<int, InvalidHand>();

    private float _timeSinceHandLost;

    private ConfigInput _settings;

    private bool _verificationEnabled;

    public void Start()
    {
      LoadHandVerification();
      _xform = new LeapTransform(Vector.Zero, LeapQuaternion.Identity, new Vector(MillimetersToMeters, MillimetersToMeters, MillimetersToMeters));
      _xform.MirrorZ();
      _controller = new Controller();
      _controller.Connect += HandleLeapConnected;
      _controller.Disconnect += HandleLeapDisconnected;
      _controller.StartConnection();
    }

    private void LoadHandVerification()
    {
      _settings = ConfigInput.Get(DataDir);
      _verificationEnabled = !ConfigGeneral.Get(DataDir).RemoteProviderMode;
      if (!_verificationEnabled)
      {
        Console.WriteLine("Warning: Hand verification will not work while running in remote provider mode.");
      }
      else
      {
        Console.WriteLine($"Hand verification time set to {_settings.VerificationDuration_ms}");
        Console.WriteLine($"Hand verification velocity threshold time set to {_settings.MaxVerificationHandVelocity}");
        Console.WriteLine($"Hand verification timeout set to {_settings.VerificationTimeout_ms}");
      }
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
      Log.Info("Leap Connected.");
    }

    private void HandleLeapDisconnected(object sender, ConnectionLostEventArgs e)
    {
      Log.Info("Leap Disconnected");
    }

    public bool Update(Dictionary<int, Hand> hands)
    {
      if (_controller == null || !_controller.IsConnected)
      {
        return false;
      }
      var f = _controller.Frame(0);
      float frameDelta = (float)((_controller.Now() - _controller.FrameTimestamp(1)) / 1E+6);
      _handsToRemoveBuffer.AddRange(hands.Values);
      _timeSinceHandLost += frameDelta;

      foreach (var leapHand in f.Hands)
      {
        // Update captured (but invalid) hands
        if (_verificationEnabled && _handsAwaitingValidation.TryGetValue(leapHand.Id, out var InvalidHand))
        {
          float PalmVelocity = leapHand.PalmVelocity.Magnitude * MillimetersToMeters;
          if (PalmVelocity <= _settings.MaxVerificationHandVelocity)
          {
            InvalidHand.TimeValidated += frameDelta;
            if (InvalidHand.TimeValidated >= _settings.VerificationDuration_ms / 1000f)
            {
              hands.Add(leapHand.Id, InvalidHand.hand);
              _handsAwaitingValidation.Remove(leapHand.Id);
            }
          }
          else
          {
            InvalidHand.TimeValidated = 0;
          }
          _handsToRemoveBuffer.Remove(InvalidHand.hand); //prevent this hand from being removed
        }
        // Update Valid Hands
        else if (hands.TryGetValue(leapHand.Id, out var hand))
        {
          hand.Apply(leapHand, _xform);
          _handsToRemoveBuffer.Remove(hand); //prevent this hand from being removed
          _timeSinceHandLost = 0;
        }
        // Create a new hand instance, putting into "valid hand" container immediately if we haven't timed out. Otherwise, send to the hand validation queue.
        else
        {
          hand = new Hand(leapHand, _xform);
          if (!_verificationEnabled || (_timeSinceHandLost < _settings.VerificationTimeout_ms / 1000.0f))
          {
            hands.Add(leapHand.Id, hand);
            _timeSinceHandLost = 0;
          }
          else
          {
            _handsAwaitingValidation.Add(leapHand.Id, new InvalidHand(hand));
          }
        }
      }

      foreach (var hand in _handsToRemoveBuffer)
      { //remove all hands that were not added or updated this frame
        hands.Remove(hand.Id);
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
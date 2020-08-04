using System.Linq;
using System.Threading;
using Leap;
using TouchlessDesign;

namespace Providers.LeapMotion {
  public class LeapMotionProvider : IInputProvider {

    private const float MillimetersToMeters = 0.001f;

    public string DataDir { get; set; }

    public ICursor Cursor { get; set; }

    public IHotspotCollection Hotspots { get; set; }

    private LeapSettings _settings;
    private LeapTransform _xform;
    private Controller _controller;
    private Timer _timer;

    public void Start() {
      _settings = LeapSettings.Get(DataDir);
      _xform = new LeapTransform(Vector.Zero, LeapQuaternion.Identity, new Vector(MillimetersToMeters, MillimetersToMeters, MillimetersToMeters));
      _xform.MirrorZ();
      _controller = new Controller();
      _controller.Connect += HandleLeapConnected;
      _controller.Disconnect += HandleLeapDisconnected;
      _controller.StartConnection();

      var tickRate = _settings.UpdateRate_ms;
      if (tickRate <= 0) {
        tickRate = LeapSettings.Defaults().UpdateRate_ms;
      }
      _timer = new Timer(HandleTimerTick, null, 0, tickRate);
    }

    public void Stop() {
      _controller.StopConnection();
      _controller.Connect -= HandleLeapConnected;
      _controller.Disconnect -= HandleLeapDisconnected;
      _controller = null;
      _timer.Dispose();
      _timer = null;
    }

    private void HandleLeapConnected(object sender, ConnectionEventArgs e) {
      Log.Info("Leap Connected.");
    }

    private void HandleLeapDisconnected(object sender, ConnectionLostEventArgs e) {
      Log.Info("Leap Disconnected");
    }

    private void HandleTimerTick(object state) {
      if (_controller == null || !_controller.IsConnected) return;
      if (!Cursor.IsEmulationEnabled) return;
      var f = _controller.Frame(0);
      if (f.Hands.Count <= 0) {
        if (Cursor.IsButtonDown) {
          Cursor.SetMouseButtonDown(false);
        }
      }
      else {
        var hand = f.Hands.First();

        //position
        var p = _xform.TransformPoint(hand.PalmPosition);
        _settings.NormalizedPosition(p, out var horizontal, out var vertical);
        var pixelX = Cursor.PixelX(horizontal);
        var pixelY = Cursor.PixelY(vertical);
        Cursor.SetPosition(pixelX, pixelY);

        //click
        var isGrabbing = hand.GrabStrength > _settings.GrabClickThreshold;
        if (isGrabbing && !Cursor.IsButtonDown) {
          Cursor.SetMouseButtonDown(true);
        }
        else if (!isGrabbing && Cursor.IsButtonDown) {
          Cursor.SetMouseButtonDown(false);
        }
      }
    }
  }
}
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TouchlessDesign.Components.Ipc.Networking;
using TouchlessDesign.Config;
using Timer = System.Threading.Timer;

namespace TouchlessDesign.Components.Input {

  /// <summary>
  /// A TouchlessUser is essentially a grouping of TCP clients based on the IP Address of
  /// an expected remote provider, along with the input data that's recieved for that client. 
  /// </summary>
  public class TouchlessUser {

    public TouchlessUser(int deviceID, string ipAddress) {
      RemoteUserInfo = new RemoteUserInfo() { DeviceId = deviceID, IpAddress = ipAddress };
      Hands = new List<Hand>();
      _clickTimer = new Timer(HandleClickCallback, null, Timeout.Infinite, Timeout.Infinite);
    }

    public TouchlessUser(int deviceID, string ipAddress, Client client) : this(deviceID, ipAddress) {
      Client = client;
    }

    ~TouchlessUser() {
      if (Client != null) {
        Client.Dispose();
      }
      _clickTimer.Dispose();
    }

    public event Action<bool> ClickCallback;
    public event Action<bool> MouseButtonStateSet;
    //[JsonIgnore]
    public RemoteUserInfo RemoteUserInfo;

    /// <summary>
    /// Current hover state of this user
    /// </summary>
    
    public Property<HoverStates> HoverState = new Property<HoverStates>();

    /// <summary>
    /// Recorded hands for this user.
    /// </summary>
    //[JsonProperty("Hands")]
    public List<Hand> Hands;

    /// <summary>
    /// The number of hands this user is tracking.
    /// </summary>
    //[JsonProperty("HandCount")]
    public int HandCount;

    /// <summary>
    /// Whether or not this user's grab threshold has been met (for dragging or clicking)
    /// </summary>

    public Property<bool> IsButtonDown = new Property<bool>();

    /// <summary>
    /// Rectangle described in normalized coordinates the bounding box for the client's position bounds
    /// </summary>
    public Rectangle Bounds;

    /// <summary>
    /// The TCP client associated with this touchless user.
    /// </summary>
    public Client Client;

    public int ScreenX;
    public int ScreenY;

    private Timer _clickTimer;

    public bool IsClicking;
    public bool HasClicked;
    public bool InitialPress;
    public bool InitialRelease;
    public double MouseDownConfidence;


    public void SetMouseDownConfidence(double value) {
      MouseDownConfidence = value;
    }

    public void SetMouseButtonDown(bool isDown) {
      if (IsButtonDown.Value == isDown) return;
      IsButtonDown.Value = isDown;
      if (!isDown) {
        InitialRelease = true;
      }
      else {
        InitialPress = true;
      }
      if (IsClicking && !isDown) {
        StopClickCountdown();
      }
      MouseButtonStateSet?.Invoke(isDown);
    }

    public void DoClick() {
      //if (!Config.Input.ClickEnabled) return;
      if (IsClicking) return;
      SetMouseButtonDown(true);
      StartClickCountdown(AppComponent.Config.Input.ClickDuration_ms);
    }

    public void StopClickCountdown() {
      _clickTimer.Change(Timeout.Infinite, Timeout.Infinite);
      IsClicking = false;
    }

    public void StartClickCountdown(int clickDurationMs) {
      IsClicking = true;
      _clickTimer.Change(clickDurationMs, clickDurationMs);
    }

    private void HandleClickCallback(object state) {
      if (!IsClicking) return;
      StopClickCountdown();
      SetMouseButtonDown(false);
      ClickCallback?.Invoke(false);
    }
  }
}

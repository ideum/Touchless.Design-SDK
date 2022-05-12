﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
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
    }

    public TouchlessUser(int deviceID, string ipAddress, Client client) : this(deviceID, ipAddress) {
      Client = client;
    }

    ~TouchlessUser() {
      if (Client != null) {
        Client.Dispose();
      }
      if (_clickTimer != null) {
        _clickTimer.Dispose();
      }
    }


    public RemoteUserInfo RemoteUserInfo;

    /// <summary>
    /// Current hover state of this user
    /// </summary>
    public Property<HoverStates> HoverState = new Property<HoverStates>();

    /// <summary>
    /// Recorded hands for this user.
    /// </summary>
    public List<Hand> Hands;

    /// <summary>
    /// The number of hands this user is tracking.
    /// </summary>
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

    public Timer _clickTimer;

    private bool _isClicking;
    private bool _hasClicked;


  }
}

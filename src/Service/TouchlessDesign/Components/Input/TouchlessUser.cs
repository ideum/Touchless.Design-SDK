using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouchlessDesign.Components.Ipc.Networking;
using TouchlessDesign.Config;

namespace TouchlessDesign.Components.Input {

  /// <summary>
  /// A TouchlessUser is essentially a grouping of TCP clients based on the IP Address of
  /// an expected remote provider, along with the input data that's recieved for that client. 
  /// </summary>
  public class TouchlessUser {

    public RemoteUserInfo RemoteUserInfo;

    /// <summary>
    /// Current hover state of this user
    /// </summary>
    public HoverStates HoverState;

    /// <summary>
    /// Recorded hands for this user.
    /// </summary>
    public List<Hand> Hands;

    /// <summary>
    /// Whether or not this user's grab threshold has been met (for dragging or clicking)
    /// </summary>
    public bool IsButtonDown;

    /// <summary>
    /// Rectangle described in normalized coordinates the bounding box for 
    /// </summary>
    public Rectangle Bounds;

    /// <summary>
    /// The TCP client associated with this touchless user.
    /// </summary>
    public Client Client;
  }
}

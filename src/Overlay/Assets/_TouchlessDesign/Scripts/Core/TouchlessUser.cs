using Ideum.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Ideum {
  [System.Serializable]
  public class TouchlessUser {

    public event Action<HoverStates> HoverStateChanged;

    /// <summary>
    /// The device ID of this touchless user.
    /// </summary>
    public int DeviceId;

    /// <summary>
    /// HoverState
    /// </summary>
    public HoverStates HoverState;

    /// <summary>
    /// The number of hands this user is tracking.
    /// </summary>
    public int HandCount { get; private set; }

    /// <summary>
    /// Rectangle described in normalized coordinates the bounding box for the client's position bounds.
    /// </summary>
    public Rect Bounds { get; private set; }

    /// <summary>
    /// Whether or not user pointer is "down"
    /// </summary>
    public bool IsButtonDown { get; private set; }

    /// <summary>
    /// Whether or not the user has changed it's pointer state to "down" in the last update
    /// </summary>
    public bool InitialPress;

    /// <summary>
    /// Whether or not the user has changed its pointer state to "up" in the last update
    /// </summary>
    public bool InitialRelease;

    /// <summary>
    /// Previous screen position of this touchless user
    /// </summary>
    public int PrevX;
    public int PrevY;

    /// <summary>
    /// The screen position of the primary hand of this user.
    /// </summary>
    public int X { get; private set; }
    public int Y { get; private set; }

    /// <summary>
    /// Whether or not mouse emulation is enabled for this user
    /// </summary>
    public bool MouseEmulationEnabled { get; private set; }


    private Vector2 _screenPosition = new Vector2();
    public Vector3 ScreenPosition { get { return _screenPosition; } }

    /// <summary>
    /// This should be called every time data is updated from a <see cref="Ideum.Data.Msg.Types.UserUpdate"/> message
    /// </summary>
    public void Update(Msg msg) {
      PrevX = X;
      PrevY = Y;
      X = msg.X ?? -1;
      Y = msg.Y ?? -1;
      _screenPosition.x = X;
      _screenPosition.y = Y;
      HandCount = msg.HandCount;

      InitialPress = !IsButtonDown && msg.IsClicking;
      InitialRelease = IsButtonDown && !msg.IsClicking;
      
      IsButtonDown = msg.IsClicking;

      //if(IsButtonDown && !InitialPress)
      //  InitialPress = true;

      //if(!IsButtonDown && !InitialRelease)
      //  InitialRelease = msg.IsClicking;

      HoverState = msg.HoverState;
    }

  }
}

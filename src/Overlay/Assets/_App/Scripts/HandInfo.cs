using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ideum {

  public enum HandState {
    Point,
    Select,
    Close
  }

  public class HandInfo {

    public HandState State;
    public bool SensorHighlighted;
    public bool MovementVectorShown;
    public bool Animating;
    public List<HandState> AnimationStates;

    public HandInfo() {
      State = HandState.Point;
      SensorHighlighted = false;
      MovementVectorShown = false;
      Animating = false;
      AnimationStates = new List<HandState>();
    }
  }
}

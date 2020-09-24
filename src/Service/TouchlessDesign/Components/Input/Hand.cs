using Leap;

namespace TouchlessDesign.Components.Input {
  public class Hand {

    /// <summary>
    /// The unique id of this hand
    /// </summary>
    public int Id;

    /// <summary>
    /// X-Coordinate of the hand
    /// </summary>
    public float X;

    /// <summary>
    /// Y-Coordinate of the hand
    /// </summary>
    public float Y;

    /// <summary>
    /// Z-Coordinate of the hand
    /// </summary>
    public float Z;

    /// <summary>
    /// Value between 0 and 1 inclusive, indicating that the hand is closed or open
    /// </summary>
    public float GrabStrength;

    /// <summary>
    /// The confidence level of the hand, value between 0 and 1 inclusive.
    /// </summary>
    public float Confidence;

    public Hand() {

    }

    public Hand(Leap.Hand hand, Leap.LeapTransform? xform = null) {
      Apply(hand, xform);
    }

    public void Apply(Leap.Hand hand, Leap.LeapTransform? xform = null) {
      Id = hand.Id;
      GrabStrength = hand.GrabStrength;
      Confidence = hand.Confidence;
      var p = xform?.TransformPoint(hand.PalmPosition) ?? hand.PalmPosition;
      X = p.x;
      Y = p.y;
      Z = p.z;
    }
  }
}
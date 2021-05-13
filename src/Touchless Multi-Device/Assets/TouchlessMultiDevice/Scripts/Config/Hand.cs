
namespace TouchlessDesignCore.Components.Input
{
  public class Hand
  {

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

    public Hand()
    {

    }
  }
}
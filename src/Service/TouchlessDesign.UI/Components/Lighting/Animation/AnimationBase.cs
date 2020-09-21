namespace TouchlessDesign.LEDs.Animation {
  public abstract class AnimationBase : IAnimation {

    public int Duration_ms { get; protected set; }
    public int UpdateInterval_ms { get; protected set; }

    public abstract void GetPixels(int time_ms, Color[] array);
  }
}
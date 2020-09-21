namespace TouchlessDesign.LEDs.Animation {
  public interface IAnimation {

    int Duration_ms { get; }
    int UpdateInterval_ms { get; }

    void GetPixels(int time_ms, Color[] array);
  }
}
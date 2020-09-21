namespace TouchlessDesign.LEDs.Animation {
  public class SolidColorAnimation : AnimationBase {

    public static SolidColorAnimation Black { get; } = new SolidColorAnimation(Color.Black,1, 1);

    private readonly Color _color;

    public SolidColorAnimation(Color color, int duration, int updateInterval) {
      _color = new Color(color);
      Duration_ms = duration;
      UpdateInterval_ms = updateInterval;
    }

    public SolidColorAnimation(byte r, byte g, byte b, int duration, int updateInterval) {
      _color = new Color(r, g, b);
      Duration_ms = duration;
      UpdateInterval_ms = updateInterval;
    }

    public override void GetPixels(int time_ms, Color[] array) {
      for (var i = 0; i < array.Length; i++) {
        array[i] = _color;
      }
    }
  }
}
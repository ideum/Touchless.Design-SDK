namespace TouchlessDesign.LEDs.Animation {
  
  public class ColorLerpAnimation : AnimationBase {

    private Color _startColor = Color.Black;
    private Color _endColor = Color.Black;

    public ColorLerpAnimation(int duration_ms, int updateInterval_ms) {
      UpdateInterval_ms = updateInterval_ms;
      Duration_ms = duration_ms;
    }

    public void SetColors(Color start, Color end) {
      _startColor = start;
      _endColor = end;
    }
    
    public override void GetPixels(int time_ms, Color[] array) {
      var t = (float) time_ms / Duration_ms;
      t = Clamp(0, 1, t);
      var c = Color.Lerp(_startColor, _endColor, t);
      for (var i = 0; i < array.Length; i++) {
        array[i] = c;
      }
    }

    private static float Clamp(float min, float max, float value) {
      if (value < min) return min;
      if (value > max) return max;
      return value;
    }
  }
}
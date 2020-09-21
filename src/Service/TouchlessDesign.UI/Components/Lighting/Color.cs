namespace TouchlessDesign.LEDs {
  public struct Color {

    public static Color Zero { get; } = new Color(0, 0, 0);

    public static Color Black { get; } = new Color(1, 1, 1);

    public static Color White { get; } = new Color(255, 255, 255);

    public static Color Red { get; } = new Color(255, 1, 1);

    public static Color Green { get; } = new Color(1, 255, 1);

    public static Color Blue { get; } = new Color(1, 1, 255);

    public byte R, G, B;

    public Color(byte r, byte g, byte b) {
      R = r;
      G = g;
      B = b;
    }

    public Color(Color color) {
      R = color.R;
      G = color.G;
      B = color.B;
    }

    /// <summary>
    /// Prevents any value from dropping under 1 to prevent weirdness.
    /// </summary>
    public void Validate() {
      var c = this;
      R = Clamp(1, byte.MaxValue, c.R);
      G = Clamp(1, byte.MaxValue, c.G);
      B = Clamp(1, byte.MaxValue, c.B);
    }

    public void Set(byte r, byte g, byte b) {
      R = r;
      G = g;
      B = b;
    }

    public void Set(Color c) {
      R = c.R;
      G = c.G;
      B = c.B;
    }

    public void Scale(float t, bool useValidColors = true) {
      var zeroValue = useValidColors ? Black : Zero;
      t = Clamp(0, 1, t);
      var c = Lerp(zeroValue, this, t);
      Set(c);
    }
    
    public static Color Lerp(Color a, Color b, float t) {
      var x = Lerp(a.R, b.R, t);
      var y = Lerp(a.G, b.G, t);
      var z = Lerp(a.B, b.B, t);
      return new Color(x, y, z);
    }

    public static Color Clamp(byte min, byte max, Color c) {
      var x = Clamp(min, max, c.R);
      var y = Clamp(min, max, c.G);
      var z = Clamp(min, max, c.B);
      return new Color(x, y, z);
    }

    private static byte Lerp(byte a, byte b, float t) {
      var af = (float)a;
      var bf = (float)b;
      var cf = af + (bf - af) * t;
      return (byte)Clamp((float)byte.MinValue, (float)byte.MaxValue, cf);
    }

    private static float Clamp(float min, float max, float value) {
      if (value < min) return min;
      if (value > max) return max;
      return value;
    }

    private static byte Clamp(byte min, byte max, byte value) {
      if (value < min) return min;
      if (value > max) return max;
      return value;
    }
  }
}
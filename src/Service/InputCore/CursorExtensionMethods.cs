using System;

namespace TouchlessDesign {
  public static class CursorExtensionMethods {

    public static int PixelX(this ICursor cursor, float normalizedX) {
      return (int) Math.Round(normalizedX * cursor.BoundsWidth + cursor.BoundsLeft);
    }

    public static int PixelY(this ICursor cursor, float normalizedY) {
      return (int) Math.Round(normalizedY * cursor.BoundsHeight + cursor.BoundsTop);
    }
  }
}
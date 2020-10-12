using System.Windows.Forms;
using TouchlessDesign.Components.Ui.ViewModels;
using TouchlessDesign.Config;

namespace TouchlessDesign {
  public static class ExtensionMethods {
    public static bool IsEqual(this Screen screen, DisplayInfo info) {
      return Compare(info, screen);
    }

    public static bool IsEqual(this DisplayInfo info, Screen screen) {
      return Compare(info, screen);
    }

    public static bool IsEqual(this ScreenViewModel a, ScreenViewModel b) {
      return Compare(a.Screen, b.Screen);
    }

    public static bool IsEqual(this Screen a, Screen b) {
      return Compare(a, b);
    }

    public static bool IsEqual(this ScreenViewModel screen, DisplayInfo info) {
      return IsEqual(screen.Screen, info);
    }

    public static bool IsEqual(this DisplayInfo info, ScreenViewModel screen) {
      return IsEqual(screen.Screen, info);
    }

    public static bool IsEqual(this ScreenViewModel screen, Screen s) {
      return IsEqual(screen.Screen, s);
    }

    public static bool IsEqual(this Screen s, ScreenViewModel screen) {
      return IsEqual(screen.Screen, s);
    }

    public static bool Compare(DisplayInfo info, Screen screen) {
      return screen.Bounds.Width == info.Width && screen.Bounds.Height == info.Height && screen.Primary == info.Primary;
    }

    public static bool Compare(Screen a, Screen b) {
      var ab = a.Bounds;
      var bb = b.Bounds;
      return ab.Width == bb.Width && ab.Height == bb.Height && a.Primary == b.Primary;
    }

  }
}
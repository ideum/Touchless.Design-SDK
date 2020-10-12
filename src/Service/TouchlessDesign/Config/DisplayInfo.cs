using System.Windows.Forms;

namespace TouchlessDesign.Config {
  public class DisplayInfo {
    public string Name;
    public int Width, Height;
    public bool Primary;
    public int Index;

    public DisplayInfo() { }

    public DisplayInfo(Screen screen, int index = -1) {
      Name = screen.DeviceName;
      Width = screen.Bounds.Width;
      Height = screen.Bounds.Height;
      Primary = screen.Primary;
      Index = index;
    }

  }
}
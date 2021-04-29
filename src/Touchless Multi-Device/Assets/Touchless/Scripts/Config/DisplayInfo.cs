
namespace TouchlessDesign.Config
{
  public class DisplayInfo
  {

    public static DisplayInfo PrimaryDisplay { get; } = new DisplayInfo { Primary = true };
    public static DisplayInfo SecondaryDisplay { get; } = new DisplayInfo { Primary = false };

    public string Name;
    public int Width, Height;
    public bool Primary;
    public int Index;

    public DisplayInfo() { }

    //public DisplayInfo(Screen screen, int index = -1)
    //{
    //  Name = screen.DeviceName;
    //  Width = screen.Bounds.Width;
    //  Height = screen.Bounds.Height;
    //  Primary = screen.Primary;
    //  Index = index;
    //}
  }
}
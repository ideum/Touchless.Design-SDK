using System.Windows;
using System.Windows.Forms;

namespace TouchlessDesign.Components.Ui.ViewModels {
  public class ScreenViewModel : DependencyObject {

    public static readonly DependencyProperty IdProperty = DependencyProperty.Register(
      "Id", typeof(int), typeof(ScreenViewModel), new PropertyMetadata(default(int)));

    public int Id {
      get { return (int) GetValue(IdProperty); }
      set { SetValue(IdProperty, value); }
    }

    public static readonly DependencyProperty NameProperty = DependencyProperty.Register(
      "Name", typeof(string), typeof(ScreenViewModel), new PropertyMetadata(default(string)));

    public string Name {
      get { return (string) GetValue(NameProperty); }
      set { SetValue(NameProperty, value); }
    }

    public Screen Screen { get; private set; }

    public void SetScreen(int index, Screen screen) {
      Id = index + 1;
      var s = Screen = screen;
      if (s.Primary) {
        Name = $"{s.Bounds.Width}x{s.Bounds.Height} - Primary";
      }
      else {
        Name = $"{s.Bounds.Width}x{s.Bounds.Height}";
      }
    }

    public override string ToString() {
      return Name;
    }
  }
}
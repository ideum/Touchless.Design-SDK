using System.Windows;

namespace TouchlessDesign.Components.Ui.Converters {
  public class MultiBoolToVisibilityConverter : MultiBoolConverter<Visibility> {

    public MultiBoolToVisibilityConverter() : base(Visibility.Visible, Visibility.Collapsed) {

    }
  }
}
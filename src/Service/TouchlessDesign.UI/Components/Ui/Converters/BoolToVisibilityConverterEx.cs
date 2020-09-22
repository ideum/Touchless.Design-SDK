using System.Windows;

namespace TouchlessDesign.Components.Ui.Converters {
  public class BoolToVisibilityConverterEx : BoolConverter<Visibility> {
    
    public BoolToVisibilityConverterEx() : base(Visibility.Visible, Visibility.Collapsed) { }
  }
}
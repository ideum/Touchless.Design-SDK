using System.Windows.Media;

namespace TouchlessDesign.Components.Ui.Converters {
  public class BoolToBrushConverter : BoolConverter<Brush> {
    public BoolToBrushConverter() : base(new SolidColorBrush(Colors.White), new SolidColorBrush(Colors.Black)) {
    }
  }
}
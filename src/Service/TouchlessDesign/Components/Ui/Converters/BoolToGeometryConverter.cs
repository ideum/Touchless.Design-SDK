using System.Windows.Media;

namespace TouchlessDesign.Components.Ui.Converters {
  public class BoolToGeometryConverter: BoolConverter<Geometry> {
    public BoolToGeometryConverter() : base(null, null) {
    }
  }
}
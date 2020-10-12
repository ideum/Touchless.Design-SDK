using System.Windows;

namespace TouchlessDesign.Components.Ui.Converters {
  public class BoolToFontWeightConverter : BoolConverter<FontWeight> {
    
    public BoolToFontWeightConverter() : base(FontWeight.FromOpenTypeWeight(700), FontWeight.FromOpenTypeWeight(400)) {

    }
  }
}
using ModernWpf.Controls;

namespace TouchlessDesign.Components.Ui.Controls {
  public class DoubleNumberFormatter : INumberBoxNumberFormatter {


    public string FormatDouble(double value) {
      return value.ToString("0.000");
    }

    public double? ParseDouble(string text) {
      if (double.TryParse(text, out var d)) {
        return d;
      }
      return null;
    }
  }
}
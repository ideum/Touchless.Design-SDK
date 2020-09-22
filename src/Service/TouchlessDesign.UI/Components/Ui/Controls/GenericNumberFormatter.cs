using ModernWpf.Controls;

namespace TouchlessDesign.Components.Ui.Controls {
  public class GenericNumberFormatter : INumberBoxNumberFormatter {

    public static GenericNumberFormatter DefaultFormatter { get; } = new GenericNumberFormatter {Format = "0.000"};

    public string Format { get; set; } = "0.000";
     
    public string FormatDouble(double value) {
      return value.ToString(Format);
    }

    public double? ParseDouble(string text) {
      if (double.TryParse(text, out var d)) {
        return d;
      }
      return null;
    }
  }
}
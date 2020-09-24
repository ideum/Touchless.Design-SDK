using System;
using System.Windows.Data;

namespace TouchlessDesign.Components.Ui.Converters {
  public class MultiBoolConverter<T> : IMultiValueConverter {

    public T True { get; set; }

    public T False { get; set; }

    public MultiBoolConverter(T trueValue, T falseValue) {
      True = trueValue;
      False = falseValue;
    }

    public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
      var visible = true;
      foreach (var value in values) {
        if (value is bool b) {
          visible = visible && b;
        }
      }
      return visible ? True : False;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture) {
      throw new NotImplementedException();
    }
  }
}
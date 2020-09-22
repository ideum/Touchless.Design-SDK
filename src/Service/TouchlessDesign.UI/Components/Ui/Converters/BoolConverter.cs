using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace TouchlessDesign.Components.Ui.Converters {
  public class BoolConverter<T> : IValueConverter
  {
    public BoolConverter(T trueValue, T falseValue)
    {
      True = trueValue;
      False = falseValue;
    }

    public T True { get; set; }
    public T False { get; set; }

    public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
      return value is bool b && b ? True : False;
    }

    public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
      return value is T o && EqualityComparer<T>.Default.Equals(o, True);
    }
  }
}
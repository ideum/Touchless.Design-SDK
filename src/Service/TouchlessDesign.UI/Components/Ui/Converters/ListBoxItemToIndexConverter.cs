using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace TouchlessDesign.Components.Ui.Converters {
  public class ListBoxItemToIndexConverter : IValueConverter {

    public int Offset { get; set; } = 1;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
      if (!(value is ListBoxItem item)) return null;
      var itemsContainer = ItemsControl.ItemsControlFromItemContainer(item);
      if (itemsContainer == null) return null;
      var index = itemsContainer.ItemContainerGenerator.IndexFromContainer(item);
      return (index+Offset).ToString();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
      throw new NotImplementedException();
    }
  }
}
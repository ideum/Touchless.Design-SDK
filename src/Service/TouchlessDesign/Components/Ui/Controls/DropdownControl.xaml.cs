using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace TouchlessDesign.Components.Ui.Controls {

  public partial class DropdownControl : UserControl {

    public static readonly DependencyProperty LabelMinWidthProperty = DependencyProperty.Register(
      "LabelMinWidth", typeof(double), typeof(DropdownControl), new PropertyMetadata((object)0.0));

    [TypeConverter(typeof(LengthConverter))]
    public double LabelMinWidth {
      get { return (double)GetValue(LabelMinWidthProperty); }
      set { SetValue(LabelMinWidthProperty, value); }
    }

    public static readonly DependencyProperty ValueMinWidthProperty = DependencyProperty.Register(
      "ValueMinWidth", typeof(double), typeof(DropdownControl), new PropertyMetadata(default(double)));

    public double ValueMinWidth {
      get { return (double)GetValue(ValueMinWidthProperty); }
      set { SetValue(ValueMinWidthProperty, value); }
    }

    public static readonly DependencyProperty IsLabelVisibleProperty = DependencyProperty.Register(
      "IsLabelVisible", typeof(bool), typeof(DropdownControl), new PropertyMetadata(true));

    public bool IsLabelVisible {
      get { return (bool)GetValue(IsLabelVisibleProperty); }
      set { SetValue(IsLabelVisibleProperty, value); }
    }

    public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
      "Header", typeof(string), typeof(DropdownControl), new PropertyMetadata(default(string)));

    public string Header {
      get { return (string)GetValue(HeaderProperty); }
      set { SetValue(HeaderProperty, value); }
    }

    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
      "Value", typeof(object), typeof(DropdownControl), new PropertyMetadata(null));

    public object Value {
      get { return GetValue(ValueProperty); }
      set { SetValue(ValueProperty, value); }
    }

    public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
      "ItemsSource", typeof(IEnumerable), typeof(DropdownControl), new PropertyMetadata(default(IEnumerable)));

    public IEnumerable ItemsSource {
      get { return (IEnumerable) GetValue(ItemsSourceProperty); }
      set { SetValue(ItemsSourceProperty, value); }
    }


    public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register(
      "ItemTemplate", typeof(DataTemplate), typeof(DropdownControl), new PropertyMetadata(default(DataTemplate)));

    public DataTemplate ItemTemplate {
      get { return (DataTemplate) GetValue(ItemTemplateProperty); }
      set { SetValue(ItemTemplateProperty, value); }
    }

    public DropdownControl() {
      InitializeComponent();
    }
  }
}

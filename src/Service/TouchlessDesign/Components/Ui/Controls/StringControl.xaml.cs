using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace TouchlessDesign.Components.Ui.Controls {
  /// <summary>
  /// Interaction logic for StringControl.xaml
  /// </summary>
  public partial class StringControl : UserControl {

    public static readonly DependencyProperty LabelMinWidthProperty = DependencyProperty.Register(
      "LabelMinWidth", typeof(double), typeof(StringControl), new PropertyMetadata((object)0.0));

    [TypeConverter(typeof(LengthConverter))]
    public double LabelMinWidth {
      get { return (double)GetValue(LabelMinWidthProperty); }
      set { SetValue(LabelMinWidthProperty, value); }
    }

    public static readonly DependencyProperty ValueMinWidthProperty = DependencyProperty.Register(
      "ValueMinWidth", typeof(double), typeof(StringControl), new PropertyMetadata(default(double)));

    public double ValueMinWidth {
      get { return (double) GetValue(ValueMinWidthProperty); }
      set { SetValue(ValueMinWidthProperty, value); }
    }

    public static readonly DependencyProperty IsLabelVisibleProperty = DependencyProperty.Register(
      "IsLabelVisible", typeof(bool), typeof(StringControl), new PropertyMetadata(true));

    public bool IsLabelVisible {
      get { return (bool)GetValue(IsLabelVisibleProperty); }
      set { SetValue(IsLabelVisibleProperty, value); }
    }

    public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
      "Header", typeof(string), typeof(StringControl), new PropertyMetadata(default(string)));

    public string Header {
      get { return (string)GetValue(HeaderProperty); }
      set { SetValue(HeaderProperty, value); }
    }

    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
      "Value", typeof(string), typeof(StringControl), new PropertyMetadata(default(string)));

    public string Value {
      get { return (string) GetValue(ValueProperty); }
      set { SetValue(ValueProperty, value); }
    }

    public StringControl() {
      InitializeComponent();
    }
  }
}

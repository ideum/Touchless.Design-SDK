using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace TouchlessDesign.Components.Ui.Controls {
  /// <summary>
  /// Interaction logic for BoolControl.xaml
  /// </summary>
  public partial class BoolControl : UserControl {

    public static readonly DependencyProperty LabelMinWidthProperty = DependencyProperty.Register(
      "LabelMinWidth", typeof(double), typeof(BoolControl), new PropertyMetadata((object)0.0));

    [TypeConverter(typeof(LengthConverter))]
    public double LabelMinWidth {
      get { return (double)GetValue(LabelMinWidthProperty); }
      set { SetValue(LabelMinWidthProperty, value); }
    }

    public static readonly DependencyProperty IsLabelVisibleProperty = DependencyProperty.Register(
      "IsLabelVisible", typeof(bool), typeof(BoolControl), new PropertyMetadata(true));

    public bool IsLabelVisible {
      get { return (bool)GetValue(IsLabelVisibleProperty); }
      set { SetValue(IsLabelVisibleProperty, value); }
    }

    public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
      "Header", typeof(string), typeof(BoolControl), new PropertyMetadata(default(string)));

    public string Header {
      get { return (string)GetValue(HeaderProperty); }
      set { SetValue(HeaderProperty, value); }
    }

    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
      "Value", typeof(bool), typeof(BoolControl), new PropertyMetadata(default(bool)));

    public bool Value {
      get { return (bool)GetValue(ValueProperty); }
      set { SetValue(ValueProperty, value); }
    }

    public static readonly DependencyProperty OnContentProperty = DependencyProperty.Register(
      "OnContent", typeof(object), typeof(BoolControl), new PropertyMetadata("On"));

    public object OnContent {
      get { return (object) GetValue(OnContentProperty); }
      set { SetValue(OnContentProperty, value); }
    }

    public static readonly DependencyProperty OffContentProperty = DependencyProperty.Register(
      "OffContent", typeof(object), typeof(BoolControl), new PropertyMetadata("Off"));

    public object OffContent {
      get { return (object) GetValue(OffContentProperty); }
      set { SetValue(OffContentProperty, value); }
    }

    public BoolControl() {
      InitializeComponent();
    }
  }
}

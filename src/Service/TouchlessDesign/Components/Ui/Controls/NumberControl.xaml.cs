using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using ModernWpf.Controls;

namespace TouchlessDesign.Components.Ui.Controls {
  public partial class NumberControl : UserControl {

    public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
      "Header", typeof(string), typeof(NumberControl), new PropertyMetadata(default(string)));

    public string Header {
      get { return (string) GetValue(HeaderProperty); }
      set { SetValue(HeaderProperty, value); }
    }

    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
      "Value", typeof(double), typeof(NumberControl), new FrameworkPropertyMetadata(0.5, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public double Value {
      get { return (double) GetValue(ValueProperty); }
      set { SetValue(ValueProperty, value); }
    }

    public static readonly DependencyProperty MinProperty = DependencyProperty.Register(
      "Min", typeof(double), typeof(NumberControl), new PropertyMetadata(0.0));

    public double Min {
      get { return (double) GetValue(MinProperty); }
      set { SetValue(MinProperty, value); }
    }

    public static readonly DependencyProperty MaxProperty = DependencyProperty.Register(
      "Max", typeof(double), typeof(NumberControl), new PropertyMetadata(1.0));

    public double Max {
      get { return (double) GetValue(MaxProperty); }
      set { SetValue(MaxProperty, value); }
    }

    public static readonly DependencyProperty SmallChangeProperty = DependencyProperty.Register(
      "SmallChange", typeof(double), typeof(NumberControl), new PropertyMetadata(0.01));

    public double SmallChange {
      get { return (double) GetValue(SmallChangeProperty); }
      set { SetValue(SmallChangeProperty, value); }
    }

    public static readonly DependencyProperty LargeChangeProperty = DependencyProperty.Register(
      "LargeChange", typeof(double), typeof(NumberControl), new PropertyMetadata(0.1));

    public double LargeChange {
      get { return (double) GetValue(LargeChangeProperty); }
      set { SetValue(LargeChangeProperty, value); }
    }

    public static readonly DependencyProperty ShowSliderProperty = DependencyProperty.Register(
      "ShowSlider", typeof(bool), typeof(NumberControl), new PropertyMetadata(true));

    public bool ShowSlider {
      get { return (bool) GetValue(ShowSliderProperty); }
      set { SetValue(ShowSliderProperty, value); }
    }

    public static readonly DependencyProperty NumberFormatterProperty = DependencyProperty.Register(
      "NumberFormatter", typeof(INumberBoxNumberFormatter), typeof(NumberControl), new PropertyMetadata(GenericNumberFormatter.DefaultFormatter));

    public INumberBoxNumberFormatter NumberFormatter {
      get { return (INumberBoxNumberFormatter)GetValue(NumberFormatterProperty); }
      set {
        if (value == null) {
          value = GenericNumberFormatter.DefaultFormatter;
        }
        SetValue(NumberFormatterProperty, value);
      }
    }

    public static readonly DependencyProperty LabelMinWidthProperty = DependencyProperty.Register(
      "LabelMinWidth", typeof(double), typeof(NumberControl), new PropertyMetadata((object)0.0));

    [TypeConverter(typeof(LengthConverter))]
    public double LabelMinWidth {
      get { return (double)GetValue(LabelMinWidthProperty); }
      set { SetValue(LabelMinWidthProperty, value); }
    }

    public static readonly DependencyProperty IsLabelVisibleProperty = DependencyProperty.Register(
      "IsLabelVisible", typeof(bool), typeof(NumberControl), new PropertyMetadata(true));

    public bool IsLabelVisible {
      get { return (bool)GetValue(IsLabelVisibleProperty); }
      set { SetValue(IsLabelVisibleProperty, value); }
    }

    public NumberControl() {
      InitializeComponent();
    }
  }
}

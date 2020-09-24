using System.Windows;
using System.Windows.Controls;
using ModernWpf.Controls;

namespace TouchlessDesign.Components.Ui.Controls {
  /// <summary>
  /// Interaction logic for NumericSliderControl.xaml
  /// </summary>
  public partial class ClampedPropertyControl : UserControl {

    public static readonly DependencyProperty NumberFormatterProperty = DependencyProperty.Register(
      "NumberFormatter", typeof(INumberBoxNumberFormatter), typeof(ClampedPropertyControl), new PropertyMetadata(GenericNumberFormatter.DefaultFormatter));

    public INumberBoxNumberFormatter NumberFormatter {
      get { return (INumberBoxNumberFormatter) GetValue(NumberFormatterProperty); }
      set {
        if (value == null) {
          value = GenericNumberFormatter.DefaultFormatter;
        }
        SetValue(NumberFormatterProperty, value);
      }
    }

    public static readonly DependencyProperty ShowSliderProperty = DependencyProperty.Register(
      "ShowSlider", typeof(bool), typeof(ClampedPropertyControl), new PropertyMetadata(true));

    public bool ShowSlider {
      get { return (bool) GetValue(ShowSliderProperty); }
      set { SetValue(ShowSliderProperty, value); }
    }

    public ClampedPropertyControl() {
      InitializeComponent();
    }
  }
}

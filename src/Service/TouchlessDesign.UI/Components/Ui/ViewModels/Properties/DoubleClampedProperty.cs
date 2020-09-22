using System.Windows;

namespace TouchlessDesign.Components.Ui.ViewModels.Properties {
  public class DoubleClampedProperty : DoubleProperty, IClampedProperty<double> {

    public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register(
      "Minimum", typeof(double), typeof(DoubleClampedProperty), new PropertyMetadata(default(double)));

    public double Minimum {
      get { return (double)GetValue(MinimumProperty); }
      set {
        SetValue(MinimumProperty, value);
        OnPropertyChanged();
      }
    }

    public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(
      "Maximum", typeof(double), typeof(DoubleClampedProperty), new PropertyMetadata(default(double)));

    public double Maximum {
      get { return (double)GetValue(MaximumProperty); }
      set {
        SetValue(MaximumProperty, value);
        OnPropertyChanged();
      }
    }

    public static readonly DependencyProperty SmallChangeProperty = DependencyProperty.Register(
      "SmallChange", typeof(double), typeof(DoubleClampedProperty), new PropertyMetadata(0.001));

    public double SmallChange {
      get { return (double)GetValue(SmallChangeProperty); }
      set {
        SetValue(SmallChangeProperty, value);
        OnPropertyChanged();
      }
    }

    public static readonly DependencyProperty LargeChangeProperty = DependencyProperty.Register(
      "LargeChange", typeof(double), typeof(DoubleClampedProperty), new PropertyMetadata(0.1));

    public double LargeChange {
      get { return (double)GetValue(LargeChangeProperty); }
      set {
        SetValue(LargeChangeProperty, value);
        OnPropertyChanged();
      }
    }
  }
}
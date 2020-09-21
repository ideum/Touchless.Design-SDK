using System.Windows;

namespace TouchlessDesign.Components.Ui.ViewModels {
  public class DoubleProperty : PropertyBase<double> {

    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
      "Value", typeof(double), typeof(DoubleProperty), new PropertyMetadata(default(double), ValuePropertyChangedCallback));

    public override double Value {
      get { return (double) GetValue(ValueProperty); }
      set { SetValue(ValueProperty, value); }
    }
  }

  public class ClampedDoubleProperty : DoubleProperty {

    public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register(
      "Minimum", typeof(double), typeof(ClampedDoubleProperty), new PropertyMetadata(default(double)));

    public double Minimum {
      get { return (double)GetValue(MinimumProperty); }
      set { SetValue(MinimumProperty, value); }
    }

    public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(
      "Maximum", typeof(double), typeof(ClampedDoubleProperty), new PropertyMetadata(default(double)));

    public double Maximum {
      get { return (double)GetValue(MaximumProperty); }
      set { SetValue(MaximumProperty, value); }
    }

    public static readonly DependencyProperty SmallChangeProperty = DependencyProperty.Register(
      "SmallChange", typeof(double), typeof(ClampedDoubleProperty), new PropertyMetadata(0.001));

    public double SmallChange {
      get { return (double)GetValue(SmallChangeProperty); }
      set { SetValue(SmallChangeProperty, value); }
    }

    public static readonly DependencyProperty LargeChangeProperty = DependencyProperty.Register(
      "LargeChange", typeof(double), typeof(ClampedDoubleProperty), new PropertyMetadata(0.1));

    public double LargeChange {
      get { return (double)GetValue(LargeChangeProperty); }
      set { SetValue(LargeChangeProperty, value); }
    }
  }
}
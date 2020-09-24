using System.Windows;

namespace TouchlessDesign.Components.Ui.ViewModels.Properties {
  public class DoubleProperty : PropertyBase<double> {

    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
      "Value", typeof(double), typeof(DoubleProperty), new PropertyMetadata(default(double), ValuePropertyChangedCallback));

    public override double Value {
      get { return (double) GetValue(ValueProperty); }
      set {
        SetValue(ValueProperty, value);
        OnPropertyChanged();
      }
    }
  }
}
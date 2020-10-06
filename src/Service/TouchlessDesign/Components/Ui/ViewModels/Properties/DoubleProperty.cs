using System.Windows;

namespace TouchlessDesign.Components.Ui.ViewModels.Properties {
  public class DoubleProperty : PropertyBase<double> {

    public static readonly DependencyProperty PropProperty = DependencyProperty.Register(
      "Prop", typeof(double), typeof(DoubleProperty), new PropertyMetadata(default(double), ValuePropertyChangedCallback));

    public override double Prop {
      get { return (double) GetValue(PropProperty); }
      set {
        SetValue(PropProperty, value);
        OnPropertyChanged();
      }
    }
  }
}
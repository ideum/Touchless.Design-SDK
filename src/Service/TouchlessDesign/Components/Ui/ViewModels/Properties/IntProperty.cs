using System.Windows;

namespace TouchlessDesign.Components.Ui.ViewModels.Properties {
  public class IntProperty : PropertyBase<int> {

    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
      "Value", typeof(int), typeof(IntProperty), new PropertyMetadata(default(int), ValuePropertyChangedCallback));

    public override int Value {
      get { return (int) GetValue(ValueProperty); }
      set {
        SetValue(ValueProperty, value);
        OnPropertyChanged();
      }
    }
  }
}
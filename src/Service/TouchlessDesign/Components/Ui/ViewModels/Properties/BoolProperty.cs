using System.Windows;

namespace TouchlessDesign.Components.Ui.ViewModels.Properties {
  public class BoolProperty : PropertyBase<bool> {

    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
      "Value", typeof(bool), typeof(BoolProperty), new PropertyMetadata(default(bool), ValuePropertyChangedCallback));

    public override bool Value {
      get { return (bool) GetValue(ValueProperty); }
      set { SetValue(ValueProperty, value); }
    }
  }
}
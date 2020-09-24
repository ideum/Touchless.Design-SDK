using System.Windows;

namespace TouchlessDesign.Components.Ui.ViewModels.Properties {
  public class StringProperty : PropertyBase<string> {

    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
      "Value", typeof(string), typeof(StringProperty), new PropertyMetadata(default(string), ValuePropertyChangedCallback));

    public override string Value {
      get { return (string) GetValue(ValueProperty); }
      set { SetValue(ValueProperty, value); }
    }
  }
}
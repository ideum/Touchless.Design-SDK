using System.Windows;

namespace TouchlessDesign.Components.Ui.ViewModels.Properties {
  public class StringProperty : PropertyBase<string> {

    public static readonly DependencyProperty PropProperty = DependencyProperty.Register(
      "Prop", typeof(string), typeof(StringProperty), new PropertyMetadata(default(string), ValuePropertyChangedCallback));

    public override string Prop {
      get { return (string) GetValue(PropProperty); }
      set { SetValue(PropProperty, value); }
    }
  }
}
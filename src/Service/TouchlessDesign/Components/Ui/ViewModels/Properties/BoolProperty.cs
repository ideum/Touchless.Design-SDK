using System.Windows;

namespace TouchlessDesign.Components.Ui.ViewModels.Properties {
  public class BoolProperty : PropertyBase<bool> {

    public static readonly DependencyProperty PropProperty = DependencyProperty.Register(
      "Prop", typeof(bool), typeof(BoolProperty), new PropertyMetadata(default(bool), ValuePropertyChangedCallback));

    public override bool Prop {
      get { return (bool) GetValue(PropProperty); }
      set {
        SetValue(PropProperty, value); 
      }
    }
  }
}
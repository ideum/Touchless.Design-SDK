using System.Windows;

namespace TouchlessDesign.Components.Ui.ViewModels.Properties {
  public class IntProperty : PropertyBase<int> {

    public static readonly DependencyProperty PropProperty = DependencyProperty.Register(
      "Prop", typeof(int), typeof(IntProperty), new PropertyMetadata(default(int), ValuePropertyChangedCallback));

    public override int Prop {
      get { return (int) GetValue(PropProperty); }
      set {
        SetValue(PropProperty, value);
        OnPropertyChanged();
      }
    }
  }
}
using System.Windows;

namespace TouchlessDesign.Components.Ui.ViewModels.Properties {
  public class IntClampedProperty : IntProperty, IClampedProperty<int> {
    public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register(
      "Minimum", typeof(int), typeof(IntClampedProperty), new PropertyMetadata(int.MinValue));

    public int Minimum {
      get { return (int) GetValue(MinimumProperty); }
      set {
        SetValue(MinimumProperty, value);
        OnPropertyChanged();
      }
    }

    public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(
      "Maximum", typeof(int), typeof(IntClampedProperty), new PropertyMetadata(int.MaxValue));

    public int Maximum {
      get { return (int) GetValue(MaximumProperty); }
      set {
        SetValue(MaximumProperty, value);
        OnPropertyChanged();
      }
    }

    public static readonly DependencyProperty SmallChangeProperty = DependencyProperty.Register(
      "SmallChange", typeof(int), typeof(IntClampedProperty), new PropertyMetadata(1));

    public int SmallChange {
      get { return (int) GetValue(SmallChangeProperty); }
      set {
        SetValue(SmallChangeProperty, value);
        OnPropertyChanged();
      }
    }

    public static readonly DependencyProperty LargeChangeProperty = DependencyProperty.Register(
      "LargeChange", typeof(int), typeof(IntClampedProperty), new PropertyMetadata(10));

    public int LargeChange {
      get { return (int) GetValue(LargeChangeProperty); }
      set {
        SetValue(LargeChangeProperty, value);
        OnPropertyChanged();
      }
    }
  }
}
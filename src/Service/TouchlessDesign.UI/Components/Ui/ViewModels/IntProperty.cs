using System.Windows;

namespace TouchlessDesign.Components.Ui.ViewModels {
  public class IntProperty : PropertyBase<int> {

    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
      "Value", typeof(int), typeof(IntProperty), new PropertyMetadata(default(int), ValuePropertyChangedCallback));

    public override int Value {
      get { return (int) GetValue(ValueProperty); }
      set { SetValue(ValueProperty, value); }
    }
  }

  public class ClampedIntProperty : IntProperty {
    public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register(
      "Minimum", typeof(int), typeof(ClampedIntProperty), new PropertyMetadata(int.MinValue));

    public int Minimum {
      get { return (int) GetValue(MinimumProperty); }
      set { SetValue(MinimumProperty, value); }
    }

    public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(
      "Maximum", typeof(int), typeof(ClampedIntProperty), new PropertyMetadata(int.MaxValue));

    public int Maximum {
      get { return (int) GetValue(MaximumProperty); }
      set { SetValue(MaximumProperty, value); }
    }

    public static readonly DependencyProperty SmallChangeProperty = DependencyProperty.Register(
      "SmallChange", typeof(int), typeof(ClampedIntProperty), new PropertyMetadata(1));

    public int SmallChange {
      get { return (int) GetValue(SmallChangeProperty); }
      set { SetValue(SmallChangeProperty, value); }
    }

    public static readonly DependencyProperty LargeChangeProperty = DependencyProperty.Register(
      "LargeChange", typeof(int), typeof(ClampedIntProperty), new PropertyMetadata(10));

    public int LargeChange {
      get { return (int) GetValue(LargeChangeProperty); }
      set { SetValue(LargeChangeProperty, value); }
    }
  }
}
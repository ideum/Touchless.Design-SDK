using System;
using System.Windows;

namespace TouchlessDesign.Components.Ui.ViewModels.Properties {
  public abstract class PropertyBase : DependencyObject, IProperty {
    
    public event Action<IProperty> Changed;

    protected static void ValuePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      var i = d as PropertyBase;
      i?.InvokeChanged();
    }

    protected void InvokeChanged() {
      Changed?.Invoke(this);
    }


    public static readonly DependencyProperty NameProperty = DependencyProperty.Register(
      "Name", typeof(string), typeof(PropertyBase), new PropertyMetadata(default(string)));

    public string Name {
      get { return (string) GetValue(NameProperty); }
      set { SetValue(NameProperty, value); }
    }

    public static readonly DependencyProperty IsEditableProperty = DependencyProperty.Register(
      "IsEditable", typeof(bool), typeof(PropertyBase), new PropertyMetadata(true));

    public bool IsEditable {
      get { return (bool) GetValue(IsEditableProperty); }
      set { SetValue(IsEditableProperty, value); }
    }
  }

  public abstract class PropertyBase<T> : PropertyBase, IProperty<T> {

    public abstract T Value { get; set; }

  }


}
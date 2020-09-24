using System;
using System.CodeDom;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using TouchlessDesign.Annotations;

namespace TouchlessDesign.Components.Ui.ViewModels.Properties {
  public abstract class PropertyBase : DependencyObject, IProperty {
    
    public event Action<IProperty> Changed;

    public abstract object BaseValue { get; set; }

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
      set {
        SetValue(NameProperty, value);
        OnPropertyChanged();
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }

  public abstract class PropertyBase<T> : PropertyBase, IProperty<T> {
    public override object BaseValue {
      get { return Value; }
      set {
        if (value is T a) {
          Value = a;
        }
      }
    }

    public abstract T Value { get; set; }
  }


}
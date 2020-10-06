using System;
using System.Windows;

namespace TouchlessDesign.Components.Ui.ViewModels.Properties {
  public abstract class PropertyBase : DependencyObject, IProperty {
    
    protected static void ValuePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      var i = d as PropertyBase;
      i?.OnPropertyChanged();
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

    public event Action<IProperty> PropChanged;



    //public event PropertyChangedEventHandler PropertyChanged;

    //protected void DoOnPropertyChanged(string name) {
    //  PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    //}

    //[NotifyPropertyChangedInvocator]
    //protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
    //  PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    //}

    public virtual void OnPropertyChanged() {
      PropChanged?.Invoke(this);
    }
  }

  public abstract class PropertyBase<T> : PropertyBase, IProperty<T> {

    public abstract T Prop { get; set; }
  }
}
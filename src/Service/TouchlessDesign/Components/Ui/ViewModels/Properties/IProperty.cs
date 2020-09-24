using System;
using System.ComponentModel;

namespace TouchlessDesign.Components.Ui.ViewModels.Properties {
  public interface IProperty : INotifyPropertyChanged {

    event Action<IProperty> Changed;

    string Name { get; set; }

    object BaseValue { get; set; }

  }

  public interface IProperty<T> : IProperty {
     T Value { get; set; }
  }

}
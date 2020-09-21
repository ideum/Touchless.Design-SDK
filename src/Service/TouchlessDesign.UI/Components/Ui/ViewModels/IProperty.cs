using System;
using System.Windows;

namespace TouchlessDesign.Components.Ui.ViewModels {
  public interface IProperty {

    event Action<IProperty> Changed;

    string Name { get; set; }

    bool IsEditable { get; set; }

    Visibility Visibility { get; set; }
  }

  public interface IProperty<T> : IProperty {
    T Value { get; set; }
  }

}
using System;
using System.ComponentModel;

namespace TouchlessDesign.Components.Ui.ViewModels.Properties {
  public interface IProperty {

    string Name { get; set; }

    event Action<IProperty> PropChanged;
  }

  public interface IProperty<T> : IProperty {
     T Prop { get; set; }
  }

}
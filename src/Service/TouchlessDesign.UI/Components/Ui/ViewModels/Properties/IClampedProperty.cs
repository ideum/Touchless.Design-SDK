namespace TouchlessDesign.Components.Ui.ViewModels.Properties {
  public interface IClampedProperty<T> : IProperty {
    T Minimum { get; set; }
    T Maximum { get; set; }
    T SmallChange { get; set; }

    T LargeChange { get; set; }
  }
}
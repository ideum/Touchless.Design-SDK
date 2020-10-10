using System.Collections.Generic;
using System.Windows;
using TouchlessDesign.Components.Ui.ViewModels.Properties;
using TouchlessDesign.Config;

namespace TouchlessDesign.Components.Ui.ViewModels {

  public abstract class AppViewModelBase : DependencyObject {


    public BoolProperty NeedsSave { get; } = new BoolProperty {Prop = false, Name = "Needs Save"};

    public BoolProperty NeedsRestart { get; } = new BoolProperty {Prop = false, Name = "Needs Restart"};

    protected AppViewModelBase() {
      AppComponent.OnLoaded(HandleAppComponentLoaded);
    }

    private void HandleAppComponentLoaded() {
      AssignModel();
      UpdateValuesFromModel();
    }

    protected abstract void AssignModel();

    public abstract void SaveChanges();

    public abstract void RevertChanges();

    public virtual void ApplyValuesToModel() {

    }

    public virtual void UpdateValuesFromModel() {

    }
  }
  
  public abstract class AppViewModelBase<TModel> : AppViewModelBase where TModel : IConfig {

    public TModel Model { get; set; }

    protected List<IProperty> SaveProperties { get; } = new List<IProperty>();

    protected List<IProperty> RestartProperties { get; } = new List<IProperty>();

    public override void SaveChanges() {
      ApplyValuesToModel();
      Model.Save();
      NeedsSave.Prop = false;
    }

    public override void RevertChanges() {
      Model.Reload();
      NeedsSave.Prop = false;
      NeedsRestart.Prop = false;
      UpdateValuesFromModel();
    }

    private void HandleSavePropertyChanged(IProperty obj) {
      NeedsSave.Prop = true;
      OnSavePropertyChanged(obj);
    }

    protected virtual void OnSavePropertyChanged(IProperty p) {

    }

    private void HandleRestartPropertyChanged(IProperty obj) {
      NeedsRestart.Prop = true;
      NeedsSave.Prop = true;
      OnRestartPropertyChanged(obj);
    }

    protected virtual void OnRestartPropertyChanged(IProperty p) {

    }

    public void AddSaveProperties(params IProperty[] properties) {
      foreach (var p in properties) {
        AddSaveProperty(p);
      }
    }

    public void AddSaveProperty(IProperty property) {
      if (property == null || SaveProperties.Contains(property)) return;
      SaveProperties.Add(property);
      property.PropChanged += HandleSavePropertyChanged;
    }

    public void RemoveSaveProperty(IProperty property) {
      if (property == null || !SaveProperties.Contains(property)) return;
      SaveProperties.Remove(property);
      property.PropChanged -= HandleSavePropertyChanged;
    }

    public void AddRestartProperties(params IProperty[] properties) {
      foreach (var p in properties) {
        AddRestartProperty(p);
      }
    }

    public void AddRestartProperty(IProperty property) {
      if (property == null || RestartProperties.Contains(property)) return;
      RestartProperties.Add(property);
      property.PropChanged += HandleRestartPropertyChanged;
    }

    public void RemoveRestartProperty(IProperty property) {
      if (property == null || !RestartProperties.Contains(property)) return;
      RestartProperties.Remove(property);
      property.PropChanged -= HandleRestartPropertyChanged;
    }

  }
}
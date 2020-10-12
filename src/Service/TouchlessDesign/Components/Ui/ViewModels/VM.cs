using System;
using System.Windows;
using TouchlessDesign.Config;

namespace TouchlessDesign.Components.Ui.ViewModels {

  public enum PropertyTypes {
    None,
    Save,
    Restart
  }

  public abstract class VM : DependencyObject {

    protected static DependencyProperty Reg<TViewModelType, TDataType>(string name, PropertyTypes type = PropertyTypes.None) where TViewModelType : VM {
      return DependencyProperty.Register(name, typeof(TDataType), typeof(TViewModelType), new FrameworkPropertyMetadata(
        default(TDataType),
        FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
        (d, e) => {
          if (!(d is TViewModelType i)) {
            Log.Error($"Failed to cast to {typeof(TViewModelType).Name}");
            return;
          }
          i.PropertyValueChanged(name, type, e.NewValue);
        })
      );
    }

    protected static DependencyProperty Reg<TViewModelType, TDataType>(string name, TDataType defaultValue, PropertyTypes type = PropertyTypes.None) where TViewModelType : VM {
      return DependencyProperty.Register(name, typeof(TDataType), typeof(TViewModelType), new FrameworkPropertyMetadata(
        defaultValue,
        FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
        (d, e) => {
          if (!(d is TViewModelType i)) {
            Log.Error($"Failed to cast to {typeof(TViewModelType).Name}");
            return;
          }
          i.PropertyValueChanged(name, type, e.NewValue);
        })
      );
    }

    public static readonly DependencyProperty NeedsSaveProperty = Reg<VM, bool>("NeedsSave", false);

    public bool NeedsSave {
      get { return (bool) GetValue(NeedsSaveProperty); }
      set { SetValue(NeedsSaveProperty, value); }
    }


    public static readonly DependencyProperty NeedsRestartProperty = Reg<TestVM, bool>("NeedsRestart", true);

    public bool NeedsRestart {
      get { return (bool)GetValue(NeedsRestartProperty); }
      set { SetValue(NeedsRestartProperty, value); }
    }

    public event Action SavePropertyChanged, ResetPropertyChanged;

    public bool SuppressSaveChanged = false;
    public bool SuppressRestartChanged = false;

    protected VM() {
      AppComponent.OnLoaded(HandleAppComponentLoaded);
    }

    private void HandleAppComponentLoaded() {
      Log.Info($"Assigning Model for {GetType().Name}");
      AssignModel();
      SuppressRestartChanged = true;
      SuppressSaveChanged = true;
      Log.Info($"Updating values for {GetType().Name}");
      UpdateValuesFromModel();
      SuppressRestartChanged = false;
      SuppressSaveChanged = false;
      SavePropertyChanged += UpdateRealTimeProperties;
    }

    protected virtual void AssignModel() {

    }

    public virtual void UpdateRealTimeProperties() {

    }

    public void PropertyValueChanged(string name, PropertyTypes type, object newValue) {
      switch (type) {
        case PropertyTypes.None:
          break;
        case PropertyTypes.Save:
          if (!SuppressSaveChanged) {
            NeedsSave = true;
            SavePropertyChanged?.Invoke();
          }
          break;
        case PropertyTypes.Restart:
          if (!SuppressSaveChanged) {
            NeedsSave = true;
            SavePropertyChanged?.Invoke();
          }
          if (!SuppressRestartChanged) {
            NeedsRestart = true;
            ResetPropertyChanged?.Invoke();
          }
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof(type), type, null);
      }
    }

    public abstract void ApplyValuesToModel();

    public abstract void UpdateValuesFromModel();

    public abstract void SaveChanges();

    public abstract void RevertChanges();

  }

  public abstract class VM<T> : VM where T : IConfig {

    public T Model { get; set; }

    public override void SaveChanges() {
      ApplyValuesToModel();
      Model.Save();
      NeedsSave = false;
    }

    public override void RevertChanges() {
      Model.Reload();
      UpdateValuesFromModel();
      NeedsSave = false;
      NeedsRestart = false;
    }
  }
}
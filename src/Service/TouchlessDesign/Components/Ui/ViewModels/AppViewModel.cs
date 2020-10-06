using System;
using System.Windows;

namespace TouchlessDesign.Components.Ui.ViewModels {
  public class AppViewModel : DependencyObject {

    public static readonly DependencyProperty SaveNeededProperty = DependencyProperty.Register(
      "SaveNeeded", typeof(bool), typeof(AppViewModel), new PropertyMetadata(default(bool)));

    public bool SaveNeeded {
      get { return (bool) GetValue(SaveNeededProperty); }
      set { SetValue(SaveNeededProperty, value); }
    }

    public static readonly DependencyProperty RestartNeededProperty = DependencyProperty.Register(
      "RestartNeeded", typeof(bool), typeof(AppViewModel), new PropertyMetadata(default(bool)));

    public bool RestartNeeded {
      get { return (bool) GetValue(RestartNeededProperty); }
      set { SetValue(RestartNeededProperty, value); }
    }

    public static readonly DependencyProperty InputProperty = DependencyProperty.Register(
      "Input", typeof(InputViewModel), typeof(AppViewModel), new PropertyMetadata(default(InputViewModel)));

    public InputViewModel Input {
      get { return (InputViewModel) GetValue(InputProperty); }
      set { SetValue(InputProperty, value); }
    }

    public static readonly DependencyProperty DisplayProperty = DependencyProperty.Register(
      "Display", typeof(DisplayViewModel), typeof(AppViewModel), new PropertyMetadata(default(DisplayViewModel)));

    public DisplayViewModel Display {
      get { return (DisplayViewModel) GetValue(DisplayProperty); }
      set { SetValue(DisplayProperty, value); }
    }

    public static readonly DependencyProperty GeneralProperty = DependencyProperty.Register(
      "General", typeof(GeneralViewModel), typeof(AppViewModel), new PropertyMetadata(default(GeneralViewModel)));

    public GeneralViewModel General {
      get { return (GeneralViewModel) GetValue(GeneralProperty); }
      set { SetValue(GeneralProperty, value); }
    }

    public static readonly DependencyProperty NetworkProperty = DependencyProperty.Register(
      "Network", typeof(NetworkViewModel), typeof(AppViewModel), new PropertyMetadata(default(NetworkViewModel)));

    public NetworkViewModel Network {
      get { return (NetworkViewModel) GetValue(NetworkProperty); }
      set { SetValue(NetworkProperty, value); }
    }


    public event Action Saved, Reverted; 

    private VM[] _viewModels;

    public AppViewModel() {
      AppComponent.OnLoaded(HandleAppComponentLoaded);
    }

    private void HandleAppComponentLoaded() {
      _viewModels = new VM[] {Input, Display, General, Network};
      foreach (var vm in _viewModels) {
        if (vm == null) continue;
        vm.SavePropertyChanged += SaveNeededChanged;
        vm.ResetPropertyChanged += RestartNeededChanged;
      }
    }

    private void SaveNeededChanged() {
      var sv = false;
      foreach (var vm in _viewModels) {
        if (vm == null) continue;
        sv |= vm.NeedsSave;
      }
      SaveNeeded = sv;
    }

    private void RestartNeededChanged() {
      var rs = false;
      foreach (var vm in _viewModels) {
        if (vm == null) continue;
        rs |= vm.NeedsRestart;
      }
      RestartNeeded = rs;
    }

    public void SaveChanges() {
      foreach (var vm in _viewModels) {
        vm?.SaveChanges();
      }
      Saved?.Invoke();
      SaveNeeded = false;
    }

    public void RevertChanges() {
      foreach (var vm in _viewModels) {
        vm.RevertChanges();
      }
      Reverted?.Invoke();
      SaveNeeded = false;
    }
  }
}
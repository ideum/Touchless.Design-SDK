using System.Windows;
using System.Windows.Controls;
using TouchlessDesign.Components.Ui.ViewModels;

namespace TouchlessDesign.Components.Ui {
  /// <summary>
  /// Interaction logic for PageDisplay.xaml
  /// </summary>
  public partial class PageDisplay : UserControl {

    private bool _hasLoaded;

    public PageDisplay() {
      InitializeComponent();
      Loaded += PageDisplay_Loaded;
    }

    private void PageDisplay_Loaded(object sender, RoutedEventArgs e) {
      DoRefreshDisplays();
      Log.Info("PAGE LOADED");
    }

    private void RefreshDisplaysClicked(object sender, RoutedEventArgs e) {
      DoRefreshDisplays();
    }

    private void DoRefreshDisplays() {
      if (DataContext is DisplayViewModel vm) {
        vm.RefreshDisplays();
      }
    }

    private void FrameworkElement_OnLoaded(object sender, RoutedEventArgs e) {
      Log.Info("CONTROL LOADED");
      if (!_hasLoaded) {
        _hasLoaded = true;
        if (DataContext is DisplayViewModel vm) {
          vm.UpdateValuesFromModel();
        }
      }
    }
  }
}

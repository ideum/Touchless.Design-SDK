using System.Windows;
using System.Windows.Controls;
using TouchlessDesign.Components.Ui.ViewModels;

namespace TouchlessDesign.Components.Ui {
  /// <summary>
  /// Interaction logic for PageDisplay.xaml
  /// </summary>
  public partial class PageDisplay : UserControl {

    public PageDisplay() {
      InitializeComponent();
      Loaded += PageDisplay_Loaded;
    }

    private void PageDisplay_Loaded(object sender, RoutedEventArgs e) {
      DoRefreshDisplays();
    }

    private void DoRefreshDisplays() {
      if (DataContext is DisplayViewModel vm) {
        vm.RefreshDisplays();
      }
    }
  }
}

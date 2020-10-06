using System.Windows;
using System.Windows.Controls;

namespace TouchlessDesign.Components.Ui {

  public partial class PageInput : UserControl {

    public PageInput() {
      InitializeComponent();
    }

    private void OpenNetworkSettingsButtonClicked(object sender, RoutedEventArgs e) {
      MainWindow.Instance.DoSelectNetworkTab();
    }
  }
}

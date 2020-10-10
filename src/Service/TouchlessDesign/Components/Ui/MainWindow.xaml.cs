using System.ComponentModel;
using System.Windows;

namespace TouchlessDesign.Components.Ui {
  public partial class MainWindow : Window {

    public static readonly DependencyProperty SelectNetworkTabProperty = DependencyProperty.Register(
      "SelectNetworkTab", typeof(bool), typeof(MainWindow), new PropertyMetadata(default(bool)));

    public bool SelectNetworkTab {
      get { return (bool) GetValue(SelectNetworkTabProperty); }
      set { SetValue(SelectNetworkTabProperty, value); }
    }

    public static MainWindow Instance { get; private set; }

    private bool _trueClose = false;

    public App App {
      get { return Application.Current as App; }
    }

    public MainWindow() {
      Instance = this;
      InitializeComponent();
    }

    private void HandleRestartClicked(object sender, RoutedEventArgs e) {
      Startup.Restart = true;
      _trueClose = true;
      App.Close();
    }

    private void HandleExitButtonClicked(object sender, RoutedEventArgs e) {
      _trueClose = true;
      App.Close();
    }

    private void MainWindow_OnClosing(object sender, CancelEventArgs e) {
      if (_trueClose) return;
      e.Cancel = true;
      HideWindow();
    }

    public void DoSelectNetworkTab() {
      SelectNetworkTab = true;
    }

    public void ShowWindow() {
      App.StatusViewModel.UpdateValues();
      App.StatusViewModel.Start();
      Show();
    }

    public void HideWindow() {
      App.StatusViewModel.Stop();
      Hide();
    }

    private void HandleRevertButtonClicked(object sender, RoutedEventArgs e) {
      App.AppViewModel.RevertChanges();
    }

    private void HandleSaveButtonClicked(object sender, RoutedEventArgs e) {
      App.AppViewModel.SaveChanges();
    }
  }
}

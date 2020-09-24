using System.ComponentModel;
using System.Windows;

namespace TouchlessDesign.Components.Ui {
  public partial class MainWindow : Window {

    private bool _trueClose = false;


    public App App {
      get { return Application.Current as App; }
    }

    public MainWindow() {
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

    public void ShowWindow() {
      App.StatusViewModel.UpdateValues();
      App.StatusViewModel.Start();
      Show();
    }

    public void HideWindow() {
      App.StatusViewModel.Stop();
      Hide();
    }
  }
}

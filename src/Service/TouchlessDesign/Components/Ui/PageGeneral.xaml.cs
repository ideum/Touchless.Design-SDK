using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace TouchlessDesign.Components.Ui {
  /// <summary>
  /// Interaction logic for PageGeneral.xaml
  /// </summary>
  public partial class PageGeneral : UserControl {
    public PageGeneral() {
      InitializeComponent();
    }

    private void HandleOpenDirectoryClicked(object sender, RoutedEventArgs e) {
      try {
        Process.Start(new ProcessStartInfo {
          UseShellExecute = true,
          Verb = "open",
          FileName = AppComponent.Ui.DataDir + Path.DirectorySeparatorChar
        });
      }
      catch (Exception ex) {
        Log.Error(ex);
      }
    }
  }
}

using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace TouchlessDesign.Components.Ui {
  /// <summary>
  /// Interaction logic for PageAbout.xaml
  /// </summary>
  public partial class PageAbout : UserControl {
    public PageAbout() {
      InitializeComponent();

      var v = Assembly.GetExecutingAssembly().GetName().Version;
      VersionText.Text = $"v{v.Major}.{v.Minor}.{v.Build}";
    }

    private void HandleReleasesClicked(object sender, RoutedEventArgs e) {
      OpenUrl("https://github.com/ideum/Touchless.Design-SDK/releases");
    }

    private void HandleIdeumClicked(object sender, RoutedEventArgs e) {
      OpenUrl("https://ideum.com/");
    }

    private void HandleWebsiteClicked(object sender, RoutedEventArgs e) {
      OpenUrl("https://touchless.design/");
    }

    private void HandleIssuesClicked(object sender, RoutedEventArgs e) {
      OpenUrl("https://github.com/ideum/Touchless.Design-SDK/issues");
    }

    private void HandleCodeClicked(object sender, RoutedEventArgs e) {
      OpenUrl("https://github.com/ideum/Touchless.Design-SDK");
    }

    private void OpenUrl(string url) {
      System.Diagnostics.Process.Start(url);
    }

    
  }
}

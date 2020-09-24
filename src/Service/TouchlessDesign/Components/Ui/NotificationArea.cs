using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace TouchlessDesign.Components.Ui {
  public class NotificationArea : ApplicationContext {

    private static NotificationArea _instance;
    private static Thread _thread;
    private static Ui _ui;

    private Icon _icon;
    private NotifyIcon _notify;

    public NotificationArea() {
      var assembly = System.Reflection.Assembly.GetExecutingAssembly();
      var iconStream = assembly.GetManifestResourceStream("TouchlessDesign.Resources.icon.ico");
      _icon = iconStream == null ? SystemIcons.Application : new Icon(iconStream);
      _notify = new NotifyIcon {
        Icon = _icon,
        Visible = true,
      };
      _notify.MouseClick += _notify_MouseClick;
    }

    private void _notify_MouseClick(object sender, MouseEventArgs e) {
      _ui.ShowUi();
    }

    private void HandleSettingsButtonClicked(object sender, EventArgs e) {
    }

    private void HandleExitButtonClicked(object sender, EventArgs e) {
      App.Close();
    }

    public static void Start(Ui ui) {
      _ui = ui;
      _thread = new Thread(RunThread);
      _thread.SetApartmentState(ApartmentState.STA);
      _thread.Start();
    }

    public static void Stop() {
      _instance?.ExitThread();
    }

    private static void RunThread() {
      _instance = new NotificationArea();
      using (_instance) {
        Application.Run(_instance);
      }
      _instance = null;
    }

    protected override void Dispose(bool disposing) {
      base.Dispose(disposing);
      if (!disposing) return;
      _notify?.Dispose();
      _icon?.Dispose();
      _notify = null;
      _icon = null;
    }
  }
}
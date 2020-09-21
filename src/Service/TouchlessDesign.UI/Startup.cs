using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace TouchlessDesign {
  public class Startup {

    public static bool Restart = false;

    [STAThread]
    private static void Main() {
      using (var singletonMutex = new Mutex(true, "Ideum.Touchless.Design_mutex", out var isAvailable)) {
        if (!isAvailable) {
          Trace.TraceError("Touchless.Design: Mutex not available. Exiting.");
          return;
        }
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        try {
          var app = new App();
          app.InitializeComponent();
          app.Run();
        }
        catch (Exception e) {
          Trace.TraceError(e.ToString());
        }
      }

      if (Restart) {
        Process.Start(Assembly.GetCallingAssembly().CodeBase);
      }
      Environment.Exit(0);
    }
  }
}
using System;
using System.Configuration;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace TouchlessDesign {
  public class App : ApplicationContext {

    public string DataDir { get; private set; }
    public string ExeDir { get; private set; }

    public Input Input { get; private set; }
    public Ipc Ipc { get; private set; }
    public Ui Ui { get; private set; }
    public LEDs.Controller LEDController { get; private set; }

    public Component[] Components { get; private set; }

    private App() {

    }

    private void Start() {
      DataDir = GetDataDir();
      ExeDir = GetExeDirectory();
      Log.Init(DataDir);
      Input = new Input();
      Ipc = new Ipc();
      Ui = new Ui();
      LEDController = new LEDs.Controller();
      Components = new Component[] { Input, Ipc, Ui, LEDController };
      foreach (var c in Components) {
        c.App = this;
        c.Start();
      }
    }

    public void Stop() {
      Application.Exit();
      foreach (var c in Components) {
        c.Stop();
        c.App = null;
      }
      Components = null;
    }

    private static string GetDataDir() {
      string dataDir;
      try {
        dataDir = ConfigurationManager.AppSettings["DataDirectory"];
      }
      catch (Exception) {
        dataDir = "%appdata%/Ideum/TouchlessDesignService";
      }
      dataDir = Environment.ExpandEnvironmentVariables(dataDir);
      return dataDir;
    }

    private static string GetExeDirectory() {
      var path = AppDomain.CurrentDomain.BaseDirectory;
      return path;
    }

    [STAThread]
    private static void Main() {
      using (var singletonMutex = new Mutex(true, "Ideum.TouchlessDesignService_mutex", out var isAvailable)) {
        if (!isAvailable) {
          Trace.TraceError("TouchlessDesign: Mutex not available. Exiting.");
          return;
        }
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        using (var instance = new App()) {
          instance.Start();
          Application.Run(instance);
        }
      }
      Environment.Exit(0);
    }

    public abstract class Component {

      public abstract void Start();
      public abstract void Stop();

      public App App { get; set; }

      public Ui Ui {
        get { return App.Ui; }
      }

      public Ipc Ipc {
        get { return App.Ipc; }
      }

      public Input Input {
        get { return App.Input; }
      }

      public LEDs.Controller Controller {
        get { return App.LEDController; }
      }

      public string DataDir {
        get { return App.DataDir; }
      }
    }
  }
}

using System;
using System.Diagnostics;
using System.IO;

namespace TouchlessDesign {
  public class ConcreteLog : Log.ILogger {

    public void Trace(object o) {
      System.Diagnostics.Trace.WriteLine(o);
    }

    public void Debug(object o) {
      System.Diagnostics.Trace.WriteLine(o);
    }

    public void Info(object o) {
      System.Diagnostics.Trace.WriteLine(o);
    }

    public void Warn(object o) {
      System.Diagnostics.Trace.TraceWarning(o.ToString());
    }

    public void Error(object o) {
      System.Diagnostics.Trace.TraceError(o.ToString());
    }

    public void Fatal(string o) {
      System.Diagnostics.Trace.TraceError(o);
    }

    public void Init(string dir) {
      try {
        dir = Path.Combine(dir, "logs");
        if (!Directory.Exists(dir)) {
          Directory.CreateDirectory(dir);
        }
        var path = Path.Combine(dir, "log.txt");
        var backupPath = Path.Combine(dir, "log.backup.txt");
        try {
          if (File.Exists(path)) {
            if (File.Exists(backupPath)) {
              File.Delete(backupPath);
            }
            File.Copy(path, backupPath);
            File.Delete(path);
          }
        }
        catch (Exception e) {
          System.Diagnostics.Trace.TraceError(e.ToString());
        }
        System.Diagnostics.Trace.Listeners.Add(new TextWriterTraceListener(path));
        System.Diagnostics.Trace.AutoFlush = true;
      }
      catch (Exception e) {
        System.Diagnostics.Trace.TraceError(e.ToString());
      }
    }

    public void DeInit() {
      System.Diagnostics.Trace.Flush();
    }
  }
}
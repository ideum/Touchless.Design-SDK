namespace Ideum.Logging {
  public interface ILogger {
    void Write(Levels level, object o);
  }
}
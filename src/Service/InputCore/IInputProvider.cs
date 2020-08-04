
namespace TouchlessDesign {

  /// <summary>
  /// Base interface for an input provider.
  /// Concrete implementations of this interface must define a public and parameter-less constructor.
  /// The DataDir, Cursor, and Hotspots properties are set before the start method is called.
  /// The aforementioned properties values will not be changed while the input provider is started.
  /// </summary>
  public interface IInputProvider {
    
    /// <summary>
    /// The data directory of the application. This is where configuration files are stored.
    /// </summary>
    string DataDir { get; set; }

    /// <summary>
    /// The ICursor interface associated with this input provider
    /// </summary>
    ICursor Cursor { get; set; }
    
    /// <summary>
    /// The collection of currently active hotspots
    /// </summary>
    IHotspotCollection Hotspots { get; set; }

    /// <summary>
    /// Called to initialize and then start this input provider.
    /// </summary>
    void Start();

    /// <summary>
    /// Called to stop and de-initialize this input provider. Any remaining disposable references should be cleaned up here.
    /// </summary>
    void Stop();
  }
}
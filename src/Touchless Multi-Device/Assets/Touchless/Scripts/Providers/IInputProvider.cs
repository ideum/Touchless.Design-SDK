using System.Collections.Generic;

namespace TouchlessDesign.Components.Input.Providers
{

  /// <summary>
  /// Base interface for an input provider.
  /// Concrete implementations of this interface must define a public and parameter-less constructor.
  /// The DataDir and Cursor are set before the start method is called.
  /// The aforementioned properties values will not be changed while the input provider is started.
  /// </summary>
  public interface IInputProvider
  {

    /// <summary>
    /// The data directory of the application. This is where configuration files are stored.
    /// </summary>
    string DataDir { get; set; }

    /// <summary>
    /// Call to update the list of active hands and inform the Input instance if the underlying provider is active
    /// </summary>
    /// <param name="hands">list to update and populate with Hand instances.</param>
    /// <returns>true if this provider is active and functioning, false otherwise.</returns>
    bool Update(Dictionary<int, Hand> hands);

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
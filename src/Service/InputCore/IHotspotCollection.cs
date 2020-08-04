using System;
using System.Collections.Generic;

namespace TouchlessDesign {

  /// <summary>
  /// An observable collection of <see cref="IHotspot"/> instances
  /// </summary>
  public interface IHotspotCollection {
    
    /// <summary>
    /// Checks to see if the defined hotspot exists in the collection
    /// </summary>
    /// <param name="h"></param>
    /// <returns></returns>
    bool Contains(IHotspot h);

    /// <summary>
    /// Adds a listener to the collection of listeners that are invoked when this collection has changed.
    /// </summary>
    /// <param name="listener">the listener to add</param>
    void AddOnChangedListener(Action listener);

    /// <summary>
    /// Removes a listener from the collection of listeners that are invoked when this collection has changed.
    /// </summary>
    /// <param name="listener">the listener to remove</param>
    void RemoveOnChangedListener(Action listener);

    /// <summary>
    /// Iterates through the hotspots collection, invoking the hotspot action for each hotspot
    /// </summary>
    /// <param name="action"></param>
    void ForEach(Action<IHotspot> action);

    /// <summary>
    /// Adds all hotspots to the provided IList. No clear operation is provided.
    /// </summary>
    /// <param name="collection"></param>
    void Populate(IList<IHotspot> collection);
  }
}
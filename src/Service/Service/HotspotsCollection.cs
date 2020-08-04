using System;
using System.Collections.Generic;

namespace TouchlessDesign {
  public class HotspotsCollection : IHotspotCollection {
    
    private readonly HashSet<Action> _changedListeners = new HashSet<Action>();
    private readonly List<IHotspot> _hotspots = new List<IHotspot>();
    
    public bool Contains(IHotspot h) {
      lock (_hotspots) {
        return _hotspots.Contains(h);
      }
    }

    public void AddOnChangedListener(Action listener) {
      lock (_changedListeners) {
        _changedListeners.Add(listener);
      }
    }

    public void RemoveOnChangedListener(Action listener) {
      lock (_changedListeners) {
        _changedListeners.Remove(listener);
      }
    }

    public void ForEach(Action<IHotspot> action) {
      lock (_hotspots) {
        foreach (var h in _hotspots) {
          try {
            action(h);
          }
          catch (Exception e) {
            Log.Error($"Caught exception while executing action in ForEach call. {e}");
          }
        }
      }
    }

    public void Populate(IList<IHotspot> collection) {
      lock (_hotspots) {
        foreach (var h in _hotspots) {
          collection.Add(h);
        }
      }
    }

    public void Add(IHotspot h) {
      lock (_hotspots) {
        _hotspots.Add(h);
      }
      InvokeChangedListeners();
    }

    public void Remove(IHotspot h) {
      lock (_hotspots) {
        _hotspots.Remove(h);
      }
      InvokeChangedListeners();
    }

    public void Add(IEnumerable<IHotspot> hotspots) {
      lock (_hotspots) {
        _hotspots.AddRange(hotspots);
      }
      InvokeChangedListeners();
    }

    public void Remove(IEnumerable<IHotspot> hotspots) {
      lock (_hotspots) {
        foreach (var h in hotspots) {
          if (_hotspots.Contains(h)) {
            _hotspots.Remove(h);
          }
        }
      }
      InvokeChangedListeners();
    }

    private void InvokeChangedListeners() {
      lock (_changedListeners) {
        foreach (var a in _changedListeners) {
          try {
            a();
          }
          catch (Exception e) {
            Log.Error($"Exception thrown while executing changed listener {a}");
          }
        }
      }
    }

  }
}
using Ideum.Data;
using UnityEngine;

namespace Ideum {
  
  public class App : MonoBehaviour {

    public Panel[] Panels;
    public ItemData[] Items;

    private ItemData _selectedItem;

    public ItemData SelectedItem {
      get { return _selectedItem; }
      set {
        LastSelectedItem = _selectedItem;
        _selectedItem = value;
        foreach (var panel in Panels) {
          panel.AppChangedSelection(value);
        }
      }
    }

    public ItemData LastSelectedItem { get; private set; }

    void Start() {
      foreach (var panel in Panels) {
        panel.App = this;
        panel.Init();
      }
      TouchlessDesign.Initialize(AppSettings.Get().DataDirectory.GetPath());
    }

    void OnApplicationQuit() {
      TouchlessDesign.DeInitialize();
    }
  }
}

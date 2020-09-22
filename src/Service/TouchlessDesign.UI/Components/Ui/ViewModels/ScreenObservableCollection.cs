using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TouchlessDesign.Components.Ui.ViewModels {
  public class ScreenObservableCollection : ObservableCollection<ScreenViewModel> {
    public ScreenObservableCollection() { }

    public ScreenObservableCollection(ICollection<ScreenViewModel> c) : base(c) { }
  }
}
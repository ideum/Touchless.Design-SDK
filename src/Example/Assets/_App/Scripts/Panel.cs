using UnityEngine;

namespace Ideum {
  public abstract class Panel : MonoBehaviour {

    public App App { get; set; }

    public abstract void Init();
    public abstract void AppChangedSelection(ItemData selection);

  }
}
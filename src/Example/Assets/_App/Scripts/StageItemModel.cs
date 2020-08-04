using UnityEngine;

namespace Ideum {
  public class StageItemModel : MonoBehaviour {

    public Transform Pivot;
    public Transform Model;

    public Vector3 OriginalPosition { get; private set; }

    public void Init() {
      OriginalPosition = transform.position;
    }

  }
}

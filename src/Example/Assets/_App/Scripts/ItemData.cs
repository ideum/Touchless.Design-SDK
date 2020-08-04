using UnityEngine;

namespace Ideum {

  public class ItemData : MonoBehaviour {

    public SelectionButton Button;
    public StageItemModel Model;

    public Color Color;
    public string Name;
    public string Title;
    public string Medium;
    public string Date;
    public string Location;

    [TextArea]
    public string Desc;
  }
}
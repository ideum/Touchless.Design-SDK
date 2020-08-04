using Ideum.Data;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Ideum {
  public class TouchlessClickSurface : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    public void OnPointerEnter(PointerEventData eventData) {
      TouchlessDesign.SetHoverState(HoverStates.Click);
    }

    public void OnPointerExit(PointerEventData eventData) {
      TouchlessDesign.SetHoverState(false);
    }
  }
}
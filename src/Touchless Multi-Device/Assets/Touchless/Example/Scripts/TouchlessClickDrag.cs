using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TouchlessDesignCore;

namespace TouchlessDesignCore.Examples
{
  public class TouchlessClickDrag : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IEndDragHandler
  {
    public bool Click, Drag, DragHorizontal, DragVertical;

    public void OnDrag(PointerEventData eventData)
    {
      TouchlessDesign.Instance.TryGetUserFromEventData(eventData, out TouchlessUser user);
      if (!user) return;

      if (Drag)
      {
        user.SetHoverState(HoverStates.Drag);
      }
      else if(DragHorizontal)
      {
        user.SetHoverState(HoverStates.DragHorizontal);
      }
      else if(DragVertical)
      {
        user.SetHoverState(HoverStates.DragVertical);
      }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
      if (!Drag) return;
      if (TouchlessDesign.Instance.TryGetUserFromEventData(eventData, out TouchlessUser user))
      {
        user.SetHoverState(HoverStates.None);
      }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
      TouchlessDesign.Instance.TryGetUserFromEventData(eventData, out TouchlessUser user);
      if (!user) return;

      if (Click)
      {
        user.SetHoverState(HoverStates.Click);
      }
      if (Drag)
      {
        user.SetHoverState(HoverStates.Drag);
      }
      else if (DragHorizontal)
      {
        user.SetHoverState(HoverStates.DragHorizontal);
      }
      else if (DragVertical)
      {
        user.SetHoverState(HoverStates.DragVertical);
      }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
      if (TouchlessDesign.Instance.TryGetUserFromEventData(eventData, out TouchlessUser user))
      {
        user.SetHoverState(HoverStates.None);
      }
    }
  }
}
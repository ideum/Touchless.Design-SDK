using System;
using Ideum.Data;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Ideum {
  public class TouchlessDragSurface : MonoBehaviour, IDragHandler, IPointerDownHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler {

    public event Action StartDragging, Dragging, EndDragging;

    public bool IsDragging { get; private set; }
    public Vector2 StartPosition { get; private set; }
    public Vector2 CurrentPosition { get; private set; }
    public Vector2 CurrentDelta { get; private set; }

    public Vector2 TotalDelta {
      get { return CurrentPosition - StartPosition; }
    }

    public void OnPointerDown(PointerEventData eventData) {
      IsDragging = true;
      StartPosition = CurrentPosition = eventData.position;
      StartDragging?.Invoke();
    }

    public void OnDrag(PointerEventData eventData) {
      CurrentPosition = eventData.position;
      CurrentDelta = eventData.delta;
      Dragging?.Invoke();
    }

    public void OnEndDrag(PointerEventData eventData) {
      IsDragging = false;
      EndDragging?.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData) {
      if (!TouchlessDesign.IsConnected) return;
      TouchlessDesign.SetHoverState(HoverStates.Drag);
    }

    public void OnPointerExit(PointerEventData eventData) {
      if (!TouchlessDesign.IsConnected) return;
      TouchlessDesign.SetHoverState(HoverStates.None);
    }
  }
}

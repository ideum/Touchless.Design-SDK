using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragPanel : MonoBehaviour, IDragHandler, IEndDragHandler
{
  public Image DragImage;

  private void Start()
  {
    DragImage.gameObject.SetActive(false);
  }

  public void OnDrag(PointerEventData eventData)
  {
    DragImage.gameObject.SetActive(true);
    DragImage.transform.position = Camera.main.ScreenToWorldPoint(eventData.position);
  }

  public void OnEndDrag(PointerEventData eventData)
  {
    DragImage.transform.localPosition = Vector3.zero;
    DragImage.gameObject.SetActive(false);
  }
}

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

namespace Ideum {
  public class DetectHover : UIBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public Action HoverEnter;
    public Action HoverExit;

    protected Selectable _selectable;
    protected bool _isHover = false;

    public void OnPointerEnter(PointerEventData eventData) {
      DoPointerEnterLogic();
    }

    public void OnPointerExit(PointerEventData eventData) {
      DoPointerExitLogic();

    }

    protected override void OnEnable() {
      base.OnEnable();
      if (_selectable == null) {
        _selectable = GetComponent<Selectable>();
      }
    }

    void Update() {
      if (_selectable != null) {
        if (!_selectable.interactable && _isHover) {
          DoPointerExitLogic();
        }
      }
    }

    protected override void OnDisable() {
      DoPointerExitLogic();
      base.OnDisable();

    }

    protected override void OnDestroy() {
      DoPointerExitLogic();
      base.OnDestroy();
    }

    private void DoPointerEnterLogic() {
      if (_selectable != null && !_selectable.interactable) return;
      _isHover = true;
      HoverEnter?.Invoke();
      //if (TouchlessNetwork.Instance != null) {
      //  TouchlessNetwork.Instance.HoverAreaActive(Ideum.Data.HoverStates.Click);
      //}
      //Debug.Log("Hover Enter");
    }

    protected void DoPointerExitLogic() {
      if (!_isHover) return;
      _isHover = false;
      HoverExit?.Invoke();
      //if (TouchlessNetwork.Instance != null) {
      //  TouchlessNetwork.Instance.HoverAreaDeactive();
      //}
      //Debug.Log("Hover Exit");
    }
  }
}

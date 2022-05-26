using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;

namespace Ideum {
  public class DetectHover : UIBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public Action<int> HoverEnter;
    public Action<int> HoverExit;
    private List<int> _hoveringUsers = new List<int>();

    protected Selectable _selectable;
    protected bool _isHover = false;

    public void OnPointerEnter(PointerEventData eventData) {
      DoPointerEnterLogic(eventData.pointerId);
    }

    public void OnPointerExit(PointerEventData eventData) {
      DoPointerExitLogic(eventData.pointerId);
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
          foreach(var userId in _hoveringUsers) {
            DoPointerExitLogic(userId);
          }
          _hoveringUsers.Clear();
        }
      }
    }

    protected override void OnDisable() {
      ClearHoveredUsers();
      base.OnDisable();

    }

    protected override void OnDestroy() {
      ClearHoveredUsers();
      base.OnDestroy();
    }

    protected void ClearHoveredUsers() {
      foreach (var userId in _hoveringUsers) {
        DoPointerExitLogic(userId);
      }
      _hoveringUsers.Clear();
    }

    private void DoPointerEnterLogic(int pointerId) {
      if (_selectable != null && !_selectable.interactable) return;
      _isHover = true;
      _hoveringUsers.Add(pointerId);
      HoverEnter?.Invoke(pointerId);
      //if (TouchlessNetwork.Instance != null) {
      //  TouchlessNetwork.Instance.HoverAreaActive(Ideum.Data.HoverStates.Click);
      //}
      //Debug.Log("Hover Enter");
    }

    protected void DoPointerExitLogic(int pointerId) {
      if (!_isHover) return;
      _hoveringUsers.Remove(pointerId);
      if(_hoveringUsers.Count == 0) {
        _isHover = false;
      }
      HoverExit?.Invoke(pointerId);
      //if (TouchlessNetwork.Instance != null) {
      //  TouchlessNetwork.Instance.HoverAreaDeactive();
      //}
      //Debug.Log("Hover Exit");
    }
  }
}

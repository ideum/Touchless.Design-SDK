using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Ideum {
  public class DetectDrag : DetectHover, IDragHandler, IPointerDownHandler, IPointerUpHandler {

    public Action DragStart;
    public Action DragEnd;
    public Action<Vector2> DragUpdate;

    public RectTransform Target {
      get { return _target; }
    }

    private RectTransform _target;

    private bool _grabbed = false;

    private Vector2 _lastFrame;
    private Vector2 _startPosition;

    private Sequence _seq;

    public void OnDrag(PointerEventData eventData) {
      DoDragUpdateLogic(eventData.position);
    }

    public void OnPointerDown(PointerEventData eventData) {
      DoDragStartLogic(eventData.position);
    }

    public void OnPointerUp(PointerEventData eventData) {
      DoDragEndLogic();
    }

    public void Deactivate() {
      _selectable.interactable = false;
    }

    public void Activate() {
      _selectable.interactable = true;
    }

    public void ResetRing(bool immediate = false) {
      if (immediate) {
        _target.anchoredPosition = _startPosition;
      }

      _seq?.Kill();
      _seq = DOTween.Sequence();

      _seq.Append(_target.DOAnchorPos(_startPosition, 0.5f).SetEase(Ease.InOutQuad));
    }

    void Update() {
      if(_selectable != null) {
        if(!_selectable.interactable && _isHover) {
          DoPointerExitLogic();
        }
        if(!_selectable.interactable && _grabbed) {
          DoDragEndLogic();
        }
      }
    }

    protected override void OnEnable() {
      base.OnEnable();
      _target = GetComponent<RectTransform>();
      _startPosition = _target.anchoredPosition;
    }

    protected override void OnDisable() {
      DoDragEndLogic();
      base.OnDisable();
    }

    protected override void OnDestroy() {
      DoDragEndLogic();
      base.OnDestroy();
    }

    private void DoDragEndLogic() {
      if (!_grabbed) return;
      _grabbed = false;
      DragEnd?.Invoke();
    }

    private void DoDragStartLogic(Vector2 pos) {
      if (_selectable != null && !_selectable.interactable) return;
      _grabbed = true;
      DragStart?.Invoke();
      _lastFrame = pos;
    }

    private void DoDragUpdateLogic(Vector2 pos) {
      if (!_grabbed) return;
      if (_selectable != null && !_selectable.interactable) return;

      float deltaX = _lastFrame.x - pos.x;
      float deltaY = _lastFrame.y - pos.y;

      Vector2 newPos = new Vector2(_target.anchoredPosition.x - deltaX, _target.anchoredPosition.y - deltaY);

      _lastFrame = pos;

      _target.anchoredPosition = newPos;
      DragUpdate?.Invoke(newPos);
    }
  }
}
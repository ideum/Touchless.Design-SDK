using DG.Tweening;
using Ideum;
using Ideum.Data;
using UnityEngine;
using UnityEngine.UI;

public class HandCursor : Cursor {

  public HoverStates State;

  public Image PointerHand;
  public UiCircle Ring;
  public Image DragHand;

  public float SizeHover = 180f;
  public float SizeShrink = 150f;

  public float BorderWidthFull = 24f;
  public float BorderWidthSmall = 9f;

  public float FingerDownDelta = 100f;

  private RectTransform _pointerHandRect;
  private float _pointerHandHeight;
  private RectTransform _cursorRect;
  private CanvasGroup _pointerHandCG;
  private CanvasGroup _dragHandCG;

  private Color _hoverColor = Color.yellow;
  private Color _selectedColor = Color.green;
  private Color _errorColor = Color.red;

  private bool _selected; 
  private Sequence _seq;

  private int _clickAnimationCounter = 0;
  private bool _clicking = false;

  private bool _touchWarningShowing = false;

  private void Awake() {
    _pointerHandRect = PointerHand.GetComponent<RectTransform>();
    _pointerHandHeight = _pointerHandRect.sizeDelta.y;
    _cursorRect = GetComponent<RectTransform>();

    PointerHand.color = _hoverColor;
    Ring.color = _hoverColor;
    Ring.BorderWidth = BorderWidthSmall;
    _cursorRect.sizeDelta = new Vector2(SizeHover, SizeHover);

    _pointerHandCG = PointerHand.GetComponent<CanvasGroup>();
    _dragHandCG = DragHand.GetComponent<CanvasGroup>();

    _pointerHandCG.alpha = 1.0f;
    _dragHandCG.alpha = 0.0f;
  }

  private void Update() {
    var canvas = GetComponentInParent<Canvas>();
    RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), Input.mousePosition, canvas.worldCamera, out var pos);
    _cursorRect.position = canvas.transform.TransformPoint(pos);
  }

  public override void DoStateChange(HoverStates state, bool selected) {
    if (State == state && _selected == selected) return;
    if (_clicking || _touchWarningShowing) return;
    Log.Info(state + " " + selected);

    State = state;
    _selected = selected;

    switch (state) {
      case HoverStates.None:
        Idle();
        break;
      case HoverStates.Click:
        if (selected) {
          ClickSelect();
        } else {
          _clickAnimationCounter = 0;
          HoverClick();
        }
        break;
      case HoverStates.DragHorizontal:
        HoverDrag();
        break;
      case HoverStates.Drag:
        HoverDrag();
        break;
    }
  }

  public override void ShowNoTouch() {
    _touchWarningShowing = true;

    _seq?.Kill();
    _seq = DOTween.Sequence();

    _seq.Append(Ring.DOColor(_errorColor, 0.5f));
    _seq.Join(PointerHand.DOColor(_errorColor, 0.5f));
    _seq.Join(_pointerHandCG.DOFade(1.0f, 0.5f));
    _seq.Join(_dragHandCG.DOFade(0.0f, 0.5f));
    _seq.Join(_pointerHandRect.DOSizeDelta(new Vector2(_pointerHandRect.sizeDelta.x, _pointerHandHeight), 0.5f));
    _seq.Join(DOTween.To(() => Ring.BorderWidth, x => Ring.BorderWidth = x, BorderWidthSmall, 0.5f));
    _seq.Join(_cursorRect.DOSizeDelta(new Vector2(SizeHover, SizeHover), 0.5f));
    _seq.AppendInterval(1f);
    _seq.OnComplete(() => {
      _touchWarningShowing = false;
      Idle();
    });
  }

  private void Idle() {
    _seq?.Kill();
    _seq = DOTween.Sequence();

    _seq.Append(Ring.DOColor(_hoverColor, 0.5f));
    _seq.Join(PointerHand.DOColor(_hoverColor, 0.5f));
    _seq.Join(_pointerHandCG.DOFade(1.0f, 0.5f));
    _seq.Join(_dragHandCG.DOFade(0.0f, 0.5f));
    _seq.Join(_pointerHandRect.DOSizeDelta(new Vector2(_pointerHandRect.sizeDelta.x, _pointerHandHeight), 0.5f));
    _seq.Join(DOTween.To(() => Ring.BorderWidth, x => Ring.BorderWidth = x, BorderWidthSmall, 0.5f));
    _seq.Join(_cursorRect.DOSizeDelta(new Vector2(SizeHover, SizeHover), 0.5f));
  }

  private void HoverClick() {
    _seq?.Kill();
    _seq = DOTween.Sequence();

    _seq.Append(Ring.DOColor(_hoverColor, 0.5f));
    _seq.Join(_pointerHandCG.DOFade(1.0f, 0.5f));
    _seq.Join(_dragHandCG.DOFade(0.0f, 0.5f));
    _seq.Join(PointerHand.DOColor(_hoverColor, 0.5f));
    _seq.Join(_pointerHandRect.DOSizeDelta(new Vector2(_pointerHandRect.sizeDelta.x, _pointerHandHeight), 0.5f));
    _seq.Join(DOTween.To(() => Ring.BorderWidth, x => Ring.BorderWidth = x, BorderWidthFull, 0.5f));
    _seq.Join(_cursorRect.DOSizeDelta(new Vector2(SizeHover, SizeHover), 0.5f));
    _seq.AppendInterval(0.5f);

    _clickAnimationCounter++;
    if (_clickAnimationCounter < 3) {

      _seq.Append(_cursorRect.DOSizeDelta(new Vector2(SizeShrink, SizeShrink), 0.5f));
      _seq.Join(_pointerHandRect.DOSizeDelta(new Vector2(_pointerHandRect.sizeDelta.x, _pointerHandHeight - FingerDownDelta), 0.5f));
      _seq.Join(DOTween.To(() => Ring.BorderWidth, x => Ring.BorderWidth = x, BorderWidthSmall, 0.5f));
      _seq.AppendInterval(0.25f);
      _seq.OnComplete(() => {
        HoverClick();
      });
    }
  }

  private void ClickSelect() {
    _clicking = true;
    _seq?.Kill();
    _seq = DOTween.Sequence();

    _seq.Append(Ring.DOColor(_selectedColor, 0.5f));
    _seq.Join(_pointerHandCG.DOFade(1.0f, 0.5f));
    _seq.Join(PointerHand.DOColor(_selectedColor, 0.5f));
    _seq.Join(_dragHandCG.DOFade(0.0f, 0.5f));
    _seq.Join(_pointerHandRect.DOSizeDelta(new Vector2(_pointerHandRect.sizeDelta.x, _pointerHandHeight - FingerDownDelta), 0.5f));
    _seq.Join(DOTween.To(() => Ring.BorderWidth, x => Ring.BorderWidth = x, BorderWidthSmall, 0.5f));
    _seq.Join(_cursorRect.DOSizeDelta(new Vector2(SizeShrink, SizeShrink), 0.5f));
    _seq.AppendInterval(0.4f);
    _seq.OnComplete(() => {
      _clicking = false;
    });
  }

  private void HoverDrag() {
    _seq?.Kill();
    _seq = DOTween.Sequence();

    _seq.Append(Ring.DOColor(_selected ? _selectedColor : _hoverColor, 0.5f));
    _seq.Join(_pointerHandCG.DOFade(0.0f, 0.5f));
    _seq.Join(_dragHandCG.DOFade(1.0f, 0.5f));
    _seq.Join(DragHand.DOColor(_selected ? _selectedColor : _hoverColor, 0.5f));
    _seq.Join(DOTween.To(() => Ring.BorderWidth, x => Ring.BorderWidth = x, BorderWidthSmall, 0.5f));
    float cursorSize = _selected ? SizeShrink : SizeHover;
    _seq.Join(_cursorRect.DOSizeDelta(new Vector2(cursorSize, cursorSize), 0.5f));
  }
}

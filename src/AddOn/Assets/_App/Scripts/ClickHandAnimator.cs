using DG.Tweening;
using Ideum.Data;
using Ideum;
using UnityEngine;

public class ClickHandAnimator : MonoBehaviour, IStateAnimator {

  public CanvasGroup HandCanvas;
  public UiCircle Touch;

  public float HandHeightDelta = 100f;
  public float TouchRingFull = 230f;
  public float TouchRingClick = 120f;

  private RectTransform _handRect;
  private float _handHeight;

  private RectTransform _touchRect;

  private Sequence _seq;

  public void AnimateTo(HoverStates state, bool clicked) {

    if (clicked && state != HoverStates.None) {
      Fade();
      return;
    }

    switch (state) {
      case HoverStates.Click:
        StartAnimationLoop();
        break;
      case HoverStates.None:
        Idle();
        break;
      default:
        Fade();
        break;
    }
  }

  public void HandleNoTouch() {
    Fade();
  }

  private void Awake() {
    _handRect = HandCanvas.GetComponent<RectTransform>();
    _handHeight = _handRect.sizeDelta.y;

    _touchRect = Touch.GetComponent<RectTransform>();

    _touchRect.sizeDelta = new Vector2(0, 0);
  }

  private void Fade() {
    _seq?.Kill();
    _seq = DOTween.Sequence();

    _seq.Append(HandCanvas.DOFade(0.0f, 0.5f));
    _seq.Join(_touchRect.DOSizeDelta(new Vector2(0, 0), 0.5f));
  }

  private void Idle() {
    _seq?.Kill();
    _seq = DOTween.Sequence();

    _seq.Append(HandCanvas.DOFade(1.0f, 0.5f));
    _seq.Join(_touchRect.DOSizeDelta(new Vector2(0, 0), 0.5f));
    _seq.Join(_handRect.DOSizeDelta(new Vector2(_handRect.sizeDelta.x, _handHeight), 0.5f));
  }

  private void StartAnimationLoop() {
    _seq?.Kill();
    _seq = DOTween.Sequence(); 

    _seq.Append(HandCanvas.DOFade(1.0f, 0.5f));
    _seq.Join(_handRect.DOSizeDelta(new Vector2(_handRect.sizeDelta.x, _handHeight), 0.5f));
    _seq.Join(_touchRect.DOSizeDelta(new Vector2(TouchRingFull, TouchRingFull), 0.5f));
    _seq.Join(Touch.DOColor(Color.white, 0.5f));
    _seq.Join(DOTween.To(() => Touch.BorderWidth, x => Touch.BorderWidth = x, 23, 0.5f));
    _seq.AppendInterval(1f);

    _seq.Append(_touchRect.DOSizeDelta(new Vector2(TouchRingClick, TouchRingClick), 0.5f));
    _seq.Join(Touch.DOColor(Color.green, 0.5f));
    _seq.Join(DOTween.To(() => Touch.BorderWidth, x => Touch.BorderWidth = x, 60, 0.5f));
    _seq.Join(_handRect.DOSizeDelta(new Vector2(_handRect.sizeDelta.x, _handHeight - HandHeightDelta), 0.5f));
    _seq.AppendInterval(0.25f);

    _seq.Join(_handRect.DOSizeDelta(new Vector2(_handRect.sizeDelta.x, _handHeight), 0.5f));
    _seq.Join(_touchRect.DOSizeDelta(new Vector2(TouchRingFull, TouchRingFull), 0.5f));
    _seq.Join(Touch.DOColor(Color.white, 0.5f));
    _seq.Join(DOTween.To(() => Touch.BorderWidth, x => Touch.BorderWidth = x, 23, 0.5f));
    _seq.AppendInterval(1f);

    _seq.OnComplete(() => {
      StartAnimationLoop();
    });
  }
}

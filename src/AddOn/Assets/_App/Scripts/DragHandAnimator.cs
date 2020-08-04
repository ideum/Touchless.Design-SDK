using DG.Tweening;
using Ideum.Data;
using Ideum;
using UnityEngine;
using UnityEngine.UI;

public class DragHandAnimator : MonoBehaviour, IStateAnimator {

  public CanvasGroup DragHand;
  public CanvasGroup DragMarker;

  public Image LeftArrow;
  public Image RightArrow;
  public UiCircle TouchCircle;

  public float DragCircleFull = 230f;
  public float DragCircleSmall = 140f;

  private RectTransform _touchCircleRect;

  private Sequence _seq;

  public void AnimateTo(HoverStates state, bool clicked) {

    if (clicked) {
      Fade();
      return;
    }

    switch (state) {
      case HoverStates.Drag:
        StartAnimationLoop();
        break;
      case HoverStates.DragHorizontal:
        StartAnimationLoop();
        break;
      case HoverStates.DragVertical:
        StartAnimationLoop();
        break;
      default:
        Fade();
        break;
    }
  }

  private void Fade() {
    _seq?.Kill();
    _seq = DOTween.Sequence();

    _seq.Append(DragHand.DOFade(0.0f, 0.5f));
    _seq.Join(DragMarker.DOFade(0.0f, 0.5f));
  }

  private void StartAnimationLoop() {
    LeftArrow.color = Color.yellow;
    RightArrow.color = Color.yellow;
    TouchCircle.color = Color.white;

    _touchCircleRect.sizeDelta = new Vector2(DragCircleFull, DragCircleFull);
    TouchCircle.BorderWidth = 23;

    _seq?.Kill();
    _seq = DOTween.Sequence();

    _seq.Append(DragMarker.DOFade(1.0f, 0.5f));
    _seq.Join(DragHand.DOFade(1.0f, 0.5f));
    _seq.AppendInterval(1f);

    _seq.Append(LeftArrow.DOColor(Color.green, 0.5f));
    _seq.Join(RightArrow.DOColor(Color.green, 0.5f));
    _seq.Join(TouchCircle.DOColor(Color.green, 0.5f));
    _seq.Join(_touchCircleRect.DOSizeDelta(new Vector2(DragCircleSmall, DragCircleSmall), 0.5f));
    _seq.Join(DOTween.To(() => TouchCircle.BorderWidth, x => TouchCircle.BorderWidth = x, 70, 0.5f));
    _seq.AppendInterval(0.5f);

    _seq.Append(LeftArrow.DOColor(Color.yellow, 0.5f));
    _seq.Join(RightArrow.DOColor(Color.yellow, 0.5f));
    _seq.Join(TouchCircle.DOColor(Color.white, 0.5f));
    _seq.Join(_touchCircleRect.DOSizeDelta(new Vector2(DragCircleFull, DragCircleFull), 0.5f));
    _seq.Join(DOTween.To(() => TouchCircle.BorderWidth, x => TouchCircle.BorderWidth = x, 23, 0.5f));
    _seq.AppendInterval(0.5f);

    _seq.OnComplete(() => {
      StartAnimationLoop();
    });
  }

  private void Awake() {
    _touchCircleRect = TouchCircle.GetComponent<RectTransform>();

    LeftArrow.color = Color.yellow;
    RightArrow.color = Color.yellow;
    TouchCircle.color = Color.white;

    _touchCircleRect.sizeDelta = new Vector2(DragCircleFull, DragCircleFull);
    TouchCircle.BorderWidth = 23;
  }

  public void HandleNoTouch() {
    Fade();
  }
}

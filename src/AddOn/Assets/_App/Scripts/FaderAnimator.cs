using DG.Tweening;
using Ideum.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FaderAnimator : MonoBehaviour, IStateAnimator {

  public CanvasGroup HandCG;
  public CanvasGroup IconCG;

  private Image _iconImage;

  private Color _green = new Color(0.165f, 0.918f, 0.165f, 1.0f);
  private Color _red = new Color(0.118f, 0.118f, 0.110f, 1.0f);

  private Sequence _seq;
  private void Awake() {
    _iconImage = IconCG.GetComponent<Image>();

    Idle();
  }

  public void AnimateTo(HoverStates state, bool clicked) {
    if (clicked && state != HoverStates.None) {
      Selected(state == HoverStates.Click);
      return;
    }
    switch (state) {
      case HoverStates.Click:
        Click();
        break;
      case HoverStates.Drag:
        Click();
        break;
      case HoverStates.DragHorizontal:
        Click();
        break;
      case HoverStates.DragVertical:
        Click();
        break;
      default:
        Idle();
        break;
    }
  }

  public void HandleNoTouch() {
    _seq?.Kill();
    _seq = DOTween.Sequence();

    _iconImage.color = _red;

    _seq.Join(HandCG.DOFade(1.0f, 0.5f));
    _seq.Join(IconCG.DOFade(1.0f, 0.5f));
  }

  private void Selected(bool click) {
    _seq?.Kill();
    _seq = DOTween.Sequence();

    _iconImage.color = _green;

    _seq.Join(HandCG.DOFade(0.0f, 0.25f));
    _seq.Join(IconCG.DOFade(1.0f, 0.5f));
  }

  private void Idle() {
    _seq?.Kill();
    _seq = DOTween.Sequence();

    _seq.Append(HandCG.DOFade(0.0f, 0.5f));
    if(IconCG.alpha > 0.0f) {
      _seq.Join(IconCG.DOFade(0.0f, 0.5f));
    }
    _seq.AppendInterval(2.5f);
    _seq.Append(HandCG.DOFade(1.0f, 0.5f));
    _seq.AppendInterval(2f);
    _seq.Append(HandCG.DOFade(0.0f, 0.5f));
    _seq.OnComplete(() => {
      Idle();
    });
  }

  private void Click() {
    _seq?.Kill();
    _seq = DOTween.Sequence();

    _seq.Append(HandCG.DOFade(0.0f, 0.5f));
    if (IconCG.alpha > 0.0f) {
      _seq.Join(IconCG.DOFade(0.0f, 0.5f));
    }
    _seq.AppendInterval(2.5f);
    _seq.Append(HandCG.DOFade(1.0f, 0.5f));
    _seq.AppendInterval(6.5f);
    _seq.Append(HandCG.DOFade(0.0f, 0.5f));

    _seq.OnComplete(() => {
      Click();
    });
  }
}

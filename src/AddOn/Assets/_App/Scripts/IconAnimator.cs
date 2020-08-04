using DG.Tweening;
using Ideum.Data;
using UnityEngine;
using UnityEngine.UI;

public class IconAnimator : MonoBehaviour, IStateAnimator {

  public Image Icon;

  public Sprite ClickSelectIcon;
  public Sprite DragSelectIcon;
  public Sprite ErrorIcon;

  private Sequence _seq;

  public void AnimateTo(HoverStates state, bool clicked) {
    if (!clicked) {
      Fade();
      return;
    }

    switch (state) {
      case HoverStates.Click:
        Icon.sprite = ClickSelectIcon;
        Show();
        break;
      case HoverStates.Drag:
        Icon.sprite = DragSelectIcon;
        Show();
        break;
      case HoverStates.DragHorizontal:
        Icon.sprite = DragSelectIcon;
        Show();
        break;
      case HoverStates.DragVertical:
        Icon.sprite = DragSelectIcon;
        Show();
        break;
      default:
        Fade();
        break;
    }
  }

  public void HandleNoTouch() {
    Icon.sprite = ErrorIcon;
    Show();
  }

  private void Fade() {
    _seq?.Kill();
    _seq = DOTween.Sequence();

    _seq.Append(Icon.DOFade(0.0f, 0.5f));
  }

  private void Show() {
    _seq?.Kill();
    _seq = DOTween.Sequence();

    _seq.Append(Icon.DOFade(1.0f, 0.5f));
  }
}

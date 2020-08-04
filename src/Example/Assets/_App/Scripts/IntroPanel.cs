using DG.Tweening;
using UnityEngine;

namespace Ideum {
  public class IntroPanel : Panel {

    public CanvasGroup CanvasGroup;

    private Tween _tween;

    public override void Init() {
      CanvasGroup.alpha = 1;
    }

    public override void AppChangedSelection(ItemData selection) {
      _tween?.Kill();
      if (selection == null) {
        _tween = CanvasGroup.DOFade(1f, 0.25f);
      }
      else {
        _tween = CanvasGroup.DOFade(0f, 0.25f);

      }
    }
  }
}

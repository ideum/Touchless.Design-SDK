using DG.Tweening;
using UnityEngine.UI;

namespace Ideum {
  public class BackgroundPanel : Panel {

    public Graphic SelectionGraphic;

    private Tween _tween;

    public override void Init() {
      _tween = SelectionGraphic.DOFade(0, 0f);
    }

    public override void AppChangedSelection(ItemData selection) {
      _tween?.Kill();
      _tween = SelectionGraphic.DOFade(selection == null ? 0 : 1, 0.5f);
    }
  }
}

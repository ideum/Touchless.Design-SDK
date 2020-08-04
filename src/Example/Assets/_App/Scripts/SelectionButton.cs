using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Ideum {
  public class SelectionButton : MonoBehaviour {

    public Button Button;
    public CanvasGroup GlowGroup, SelectGroup, GoBackGroup;

    private Sequence _seq;
    
    public void Select(bool isSelected) {
      _seq?.Kill();
      var s = _seq = DOTween.Sequence();
      if (isSelected) {
        s.Insert(0.00f, SelectGroup.DOFade(0f, 0.25f));
        s.Insert(0.25f, GoBackGroup.DOFade(1f, 0.25f));
        s.Insert(0.25f, GlowGroup.DOFade(1f, 0.25f));
      }
      else {
        s.Insert(0.00f, GoBackGroup.DOFade(0f, 0.25f));
        s.Insert(0.00f, GlowGroup.DOFade(0f, 0.25f));
        s.Insert(0.25f, SelectGroup.DOFade(1f, 0.25f));
      }
    }
  }
}

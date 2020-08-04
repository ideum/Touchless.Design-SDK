using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Ideum {
  [ExecuteInEditMode]
  public class InformationPanel : Panel {

    public TextMeshProUGUI Name, Desc, Title, Medium, Date, Location;

    public CanvasGroup CanvasGroup;
    public ScrollRect DescScroller;

    public Graphic[] ColoredGraphics;

    private Tween _tween;

    public override void Init() {
      CanvasGroup.blocksRaycasts = false;
      CanvasGroup.interactable = false;
    }

    public override void AppChangedSelection(ItemData selection) {
      _tween?.Kill();
      if (selection == null) {
        _tween = CanvasGroup.DOFade(0, 0.25f);
      }
      else {
        var s = DOTween.Sequence();
        _tween = s;

        if (App.LastSelectedItem != null) {
          s.Append(CanvasGroup.DOFade(0f, 0.25f));
          CanvasGroup.blocksRaycasts = false;
          CanvasGroup.interactable = false;
        }
        s.AppendCallback(() => {
          CanvasGroup.blocksRaycasts = true;
          CanvasGroup.interactable = true;
          DescScroller.normalizedPosition = new Vector2(0,1);
          Name.SetText(selection.name);
          Desc.SetText(selection.Desc);
          Title.SetText(selection.Title);
          Medium.SetText(selection.Medium);
          Date.SetText(selection.Date);
          Location.SetText(selection.Location);
          foreach (var c in ColoredGraphics) {
            c.color = selection.Color;
          }
        });
        s.Append(CanvasGroup.DOFade(1f, 0.25f));
      }
    }
  }
}
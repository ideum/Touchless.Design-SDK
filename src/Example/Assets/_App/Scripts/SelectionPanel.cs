using System.Linq;
using DG.Tweening;
using UnityEngine.UI;

namespace Ideum {
  public class SelectionPanel : Panel {
    
    public HorizontalLayoutGroup LayoutGroup;
    public SelectionButton[] Buttons;

    private Tween _tween;

    public override void Init() {
      LayoutGroup.spacing = 815;
      foreach (var button in Buttons) {
        AddButtonListener(button);
      }
    }

    public override void AppChangedSelection(ItemData selection) {
      _tween?.Kill();
      if (selection == null) {
        _tween = DOSpacing(825, 0.5f);
        foreach (var button in Buttons) {
          button.Select(false);
        }
      }
      else {
        _tween = DOSpacing(225, 0.5f);
        foreach (var button in Buttons) {
          button.Select(selection.Button == button);
        }
      }
    }

    private void AddButtonListener(SelectionButton button) {
      button.Button.onClick.AddListener(() => {
        var i = App.Items.FirstOrDefault(p => p.Button == button);
        if (i == null) {
          Log.Error("No mapped item for this button.");
        }
        else {
          App.SelectedItem = i == App.SelectedItem ? null : i;
        }
      });
    }

    private Tween DOSpacing(float endValue, float duration) {
      return DOTween.To(GetLayoutSpacing, SetLayoutSpacing, endValue, duration);
    }

    private float GetLayoutSpacing() {
      return LayoutGroup.spacing;
    }

    private void SetLayoutSpacing(float s) {
      LayoutGroup.spacing = s;
    }
  }
}
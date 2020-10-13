using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;

namespace Ideum {
  public class OnboardingStep : MonoBehaviour {

    public TextMeshProUGUI Text;
    public SVGImage Circle;
    public TextMeshProUGUI Number;

    public string SectionTitle;

    public Color Yellow;
    public Color Grey;

    public int Index;

    private Sequence _seq;

    private void Awake() {
      Text.text = SectionTitle;
      Number.text = "" + (Index + 1);
    }

    public void SetText(string text) {
      Text.text = text;
    }

    public void SetSelected(bool selected, bool immediate = false) {
      if (immediate) {
        Circle.color = selected ? Yellow : Grey;
        return;
      }

      _seq?.Kill();
      _seq = DOTween.Sequence();

      _seq.Append(Circle.DOColor(selected ? Yellow : Grey, 0.5f));
    }
  }
}

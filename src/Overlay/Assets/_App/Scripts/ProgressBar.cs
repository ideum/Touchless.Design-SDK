using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Ideum {
  public class ProgressBar : MonoBehaviour {

    public RectTransform FillBar;

    private RectTransform _background;
    private Sequence _seq;

    private void Awake() {
      _background = GetComponent<RectTransform>();
    }

    public void SetProgress(float normalizedProgress) {
      StartCoroutine(SetProgressDelayed(normalizedProgress));
    }

    private IEnumerator SetProgressDelayed(float normalizedProgress) {
      yield return new WaitForEndOfFrame();
      float fullWidth = _background.rect.width;

      _seq?.Kill();
      _seq = DOTween.Sequence();

      float targetWidth = fullWidth * normalizedProgress;

      _seq.Append(FillBar.DOSizeDelta(new Vector2(targetWidth, FillBar.rect.height), 0.5f));
    }
  }
}

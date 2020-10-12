using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;

namespace Ideum {
  public class OnboardingSteps : MonoBehaviour {

    public ProgressBar Bar;

    public List<OnboardingStep> Steps;
    public RectTransform StepsContainer;

    private LayoutElement _barResizer;
    private Sequence _seq;
    private CanvasGroup _bar;

    private void Awake() {
      _barResizer = GetComponentInChildren<LayoutElement>();

      StartCoroutine(UpdateWidth());
      _bar = Bar.GetComponent<CanvasGroup>();
      _bar.alpha = 0f;
    }

    public void Initialize(List<int> ActiveSections) {
      for(int i = 0; i < Steps.Count; i++) {
        if (ActiveSections[i] == 0) {
          Steps[i].gameObject.SetActive(false);
        } else {
          Steps[i].gameObject.SetActive(true);
        }
      }
    }

    public void Activate() {
      foreach (OnboardingStep s in Steps) {
        if (!s.gameObject.activeInHierarchy) continue;
        s.SetSelected(s.Index == 0, s.Index != 0);
      }
      _bar.alpha = 0.0f;
      _seq?.Kill();
      _seq = DOTween.Sequence();

      _seq.AppendInterval(0.25f);
      _seq.Append(_bar.DOFade(1.0f, 0.25f));
    }

    public void SetProgress(float progress, int completedStep) {

      Bar.SetProgress(progress);
      Steps[completedStep].SetSelected(true);
    }

    // Wait until the end of the frame because Content Size Fitter takes a frame to update it's width.
    private IEnumerator UpdateWidth() {
      yield return new WaitForEndOfFrame();
      _barResizer.preferredWidth = StepsContainer.rect.width - 160f;
    }
  }
}


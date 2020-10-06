using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Ideum {
  public class OnboardingSteps : MonoBehaviour {

    public ProgressBar Bar;

    public List<OnboardingStep> Steps;
    public RectTransform StepsContainer;

    private LayoutElement _barResizer;

    private void Awake() {
      _barResizer = GetComponentInChildren<LayoutElement>();

      StartCoroutine(UpdateWidth());
    }

    public void Initialize(List<int> ActiveSections) {
      foreach(OnboardingStep s in Steps) {
        if (!ActiveSections.Contains(s.Index)) {
          s.gameObject.SetActive(false);
        } else {
          s.gameObject.SetActive(true);
        }
      }
    }

    public void Activate() {
      foreach (OnboardingStep s in Steps) {
        if (!s.gameObject.activeInHierarchy) continue;
        s.SetSelected(s.Index == 0, s.Index != 0);
      }
      Bar.SetProgress(0.25f);
    }

    // Wait until the end of the frame because Content Size Fitter takes a frame to update it's width.
    private IEnumerator UpdateWidth() {
      yield return new WaitForEndOfFrame();
      _barResizer.preferredWidth = StepsContainer.rect.width;
    }
  }
}


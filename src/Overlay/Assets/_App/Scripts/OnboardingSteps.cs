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

    private string[] _texts = { "Hover & Point", "Point & Click", "Hold & Drag" };

    private void Awake() {
      _barResizer = GetComponentInChildren<LayoutElement>();

      StartCoroutine(UpdateWidth());
      _bar = Bar.GetComponent<CanvasGroup>();
      _bar.alpha = 0f;
    }

    public void Setup(List<int> ActiveSections) {
      int count = 0;
      for(int i = 0; i < ActiveSections.Count; i++) {
        if(ActiveSections[i] == 1) {
          Steps[count].SetText(_texts[i]);
          Steps[count].gameObject.SetActive(true);
          Steps[count].Index = count;
          count++;
        } 
      }
      for(int j = count; j < 3; j++) {
        Steps[j].gameObject.SetActive(false);
      }
      StartCoroutine(UpdateWidth());
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
      for(int i = 0; i < Steps.Count; i++) {
        Steps[i].SetSelected(completedStep >= Steps[i].Index);
      }
    }

    // Wait until the end of the frame because Content Size Fitter takes a frame to update it's width.
    private IEnumerator UpdateWidth() {
      yield return new WaitForEndOfFrame();
      Bar.SetProgress(0);
      _barResizer.preferredWidth = Mathf.Max(StepsContainer.rect.width - 160f, 1f);
    }
  }
}


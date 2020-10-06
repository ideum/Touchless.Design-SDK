using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Ideum {
  public class Onboarding : MonoBehaviour {

    public Button CloseBar;
    public OnboardingSteps Steps;

    public Action Closed;

    private CanvasGroup _cg;
    private Sequence _seq;

    private void Awake() {
      _cg = GetComponent<CanvasGroup>();
      _cg.alpha = 0.0f;
      _cg.blocksRaycasts = false;
      _cg.interactable = false;

      CloseBar.onClick.AddListener(() => {
        Closed?.Invoke();
      });
    }

    public void Initialize() {
      List<int> activeSections = new List<int> { 0, 1, 2 };
      Steps.Initialize(activeSections);
    }

    public void Activate() {
      _seq?.Kill();
      _seq = DOTween.Sequence();

      _cg.blocksRaycasts = true;
      _cg.interactable = true;

      _seq.Append(_cg.DOFade(1.0f, 0.5f));
      Steps.Activate();
    }

    public void Deactivate() {
      _seq?.Kill();
      _seq = DOTween.Sequence();

      _cg.blocksRaycasts = false;
      _cg.interactable = false;

      _seq.Append(_cg.DOFade(0.0f, 0.5f));
    }

  }
}

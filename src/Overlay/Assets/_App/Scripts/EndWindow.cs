using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ideum {
  public class EndWindow : MonoBehaviour {

    public List<TableSensorUI> Tables;
    public List<GameObject> Markers;
    public List<GameObject> Dividers;

    public RectTransform TopBar;
    public RectTransform ContentWindow;

    private CanvasGroup _cg;
    private Sequence _seq;

    private RectTransform _rect;

    public void Initialize(List<int> sections, bool isPedestal) {

      //int count = 0;
      //for (int i = 0; i < Tables.Count; i++) {
      //  if (sections[i] == 0) {
      //    Tables[i].gameObject.SetActive(false);
      //    continue;
      //  }

      //  count++;

      //  HandInfo info = new HandInfo();
      //  info.State = i == 0 ? HandState.Point : i == 1 ? HandState.Select : HandState.Close;
      //  info.MovementVectorShown = i == 0;
      //  info.SensorHighlighted = i != 0;

      //  Tables[i].Initialize(isPedestal);
      //  Tables[i].gameObject.SetActive(true);
      //  Tables[i].ChangeState(info);
      //}

      //for(int j = 0; j < Markers.Count; j++) {
      //  if (sections[j] == 0) {
      //    Markers[j].SetActive(false);
      //  } else {
      //    Markers[j].SetActive(true);
      //  }
      //}


      //if (count == 2) {
      //  Dividers[0].SetActive(true);
      //  Dividers[1].SetActive(false);
      //  TopBar.sizeDelta = new Vector2(TopBar.rect.width, 210);
      //  foreach (TableSensorUI ui in Tables) {
      //    ui.GetComponent<Transform>().localScale = new Vector3(1f, 1f, 1f);
      //  }
      //} else if (count == 1) {
      //  Dividers[0].SetActive(false);
      //  Dividers[1].SetActive(false);
      //  TopBar.sizeDelta = new Vector2(TopBar.rect.width, 310);
      //  foreach (TableSensorUI ui in Tables) {
      //    ui.GetComponent<Transform>().localScale = new Vector3(0.66f, 0.66f, 0.66f);
      //  }
      //} else {
      //  Dividers[0].SetActive(true);
      //  Dividers[1].SetActive(true);
      //  TopBar.sizeDelta = new Vector2(TopBar.rect.width, 310);
      //  foreach (TableSensorUI ui in Tables) {
      //    ui.GetComponent<Transform>().localScale = new Vector3(0.66f, 0.66f, 0.66f);
      //  }
      //}


      ContentWindow.sizeDelta = new Vector2(_rect.rect.width, _rect.rect.height - TopBar.rect.height);
      ContentWindow.anchoredPosition = new Vector2(0, -TopBar.rect.height);
      TopBar.anchoredPosition = new Vector2(0, -TopBar.rect.height);
    }

    public void Activate(float delay = 0f) {
      _seq?.Kill();
      _seq = DOTween.Sequence();

      _seq.AppendInterval(delay);
      _seq.Append(_cg.DOFade(1.0f, 0.5f));
    }

    public void Deactivate(bool immediate = false) {
      if (immediate) {
        _cg.alpha = 0.0f;
        return;
      }

      _seq?.Kill();
      _seq = DOTween.Sequence();

      _seq.Append(_cg.DOFade(0.0f, 0.5f));
    }

    private void Awake() {
      _cg = GetComponent<CanvasGroup>();

      _cg.alpha = 0.0f;
      _rect = GetComponent<RectTransform>();
    }
  }
}

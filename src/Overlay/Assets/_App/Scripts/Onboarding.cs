using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;

namespace Ideum {
  public class Onboarding : MonoBehaviour {

    public Button CloseBar;
    public OnboardingSteps Steps;

    public RectTransform ActivityRect;

    public GameObject PointActivityPrefab;
    public GameObject ClickActivityPrefab;
    public GameObject DragActivityPrefab;

    public EndWindow EndWindow;
    public CanvasGroup MainWindow;

    public TableSensorUI TableUI;
    public SensorBar Sensor;
    public RectTransform Scalar;

    private bool _active = false;
    private bool _complete = false;
    private float _progress = 0.0f;

    private CanvasGroup _cg;
    private Sequence _seq;

    private List<Activity> _activities;
    private Activity _currentActivity;
    private int _currentActivityIndex;

    private float _finishTimeout = 5f;
    private float _finishTimer = 0f;

    private float _timeout = 10f;
    private float _timeoutTimer = 0f;

    private void Awake() {
      _cg = GetComponent<CanvasGroup>();
      _cg.alpha = 0.0f;
      _cg.blocksRaycasts = false;
      _cg.interactable = false;

      CloseBar.onClick.AddListener(() => {
        Deactivate();
      });

      _activities = new List<Activity>();
    }

    private void Update() {
      if (_complete && _active) {
        _finishTimer += Time.deltaTime;
        if(_finishTimer > _finishTimeout) {
          Deactivate();
          _finishTimer = 0f;
        }
      }
    }

    public void Rescale(float newScale) {
      Scalar.sizeDelta = new Vector2(newScale, newScale);
    }

    public void ResizeSensorBar(Vector2 size) {
      Sensor.Resize(size);
    }

    public void MoveSensorBar(Vector2 newPosition) {
      Sensor.GetComponent<RectTransform>().anchoredPosition = newPosition;
    }

    public void Initialize() {
      List<int> activeSections = new List<int> { 0, 1, 2 };
      Steps.Initialize(activeSections);
      EndWindow.Initialize(activeSections);

      if (activeSections.Contains(0)) {
        Activity activity = Instantiate(PointActivityPrefab, ActivityRect).GetComponent<Activity>();
        _activities.Add(activity);
      }
      if (activeSections.Contains(1)) {
        Activity activity = Instantiate(ClickActivityPrefab, ActivityRect).GetComponent<Activity>();
        _activities.Add(activity);
      }
      if (activeSections.Contains(2)) {
        Activity activity = Instantiate(DragActivityPrefab, ActivityRect).GetComponent<Activity>();
        _activities.Add(activity);
      }
    }

    public void Activate() {
      if (_active) return;

      _active = true;
      _complete = false;

      _seq?.Kill();
      _seq = DOTween.Sequence();

      _cg.blocksRaycasts = true;
      _cg.interactable = true;
      EndWindow.Deactivate(true);
      MainWindow.alpha = 1.0f;
      Sensor.SetStatus(false);

      _seq.Append(_cg.DOFade(1.0f, 0.5f));

      _currentActivityIndex = 0;
      _seq.OnComplete(() => {
        Steps.Activate();

        _progress = 0.0f;
        Steps.SetProgress(_progress, 0);
        StartActivity();
      });
    }

    public void Deactivate() {
      if (!_active) return;

      _active = false;
      _complete = false;
      _finishTimer = 0f;

      _seq?.Kill();
      _seq = DOTween.Sequence();

      _cg.blocksRaycasts = false;
      _cg.interactable = false;

      _seq.Append(_cg.DOFade(0.0f, 0.5f));
    }


    private void StartActivity() {
      if (!_active) return;

      bool oldActivity = _currentActivity != null;
      if(oldActivity) {
        _currentActivity.Deactivate();
      }

      _progress = Mathf.Min(_progress + (1f / (_activities.Count + 1)), 1f);
      Steps.SetProgress(_progress, 0);

      _currentActivity = _activities[_currentActivityIndex];
      _currentActivity.Completed = HandleActivityCompleted;
      _currentActivity.ChangeTableUI = HandleTableUIChange;
      _currentActivity.Activate(oldActivity ? 0.5f : 0.0f);
    }

    private void HandleTableUIChange(HandInfo info) {
      if (!_active) return;

      TableUI.ChangeState(info);
      Sensor.SetStatus(info.SensorHighlighted);
    }

    private void HandleActivityCompleted() {
      _currentActivityIndex++;
      if (_currentActivityIndex >= _activities.Count) {
        _complete = true;
        EndWindow.Activate();
        _seq?.Kill();
        _seq = DOTween.Sequence();

        _seq.AppendInterval(0.5f);
        _seq.OnComplete(() => {
          MainWindow.alpha = 0.0f;
        });
        return;
      }

      _progress = Mathf.Min(_progress + (1f / (_activities.Count + 1)), 1f);
      Steps.SetProgress(_progress, _currentActivityIndex);

      StartActivity();
    }
  }
}

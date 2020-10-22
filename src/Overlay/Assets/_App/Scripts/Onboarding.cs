using DG.Tweening;
using System;
using System.Collections.Generic;
using TouchlessDesign.Config;
using UnityEngine;
using UnityEngine.UI;

namespace Ideum {
  public class Onboarding : MonoBehaviour {

    public Action<bool> SetActive;

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

    public bool Active {
      get { return _active; }
    }

    public bool Enabled {
      get { return _enabled; }
    }

    private bool _active = false;
    private bool _complete = false;
    private float _progress = 0.0f;

    private CanvasGroup _cg;
    private Sequence _seq;

    private List<Activity> _allActivities;
    private List<Activity> _activeActivities;
    private Activity _currentActivity;
    private int _currentActivityIndex;

    private float _finishTimeout = 5f;
    private float _finishTimer = 0f;

    private bool _enabled = true;
    private List<int> _activeSections;
    private bool _isPedestal = false;

    private void Awake() {
      _cg = GetComponent<CanvasGroup>();
      _cg.alpha = 0.0f;
      _cg.blocksRaycasts = false;
      _cg.interactable = false;

      CloseBar.onClick.AddListener(() => {
        SetActive?.Invoke(false);
      });

      _activeActivities = new List<Activity>();
      _allActivities = new List<Activity>();

      _activeSections = new List<int>() { 1,1,1};

      Activity activity1 = Instantiate(PointActivityPrefab, ActivityRect).GetComponent<Activity>();
      _allActivities.Add(activity1);
      Activity activity2 = Instantiate(ClickActivityPrefab, ActivityRect).GetComponent<Activity>();
      _allActivities.Add(activity2);
      Activity activity3 = Instantiate(DragActivityPrefab, ActivityRect).GetComponent<Activity>();
      _allActivities.Add(activity3);
    }

    private void Update() {
      if (_complete && _active) {
        _finishTimer += Time.deltaTime;
        if(_finishTimer > _finishTimeout) {
          SetActive?.Invoke(false);
          _finishTimer = 0f;
        }
      }
    }

    public void SettingsChanged(ConfigDisplay config) {
      if(_enabled != config.OnboardingEnabled) {
        _enabled = config.OnboardingEnabled;
        if(!_enabled && _active) {
          SetActive?.Invoke(false);
        } else if(_enabled && !_active) {
          SetActive?.Invoke(true);
        }
      }

      bool activitiesChanged = false;
      if((_activeSections[0] == 1) != config.Onboarding1Enabled) {
        activitiesChanged = true;
        _activeSections[0] = config.Onboarding1Enabled ? 1 : 0;
      }if((_activeSections[1] == 1) != config.Onboarding2Enabled) {
        activitiesChanged = true;
        _activeSections[1] = config.Onboarding2Enabled ? 1 : 0;
      }if((_activeSections[2] == 1) != config.Onboarding3Enabled) {
        activitiesChanged = true;
        _activeSections[2] = config.Onboarding3Enabled ? 1 : 0;
      }

      if (activitiesChanged) {
        SetupActivities();
      }

      if(Scalar.sizeDelta.x != config.OnboardingUIScale) {
        Rescale(config.OnboardingUIScale);
      }

      if(Sensor.Scale != config.OnboardingStatusBarScale) {
        ResizeSensorBar(config.OnboardingStatusBarScale);
      }

      if(Sensor.GetComponent<RectTransform>().anchoredPosition.x != config.OnboardingStatusBarXOffset) {
        MoveSensorBar(new Vector2(config.OnboardingStatusBarXOffset, 0));
      }
    }

    public void Initialize(bool isPedestal) {
      SetupActivities();

      _isPedestal = isPedestal;
      TableUI.Initialize(_isPedestal);
    }

    public void Activate() {
      if (_active || !_enabled) return;

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
      Steps.Activate();

      _currentActivityIndex = 0;
      _progress = _activeActivities.Count > 1 ? (float)_currentActivityIndex / ((float)_activeActivities.Count - 1f) : 0;
      Steps.SetProgress(_progress, _currentActivityIndex);

      _seq.OnComplete(() => {
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

    private void SetupActivities() {
      _activeActivities.Clear();

      if (_activeSections[0] == 1) {
        _activeActivities.Add(_allActivities[0]);
      }
      if (_activeSections[1] == 1) {
        _activeActivities.Add(_allActivities[1]);
      }
      if (_activeSections[2] == 1) {
        _activeActivities.Add(_allActivities[2]);
      }

      Steps.Setup(_activeSections);
      EndWindow.Initialize(_activeSections, _isPedestal);

      if (_active) {
        _currentActivityIndex = 0;
        _progress = _activeActivities.Count > 1 ? (float)_currentActivityIndex / ((float)_activeActivities.Count - 1f) : 0;
        Steps.SetProgress(_progress, _currentActivityIndex);
        StartActivity();
      }
    }

    private void Rescale(float newScale) {
      Scalar.localScale = new Vector2(newScale, newScale);
    }

    private void ResizeSensorBar(float scale) {
      Sensor.Resize(scale);
    }

    private void MoveSensorBar(Vector2 newPosition) {
      Sensor.GetComponent<RectTransform>().anchoredPosition = newPosition;
    }

    private void StartActivity() {
      if (!_active) return;

      bool oldActivity = _currentActivity != null;
      if(oldActivity) {
        _currentActivity.Deactivate();
      }

      _currentActivity = _activeActivities[_currentActivityIndex];
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
      _progress = _activeActivities.Count > 1 ? (float)_currentActivityIndex / ((float)_activeActivities.Count - 1f) : 0;
      Steps.SetProgress(_progress, _currentActivityIndex);
      if (_currentActivityIndex >= _activeActivities.Count) {
        _complete = true;
        EndWindow.Activate();
        _seq?.Kill();
        _seq = DOTween.Sequence();

        _seq.AppendInterval(0.5f);
        _seq.OnComplete(() => {
          MainWindow.alpha = 0.0f;
        });
        _currentActivity.Deactivate();
        _currentActivity = null;
        return;
      }

      StartActivity();
    }
  }
}

using DG.Tweening;
using Ideum.Data;
using System;
using System.Collections.Generic;
using System.Linq;
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

    private bool _enabled = true;
    private List<int> _activeSections;

    private void Awake() {
      _cg = GetComponent<CanvasGroup>();
      _cg.alpha = 0.0f;
      _cg.blocksRaycasts = false;
      _cg.interactable = false;

      CloseBar.onClick.AddListener(() => {
        SetActive?.Invoke(false);
      });

      _activities = new List<Activity>();
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

    public void SettingChanged(Msg message) {
      switch (message.S) {
        case "Enabled":
          _enabled = (bool)message.Bool;
          if(!_enabled && _active) {
            SetActive?.Invoke(false);
          }
          break;
        case "Hover&Point":
          if((bool)message.Bool && !_activeSections.Contains(0)) {
            _activeSections[0] = 1;
            SetupActivities();
          } else if((bool)message.Bool && _activeSections.Contains(0)) {
            _activeSections[0] = 0;
            SetupActivities();
          }
          break;
        case "Point&Click":
          if ((bool)message.Bool && !_activeSections.Contains(0)) {
            _activeSections[1] = 1;
            SetupActivities();
          } else if ((bool)message.Bool && _activeSections.Contains(0)) {
            _activeSections[1] = 0;
            SetupActivities();
          }
          break;
        case "Hold&Drag":
          if ((bool)message.Bool && !_activeSections.Contains(0)) {
            _activeSections[2] = 1;
            SetupActivities();
          } else if ((bool)message.Bool && _activeSections.Contains(0)) {
            _activeSections[2] = 0;
            SetupActivities();
          }
          break;
        case "UIScale":
          Rescale((float)message.F1);
          break;
        case "StatusBarScale":
          ResizeSensorBar((float)message.F1);
          break;
        case "StatusBarXOffset":
          Vector2 newPosition = new Vector2((float)message.F1, 0);
          MoveSensorBar(newPosition);
          break;
      }
    }

    public void Initialize() {
      _activeSections = new List<int> { 1, 1, 1 };
      SetupActivities();
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

    private void SetupActivities() {
      _activities.Clear();
      Steps.Initialize(_activeSections);
      EndWindow.Initialize(_activeSections);

      if (_activeSections[0] == 1) {
        Activity activity = Instantiate(PointActivityPrefab, ActivityRect).GetComponent<Activity>();
        _activities.Add(activity);
      }
      if (_activeSections[1] == 1) {
        Activity activity = Instantiate(ClickActivityPrefab, ActivityRect).GetComponent<Activity>();
        _activities.Add(activity);
      }
      if (_activeSections[2] == 1) {
        Activity activity = Instantiate(DragActivityPrefab, ActivityRect).GetComponent<Activity>();
        _activities.Add(activity);
      }
    }

    private void Rescale(float newScale) {
      Scalar.sizeDelta = new Vector2(newScale, newScale);
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

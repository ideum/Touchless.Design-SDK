/** This application is intended to be the main UI feedback mechanism for the Integrated Touchless System. When used with a client
 * application that integrates with the System, this application will change to show hover states, selection states, and no touch 
 * warning states.
 * 
 * When built, the app is windowless and replaces the Windows cursor with a cursor that animates, and changes color to provide both 
 * onboarding information and dynamic feedback.
**/

using DG.Tweening;
using Ideum.Data;
using System;
using TouchlessDesign.Config;
using UnityEngine;

namespace Ideum {
  public class App : MonoBehaviour {

    public Cursor Cursor;
    public CanvasGroup WarningBackground;
    public TransparentWindow Window;
    public Onboarding Onboarding;

    private bool _noTouchWarningEnabled = true;

    private bool _connected;
    private float _queryInterval = 0.25f;
    private float _timer;
    private int _handCount = 0;

    private bool _onboardingResetFlag = true;
    private float _onboardingResetInterval = 15f;
    private float _onboardingResetTimer = 0f;

    private float _onboardingTimeoutInterval = 30f;
    private float _onboardingTimeoutTimer = 0f;

    private Sequence _seq;
    bool _touchWarningActive = false;

    private bool _configChangeFlag = false;
    private ConfigDisplay _config;

    // Initialize the TouchlessDesign and path directory to Service and subscribe to OnConnect and OnDisconnect events.
    void Start() {
      TouchlessDesign.Initialize(AppSettings.Get().DataDirectory.GetPath());
      TouchlessDesign.Connected += OnConnected;
      TouchlessDesign.Disconnected += OnDisconnected;
      Onboarding.SetActive += SetOnboarding;
      Onboarding.Initialize(AppSettings.Get().IsPedestal);
      Cursor.GetComponent<CanvasGroup>().alpha = 0.0f;
    }

    // Deinitialize TouchlessDesign
    void OnApplicationQuit() {
      TouchlessDesign.DeInitialize();
    }

    private void OnDisconnected() {
      Log.Info("Disconnected. Suspending queries");
      Cursor.DoStateChange(HoverStates.None, false);
      TouchlessDesign.SettingChanged -= HandleSettingChanged;
      _connected = false;
    }

    private void HandleSettingChanged(Msg msg) {
      _config = msg.Config;
      _configChangeFlag = true;
    }

    private void HandleNewConfig() {
      if (Onboarding.Enabled != _config.OnboardingEnabled) {
        Onboarding.SetEnabled(_config.OnboardingEnabled);
      }

      if (_config.OnboardingNewUserTimeout_s != _onboardingResetInterval) {
        _onboardingResetInterval = _config.OnboardingNewUserTimeout_s;
      }
      if (_config.OnboardingNoHandTimeout_s != _onboardingTimeoutInterval) {
        _onboardingTimeoutInterval = _config.OnboardingNoHandTimeout_s;
      }
      if(_config.CursorEnabled != Cursor.gameObject.activeInHierarchy) {
        Cursor.gameObject.SetActive(_config.CursorEnabled);
      }
      if(_config.NoTouchEnabled != _noTouchWarningEnabled) {
        _noTouchWarningEnabled = _config.NoTouchEnabled;
      }
      Onboarding.SettingsChanged(_config);
    }

    private void OnConnected() {
      Log.Info("Connected. Starting to query...");
      TouchlessDesign.SubscribeToDisplayConfig();
      TouchlessDesign.SettingChanged += HandleSettingChanged;
      _connected = true;
    }

    // At a regular interval, query the click and hover states, as well as the no touch state, passing respective method delegates. Also manages onboarding timeout
    private void Update() {
      if (_configChangeFlag) {
        _configChangeFlag = false;
        HandleNewConfig();
      }

      if (!_onboardingResetFlag && !Onboarding.Active) {
        _onboardingResetTimer += Time.deltaTime;
        if(_onboardingResetTimer > _onboardingResetInterval) {
          _onboardingResetFlag = true;
          _onboardingResetTimer = 0f;
          Log.Debug("Onboarding reset. It will now activate when a new hand is detected.");
        }
      }

      if (Onboarding.Active) {
        _onboardingTimeoutTimer += Time.deltaTime;
        if(_onboardingTimeoutTimer > _onboardingTimeoutInterval) {
          SetOnboarding(false);
          _onboardingTimeoutTimer = 0f;
          _onboardingResetFlag = true;
          _onboardingResetTimer = 0f;
        }
      }

      if (_connected) {
        _timer += Time.deltaTime;
        if(_timer > _queryInterval) {
          TouchlessDesign.QueryClickAndHoverState(HandleQueryResponse);
          TouchlessDesign.QueryNoTouchState(HandleNoTouch);
          TouchlessDesign.QueryHandCount(HandleHandCount);
          _timer = 0f;
        }
      }
    }

    private void SetOnboarding(bool active) {
      TouchlessDesign.SetPriority(active ? 1 : 0);
      if (active) {
        Onboarding.Activate();
      } else {
        Onboarding.Deactivate();
      }

      Window.clickable = active;
    }

    // Method delegate to handle a change in the number of tracked hands. This is used to manage the timeout, and reset of the onboarding.
    private void HandleHandCount(int handCount) {
      if(_handCount != handCount) {
        if (_handCount == 0 && handCount > 0 && _onboardingResetFlag && Onboarding.Enabled) {
          SetOnboarding(true);
          _onboardingResetTimer = 0f;
          _onboardingResetFlag = false;
        }
        Cursor.GetComponent<CanvasGroup>().alpha = handCount > 0 ? 1.0f : 0.0f;
        _handCount = handCount;
      }
      if (_handCount > 0) {
        _onboardingResetTimer = 0f;
        _onboardingTimeoutTimer = 0f;
      }
    }

    // Method delegate to handle TouchlessDesign response to QueryNoTouchState. This will prompt the cursor to change its state as well
    // as animate the red warning background.
    private void HandleNoTouch(bool noTouch) {
      if (!_noTouchWarningEnabled) return;

      if (noTouch) {
        Cursor.ShowNoTouch();
      }

      if (!noTouch && _touchWarningActive) {
        _touchWarningActive = false;
        _seq?.Kill();
        _seq = DOTween.Sequence();
        _seq.Append(WarningBackground.DOFade(0.0f, 0.5f));
      } else if (noTouch && !_touchWarningActive) {
        Debug.Log("NO TOUCH: " + noTouch);
        _touchWarningActive = true;
        _seq?.Kill();
        _seq = DOTween.Sequence();

        _seq.Append(WarningBackground.DOFade(1.0f, 0.5f));
      }
    }

    // Method delegate to handle TouchlessDesign response to QueryClickAndHoverState. Passes both values on to the cursor.
    private void HandleQueryResponse(bool clickState, HoverStates hoverState) {
      Cursor.DoStateChange(hoverState, clickState);
    }

  }
}
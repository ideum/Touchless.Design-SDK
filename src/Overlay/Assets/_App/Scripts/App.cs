/** This application is intended to be the main UI feedback mechanism for the Integrated Touchless System. When used with a client
 * application that integrates with the System, this application will change to show hover states, selection states, and no touch 
 * warning states.
 * 
 * When built, the app is windowless and replaces the Windows cursor with a cursor that animates, and changes color to provide both 
 * onboarding information and dynamic feedback.
**/

using DG.Tweening;
using Ideum.Data;
using UnityEngine;

namespace Ideum {
  public class App : MonoBehaviour {

    public Cursor Cursor;
    public CanvasGroup WarningBackground;

    private bool _connected;
    private float _queryInterval = 0.25f;
    private float _timer;

    private Sequence _seq;
    bool _touchWarningActive = false;

    // Initialize the TouchlessDesign and path directory to Service and subscribe to OnConnect and OnDisconnect events.
    void Start() {
      TouchlessDesign.Initialize(AppSettings.Get().DataDirectory.GetPath());
      TouchlessDesign.Connected += OnConnected;
      TouchlessDesign.Disconnected += OnDisconnected;
    }

    // Deinitialize TouchlessDesign
    void OnApplicationQuit() {
      TouchlessDesign.DeInitialize();
    }

    private void OnDisconnected() {
      Log.Info("Disconnected. Suspending queries");
      Cursor.DoStateChange(HoverStates.None, false);
      _connected = false;
    }

    private void OnConnected() {
      Log.Info("Connected. Starting to query...");
      _connected = true;
    }

    // At a regular interval, query the click and hover states, as well as the no touch state, passing respective method delegates.
    private void Update() {
      if (_connected) {
        _timer += Time.deltaTime;
        if(_timer > _queryInterval) {
          TouchlessDesign.QueryClickAndHoverState(HandleQueryResponse);
          TouchlessDesign.QueryNoTouchState(HandleNoTouch);
          _timer = 0f;
        }
      }
    }

    // Method delegate to handle TouchlessDesign response to QueryNoTouchState. This will prompt the cursor to change its state as well
    // as animate the red warning background.
    private void HandleNoTouch(bool noTouch) {
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
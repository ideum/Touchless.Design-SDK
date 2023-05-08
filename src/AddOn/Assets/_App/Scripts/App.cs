/** This application is inteded to be a supplementary feedback mechanism for the Integrated Touchless System. It is intended
 * to run on a small 3.5" display attached to the Ideum touch table and shows dynamic gesture information and onboarding instructions
 * to users.
 **/

using System;
using Ideum.Data;
using System.IO;
using UnityEngine;
using Newtonsoft.Json.Linq;
using Ideum;

public class App : MonoBehaviour
{

  private bool _connected;
  private float _queryInterval = 0.25f;
  private float _timer;

  public UIController UIController;
  public RectTransform Scalar;

  public GameObject BigScreen;
  public GameObject PedestalScreen;

  // Sets the screen to whatever is specified in the settings file. Initialize the TouchlessDesign and path directory to Service and subscribe to OnConnect and OnDisconnect events.
  void Awake() {
    int screen = 0;
    try {
      var path = Path.Combine(Application.streamingAssetsPath, "settings.json");
      if (File.Exists(path)) {
        var json = File.ReadAllText(path);
        var obj = JObject.Parse(json);
        if (int.TryParse(obj["ScreenNumber"].ToString(), out screen)) {
          Log.Info("Screen set to " + screen);
        }
      }
    } catch (Exception e) {
      Log.Error(e);
    }

//#if !UNITY_EDITOR
//        if(Display.displays.Length > 0)
//        {
//            //PlayerPrefs.SetInt("UnitySelectMonitor", screen);

//            var display = Display.displays[0];
//            int w = display.systemWidth;
//            int h = display.systemHeight;
//            Screen.SetResolution(w, h, true);
//        } 

//    else if (Display.displays.Length > 1) {
//      //PlayerPrefs.SetInt("UnitySelectMonitor", screen);

//      var display = Display.displays[screen];
//      int w = display.systemWidth;
//      int h = display.systemHeight;

//      Display.displays[screen].Activate();
      
//      Screen.SetResolution(w, h, true);
//    }
//    else {
//      Application.Quit();
//    }
//#endif


    Ideum.TouchlessDesign.Initialize(AppSettings.Get().DataDirectory.GetPath());
    Ideum.TouchlessDesign.Connected += OnConnected;
    Ideum.TouchlessDesign.Disconnected += OnDisconnected;
  }

  // Deinitialize TouchlessDesign
  void OnApplicationQuit() {
    Ideum.TouchlessDesign.DeInitialize();
  }

  private void OnDisconnected() {
    Log.Info("Disconnected. Suspending queries");
    _connected = false;
  }

  // Query addon information once a connection is made with the Intergrated Touchless System.
  private void OnConnected() {
    Log.Info("Connected. Starting to query...");
    _connected = true;

#if UNITY_EDITOR
    Ideum.TouchlessDesign.Sync(() => {
      int width = Display.main.renderingWidth;
      int height = Display.main.renderingHeight;
      HandleAddOnQuery(true, true, width, height, width, height);
    });
#else
    Ideum.TouchlessDesign.QueryAddOn(HandleAddOnQuery);
#endif
  }

  // Response to the addon information query. Scales the application to fit the pixel ratio of the screen.
  private void HandleAddOnQuery(bool hasScreen, bool hasLEDs, int width_px, int height_px, int width_mm, int height_mm) {
    //BigScreen.SetActive(width_px != height_px);
    //PedestalScreen.SetActive(width_px == height_px);

    Log.Info($"{hasScreen}, {hasLEDs}, {width_px}, {height_px}, {width_mm}, {height_mm}");
    if (!hasScreen) return;
    float scaledX = Scalar.localScale.x * (width_mm / height_mm) / (width_px / height_px);
    Scalar.localScale = new Vector2(scaledX, Scalar.localScale.y);
  }

  // At a regular interval, query the click and hover states, as well as the no touch state, passing respective method delegates.
  private void Update() {
    if (_connected) {
      _timer += Time.deltaTime;
      if (_timer > _queryInterval) {
        Ideum.TouchlessDesign.QueryClickAndHoverState(HandleQueryResponse);
        Ideum.TouchlessDesign.QueryNoTouchState(HandleNoTouchState);
        _timer = 0f;
      }
    }
  }

  // Method delegate to handle TouchlessDesign response to QueryNoTouchState.
  private void HandleNoTouchState(bool noTouch) {
    UIController.NoTouchWarning(noTouch);
  }

  // Method delegate to handle TouchlessDesign response to QueryClickAndHoverState.
  private void HandleQueryResponse(bool clickState, HoverStates hoverState) {
    //Debug.Log("clickState: " + clickState + ", hoverState: " + hoverState);
    UIController.DoStateChange(hoverState, clickState);
  }
}

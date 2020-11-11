!["Splash Image"](/docs/images/splash.png)

# Touchless.Design SDK

[[Website](https://touchless.design)] [[Leap Motion SDK](https://developer.leapmotion.com/sdk-leap-motion-controller/)]

In light of the coronavirus crisis, touchless interaction has become the focus of many designers and developers who create kiosks and screen-based exhibits for public spaces. Using the [Leap Motion Controller](https://www.ultraleap.com/product/leap-motion-controller/) and [V4 SDK](https://developer.leapmotion.com/sdk-leap-motion-controller/), our Integrated Touchless System allows users to interact with our tables and exhibits with no physical touch. This system tracks the visitor’s hand and watches for open and close gestures to give complete control over the mouse. The Integrated Touchless System is a mouse emulation system with multiple methods for onboarding and real-time feedback.

Alongside this system are two additional applications: a dynamic cursor overlay and a secondary screen and LED system add-on. Download the latest release [here](). These applications are meant to provide both feedback and onboarding information to the visitor with changing colors, gesture icons, and interaction information when hovering over buttons or drag areas. This repository also includes a Unity asset package that allows users to integrate support for the system into their own applications and a demo application for reference. The core system is built in .Net Framework 4.6.1 and the peripheral applications are built in the Unity Game Engine.

Important Notes:
- The [Leap Motion V4 Orion SDK](https://developer.leapmotion.com/sdk-leap-motion-controller/) must be installed with a leap motion device connected for this application to function.
- This repository is meant to be built and run using the [Touchless Add-on](https://ideum.com/products/touch-tables/drafting#touchless) with a Leap Motion device, 3.5” display, and LED lights mounted on to an [Ideum Drafting Touch Table](https://ideum.com/products/touch-tables/drafting) or an [Ideum Touchless Pedestal](https://ideum.com/products/touchless/touchless-pedestal).
- The keyboard shortcut <b>Ctrl+Alt+I</b> will toggle mouse emulation on/off when the service is running.

Preview Video:

[![Touchless.Design demo](https://img.youtube.com/vi/apu0_l-zF6g/0.jpg)](https://www.youtube.com/watch?v=apu0_l-zF6g)

## Repository Layout

In the src directory is all of the source code for the Integrated Touchless System, as well as the peripheral applications and dependencies with the exception of the [Leap Motion V4 Orion SDK](https://developer.leapmotion.com/setup/desktop) which should be downloaded and installed before running the Touchless Design applications.

### Service

This is the core of the Integrated Touchless System. It operates as a Windows service and is responsible for integrating the Leap Motion SDK, controlling the mouse, managing the Overlay and Add-on applications, setting the color of the attached LEDs, and communicating with any client applications. Certain settings such as gesture toggles can be configured via a system tray icon as shown below. Other options can be configured by editing the configuration JSON files in the Integrated Touchless System's root directory. This is explained in greater depth [here](#Settings).

!["System Tray Icon"](/docs/images/system_tray.PNG)

### Overlay

The Overlay application is a replacement for the Windows cursor and provides feedback to the user as they interact with the Integrated Touchless System. When enabled, the System will automatically manage and communicate with the Overlay application.

!["Overlay Cursor"](/docs/images/cursor.png)

When interacting with a client application that is connected to the Integrated Touchless System, the Overlay will have the following behaviors:

- When hovering over a button, it will show an animated select gesture.
- When hovering over a drag area, it will show a closed fist with arrows.
- When a selection is made, it will turn green and change size.
- If the screen is touched, it will turn red.

### AddOn

The Add-on application is designed to display information on the secondary monitor and control the LED lights available on the [Touchless Add-on](https://ideum.com/products/touch-tables/drafting#touchless). This application provides further feedback and onboarding information to supplement the Overlay cursor as users interact with the Integrated Touchless System. When enabled, the System will automatically manage and communicate with the Add-on application. In addition to the animated hand icon, the Add-on application also shows text feedback.

!["AddOn"](/docs/images/addon.PNG)

### Asset Package

The Unity asset package, "TouchlessDesign.unitypackage", can be imported into a Unity Engine project and used to integrate that project application with the Integrated Touchless System. More detailed instructions for this can be found [here](#Adding-Support-to-a-Custom-Application).

### Example

The Example application is a demo client application that uses the Unity asset package to integrate with the Integrated Touchless System.

### Ideum.Logging

This is a logging abstraction we use in Unity and is included in all of the Unity projects, but we've included the source code here for posterity.

## Deployment

### Service

#### Building:

In order to build the core of the Integrated Touchless System, open the TouchlessDesign.sln file in Visual Studio, right-click the Solution and select "Build Solution." We have included build instructions that will build the solution at the following path on your system: 
```
%appdata%/Ideum
```
#### Running:

Navigate to the build directory (see above), open ```TouchlessDesignService/bin/Service/```, and run the TouchlessDesignService.exe. If the AddOn and/or Overlay applications have been built and configured, they will also be launched and the mouse should immediately begin reacting to the Leap Motion controller. An icon will appear in the system tray and exposes a number of options, including the ability to toggle on or off the mouse emulation. The System can also be terminated from this icon, which will likewise close the AddOn and Overlay applications if they are running.

#### Settings:

In the TouchlessDesignService directory are a number of configuration json files that can be used to adjust certain aspects of the Integrated Touchless System.

- input.json: Allows you to adjust options related to mouse emulation.
- leap.json: Allows you to adjust values relating to the mapping of Leap Motion data to screen position.
- led.json: Allows you to adjust values relating to the [FadeCandy](https://github.com/scanlime/fadecandy) LED control.
- network.json: Configures network settings for the Service.
- ui.json: Controls which on the peripheral applications are managed by the System.

### AddOn and Overlay

1. Download Unity version 2019.3.2f1
2. Open the application in Unity, and go to File -> Build Settings.
3. Make sure the _App/Scenes/App Scene is checked and press Build.
4. In order for the Integrated Touchless System to find and run either of the peripheral applications, they should be built at the following respective paths:
  - ```%appdata%/Ideum/TouchlessDesignService/bin/AddOn/```
  - ```%appdata%/Ideum/TouchlessDesignService/bin/Overlay/```
5. In the TouchlessDesignService directory, open the ui.json file and make sure the paths to the two applications are included in the ApplicationPaths field.

Now, when you run the Integrated Touchless System, it will automatically start up the applications specified in the ui.json file. Likewise, when the System is exited, it will terminate those same applications. Note: These applications will only provide gesture feedback when running alongside a client application that uses the Integrated Touchless System bindings, such as the Example application.

## Adding Support to a Custom Application

In order to integrate a project wih the Integrated Touchless System, the project must first import the unity asset package located in the root src directory of this repository. Below are the minimum recommended steps to get the asset package loaded and integrated. The Example application can also be used as a demonstration.

1. Open the Unity project.
2. Open Assets-> Import Package -> Custom Package...
3. This will open a dialog. Select the TouchlessDesign.unitypackage file in the src directory of this repository. When it prompts you to select which parts of the package to import, select Import All.
4. In a MonoBehavior script, in the Start function, call the TouchlessDesign.Initialize function and subscribe to its OnConnected and OnDisconnected events. (Make sure to include using Ideum in the file). Also include the Deinitialize call on application quit.

``` cs
void Start() {
  TouchlessDesign.Initialize("%appdata%/Ideum/TouchlessDesignService");
  TouchlessDesign.Connected += OnConnected;
  TouchlessDesign.Disconnected += OnDisconnected;
}

void OnApplicationQuit() {
  TouchlessDesign.DeInitialize();
}
```

5. In order to query the state of the System, there are a number of static query calls. For each one, pass a delegate that will handle the response from the System. Below is an example on a script that queries both the click and hover state, and the no touch state on an interval, and passes two delegates to those calls. When the TouchlessDesign recieves a response from the System for either call, it will execute the passed delegate.

``` cs
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

private void HandleNoTouch(bool noTouch) {
  if (noTouch) {
    // Show no-touch warning information.
  } 
}

private void HandleQueryResponse(bool clickState, HoverStates hoverState) {
  // Do something with the hover and click state.
}
```

6. In order to set a hover state (for example, when the user hovers over a button), create a new script and place it on whatever component you want to react to the hover state. In that script, implement the IPointerEnterHandler and IPointerExitHandler interfaces.

``` cs
public class TouchlessHoverCapture : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
```

7. Implement the interface functions OnPointerEnter and OnPointerExit. In those functions call the TouchlessDesign SetHoverState function, and pass the desired hover state, or pass false to deactivate, as shown below:

``` cs
public void OnPointerEnter(PointerEventData eventData) {
  TouchlessDesign.SetHoverState(HoverStates.Click);
}

public void OnPointerExit(PointerEventData eventData) {
  TouchlessDesign.SetHoverState(false);
}
```

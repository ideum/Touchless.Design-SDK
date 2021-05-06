using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using TouchlessDesignCore.Components;
using TouchlessDesignCore.Components.Input;
using TouchlessDesignCore.Components.Ipc;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Net;
using TouchlessDesignCore.Components.Ipc.Networking.Tcp;

namespace TouchlessDesignCore
{
  public class TouchlessUser : MonoBehaviour
  {
    public TouchlessUserInfo UserInfo { get; private set; }
    public HoverStates HoverState { get; private set; }
    public Vector2Int ScreenPosition { get; private set; }
    public Vector2Int PrevScreenPosition { get; private set; }
    public Vector3 CursorWorldPosition { get; private set; }
    public bool IsActivated;
    public bool IsClicking { get; private set; }
    public bool HasClicked { get; private set; }
    public bool HasReleased { get; private set; }
    public bool HoverChanged { get; private set; }
    public Client Client { get; set; }

    public Action<HoverStates, HoverStates> HoverStateChanged;
    public Action<TouchlessUser> CursorActivated;
    public Action<TouchlessUser> CursorDeactivated;
    public Action<TouchlessUser> PointerPressed;
    public Action<TouchlessUser> PointerReleased;

    private List<Hand> _hands = new List<Hand>();
    private Rect _bounds;
    private HoverStates _hoverState;
    private GameObject _currentHoverGo;
    private GameObject _oldHoverGo;

    #region UI
    public PointerEventData PointerEventData { get; private set; }
    public bool IsNoTouch { get; private set; }

    public GameObject TargetGameobject;
    private bool _isButtonDown;
    private List<RaycastResult> _hitData = new List<RaycastResult>();

    #endregion

    public void SetUserData(TouchlessUserInfo info)
    {
      UserInfo = info;
      HoverState = HoverStates.None;
      _bounds = new Rect(UserInfo.BoundsX, UserInfo.BoundsY, UserInfo.BoundsWidth, UserInfo.BoundsHeight);
    }

    public void DataMessageReceived(Msg msg)
    {
      UpdateHands(msg.Hands);
      UpdateCursorPosition();

      if (msg.Type == Msg.Types.ClickAndHoverQuery)
      {
        HoverState = msg.HoverState;
        SetMouseButtonDown(msg.Bool.Value);
      }
    }

    public void ClientTcpMessageReceived(Msg msg)
    {
      /// <summary>
      /// Processes an incoming message from an end point
      /// </summary>
      /// <param name="msg">The message to process</param>
      /// <param name="send">Invoked to send a Msg instance to the desired end point</param>

      try
      {
        switch (msg.Type)
        {
          case Msg.Types.None:
            break;
          case Msg.Types.Hover:
            if (msg.ContainsIncomingServerSideData)
            {
              Debug.LogWarning($"Changing Hover {HoverState} to {msg.HoverState}");
              Debug.Log("Hover state changing, aw ye");
              HoverState = msg.HoverState;
            }
            break;
          case Msg.Types.HoverQuery:
            Client.Send(Msg.Factories.HoverQuery(HoverState));
            break;
          case Msg.Types.Quit:

            break;
          case Msg.Types.Options:

            break;
          case Msg.Types.DimensionsQuery:
            Client.Send(Msg.Factories.DimensionsQuery((int)_bounds.xMin, (int)_bounds.yMin, (int)_bounds.width, (int)_bounds.height));
            break;
          case Msg.Types.Position:
            if (msg.ContainsIncomingServerSideData)
            {
              SetPosition(msg.X.Value, msg.Y.Value);
            }
            break;
          case Msg.Types.Click:
            if (msg.ContainsIncomingServerSideData)
            {
              SetMouseButtonDown(msg.Bool.Value);
            }
            break;
          case Msg.Types.ClickQuery:
            Client.Send(Msg.Factories.ClickQuery(IsClicking));
            break;
          case Msg.Types.ClickAndHoverQuery:
            Client.Send(Msg.Factories.ClickAndHoverQuery(IsClicking, HoverState));
            break;
          case Msg.Types.Ping:
            Client.Send(Msg.Factories.Ping());
            break;
          case Msg.Types.NoTouch:
            if (msg.ContainsIncomingServerSideData)
            {
              IsNoTouch = msg.Bool.Value;
            }
            break;
          case Msg.Types.NoTouchQuery:
            Client.Send(Msg.Factories.NoTouchQuery(IsNoTouch));
            break;
          case Msg.Types.AddOnQuery:
            //var dims_px = Ui.AddOnScreenBounds;
            //c.Send(Msg.Factories.AddOnQuery(
            //  Ui.HasAddOnScreen,
            //  Lighting.NetworkState == TouchlessDesign.Components.Lighting.Lighting.NetworkStates.Connected,
            //  dims_px.Width,
            //  dims_px.Height,
            //  Ui.AddOnWidth_mm,
            //  Ui.AddOnHeight_mm));
            break;
          case Msg.Types.SubscribeToDisplaySettings:
            //if (!_settingsInterestedClients.Contains(c))
            //{
            //  _settingsInterestedClients.Add(c);
            //  Msg settingsMsg = Msg.Factories.SettingsMessage(AppComponent.Config.Display);
            //  c.Send(settingsMsg);
            //}
            break;
          case Msg.Types.DisplaySettingsChanged:
            break;
          case Msg.Types.HandCountQuery:
            int handCount = _hands.Count;
            Client.Send(Msg.Factories.HandCountQuery(handCount));
            break;
          case Msg.Types.SetPriority:
            //Input.ClientPriority.Value = msg.Priority;
            break;
          case Msg.Types.OnboardingQuery:
            // Client.Send(Msg.Factories.OnboardingQueryMessage(Input.IsOnboardingActive.Value));
            break;
          case Msg.Types.SetOnboarding:
            // Input.IsOnboardingActive.Value = msg.Bool.Value;
            break;
          default:
            throw new ArgumentOutOfRangeException();
        }
      }
      catch (Exception e)
      {
        Debug.LogError($"Caught exception at process msg: {e}");
      }

    }

    public GameObject GetCurrentHoverGo()
    {
      return _currentHoverGo;
    }

    public GameObject GetOldHoverGo()
    {
      return _oldHoverGo;
    }

    private void SetHoverGo(GameObject go)
    {
      if (_currentHoverGo == go) return;
      //Debug.Log("Setting hover gameobject to " + (go != null ? go.name : "null"));
      _oldHoverGo = _currentHoverGo;
      _currentHoverGo = go;
      PointerEventData.pointerEnter = go;
      HoverChanged = true;
    }

    public void Update()
    {
      UpdateCursorHit(); // Use in update loop in case UI elements move/get disable inbetween network messages.
    }

    public void ResetClickState()
    {
      HasClicked = false;
      HasReleased = false;
    }

    public void ResetHoverState()
    {
      HoverChanged = false;
    }

    public void UpdateHands(Hand[] hands)
    {
      if (PointerEventData == null)
      {
        PointerEventData = new PointerEventData(EventSystem.current);
      }

      if (hands == null) return;

      _hands.Clear();

      foreach (Hand h in hands)
      {
        _hands.Add(h);
      }
    }

    public void SetHoverState(HoverStates state)
    {
      if (HoverState == state)
        return;
      HoverStates oldState = HoverState;
      HoverState = state;
      HoverStateChanged?.Invoke(oldState, state);
    }

    private void UpdateCursorPosition()
    {
      PrevScreenPosition = ScreenPosition;
      if (_hands.Count == 0)
      {
        IsActivated = false;
        ScreenPosition = new Vector2Int(-1, -1);
      }
      else
      {
        // position
        IsActivated = true;
        Hand targetHand = _hands[0];
        TouchlessComponent.Config.Input.NormalizedPosition(targetHand.X, targetHand.Y, targetHand.Z, out var h, out var v);
        var pixelX = Mathf.RoundToInt(h * _bounds.width + _bounds.xMin);
        var pixelY = Mathf.RoundToInt(v * _bounds.height + _bounds.yMin);

        SetPosition(pixelX, pixelY);

        //click
        var isGrabbing = targetHand.GrabStrength > TouchlessComponent.Config.Input.GrabClickThreshold;

        var hoverState = HoverState;
        if (isGrabbing && !_isButtonDown)
        {
          if (hoverState == HoverStates.Click)
          {
            if (!HasClicked)
            {
              HasClicked = true;
              DoClick();
            }
          }
          else
          {
            SetMouseButtonDown(true);
          }
        }
        else if (!isGrabbing && _isButtonDown)
        {
          SetMouseButtonDown(false);
        }

        if (!isGrabbing && HasClicked)
        {
          HasClicked = false;
        }

        CursorWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(ScreenPosition.x, ScreenPosition.y, 0));
      }
    }

    private void SetPosition(int pixelX, int pixelY)
    {
      var b = _bounds;
      if (pixelX < b.xMin) pixelX = (int)b.xMin;
      if (pixelX > b.xMax) pixelX = (int)b.xMax;
      if (pixelY > b.yMax) pixelY = (int)b.yMax;
      if (pixelY < b.yMin) pixelY = (int)b.yMin;

      PrevScreenPosition = ScreenPosition;
      ScreenPosition = new Vector2Int(pixelX, pixelY);
      PointerEventData.position = ScreenPosition;
      PointerEventData.delta = ScreenPosition - PrevScreenPosition;
      PointerEventData.pointerId = UserInfo.Id;
    }

    private void UpdateCursorHit()
    {
      if (PointerEventData == null)
        return;

      _hitData.Clear();
      TouchlessDesign.Instance.GraphicRaycaster.Raycast(PointerEventData, _hitData);

      if (_hitData.Count > 0)
      {
        var firstRaycast = FindFirstRaycast(_hitData);
        PointerEventData.pointerCurrentRaycast = firstRaycast;
        SetHoverGo(PointerEventData.pointerEnter = firstRaycast.gameObject);
      }
      else
      {
        PointerEventData.pointerCurrentRaycast = new RaycastResult();
        SetHoverGo(null);
      }
    }

    protected static RaycastResult FindFirstRaycast(List<RaycastResult> candidates)
    {
      RaycastResult result = new RaycastResult();
      for (var i = 0; i < candidates.Count; ++i)
      {
        if (candidates[i].gameObject == null)
          continue;

        return candidates[i];
      }
      return result;
    }

    private void DoClick()
    {
      if (!TouchlessComponent.Config.Input.ClickEnabled) return;
      if (IsClicking) return;
      SetMouseButtonDown(true);
      StartClickCountdown();
    }

    private void SetMouseButtonDown(bool down)
    {
      if (_isButtonDown == down) return;
      _isButtonDown = down;
      if (down)
      {
        HasClicked = true;
        HasReleased = false;
        IsClicking = true;
        this.PointerEventData.pointerPress = PointerEventData.pointerCurrentRaycast.gameObject;
        this.PointerEventData.eligibleForClick = PointerEventData.pointerCurrentRaycast.gameObject != null;
        this.PointerEventData.pressPosition = ScreenPosition;
        PointerPressed?.Invoke(this);
      }
      else
      {
        HasClicked = false;
        HasReleased = true;
        IsClicking = false;
        PointerReleased?.Invoke(this);
      }
      if (IsClicking && !down)
      {
        StopClickCountdown();
      }
    }

    private void StartClickCountdown()
    {
      Debug.Log("Starting click countdown");
      IsClicking = true;
      StartCoroutine(ClickCountDown());
    }

    private void StopClickCountdown()
    {
      IsClicking = false;
      StopCoroutine(ClickCountDown());
    }

    private IEnumerator ClickCountDown()
    {
      yield return new WaitForSecondsRealtime(TouchlessComponent.Config.Input.ClickDuration_ms);
      SetMouseButtonDown(false);
    }
  }
}

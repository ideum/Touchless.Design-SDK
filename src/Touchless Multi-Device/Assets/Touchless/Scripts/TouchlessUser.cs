using System;
using System.Collections;
using System.Collections.Generic;
using TouchlessDesign;
using TouchlessDesign.Components;
using TouchlessDesign.Components.Input;
using TouchlessDesign.Components.Ipc;
using TouchlessDesign.Hit;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TouchlessDesign
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

    public Action<HoverStates, HoverStates> HoverStateChanged;
    public static Action<TouchlessUser> CursorActivated;
    public static Action<TouchlessUser> CursorDeactivated;
    public static Action<TouchlessUser> PointerPressed;
    public static Action<TouchlessUser> PointerReleased;
    public static Action<TouchlessUser> PointerHoverMoved;

    private List<Hand> _hands = new List<Hand>();
    private Rect _bounds;
    private HoverStates _hoverState;
    private GameObject _currentHoverGo;
    private GameObject _oldHoverGo;

    #region UI
    public PointerEventData PointerEventData { get; private set; }
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
      SetHoverState(msg.HoverState);
      UpdateCursorPosition();

      if (msg.Type == Msg.Types.ClickAndHoverQuery)
      {
        HoverState = msg.HoverState;
        SetMouseButtonDown(msg.Bool.Value);
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
      _oldHoverGo = _currentHoverGo;
      _currentHoverGo = _oldHoverGo;
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

      _hands.Clear();
      foreach (Hand h in hands)
      {
        _hands.Add(h);
      }
    }

    public void SetHoverState(HoverStates state)
    {
      HoverStates oldState = HoverState;
      HoverState = state;
      if (HoverState != state)
      {
        HoverStateChanged?.Invoke(oldState, state);
      }
    }

    private void UpdateCursorPosition()
    {
      PrevScreenPosition = ScreenPosition;
      if (_hands.Count == 0)
      {
        IsActivated = false;
        ScreenPosition = new Vector2Int(0, 0);
      }
      else
      {
        // position
        IsActivated = true;
        Hand targetHand = _hands[0];
        AppComponent.Config.Input.NormalizedPosition(targetHand.X, targetHand.Y, targetHand.Z, out var h, out var v);
        var pixelX = Mathf.RoundToInt(h * _bounds.width + _bounds.xMin);
        var pixelY = Mathf.RoundToInt(v * _bounds.height + _bounds.yMin);

        var b = _bounds;
        if (pixelX < b.xMin) pixelX = (int)b.xMin;
        if (pixelX > b.xMax) pixelX = (int)b.xMax;
        if (pixelY > b.yMax) pixelY = (int)b.yMax;
        if (pixelY < b.yMin) pixelY = (int)b.yMin;

        Vector2 CursorPositionOld = ScreenPosition;
        ScreenPosition = new Vector2Int(pixelX, pixelY);

        PointerEventData.position = ScreenPosition;
        PointerEventData.delta = ScreenPosition - CursorPositionOld;
        PointerEventData.pointerId = UserInfo.Id;

        //click
        var isGrabbing = targetHand.GrabStrength > AppComponent.Config.Input.GrabClickThreshold;

        // SetMouseDownConfidence(hand.GrabStrength);
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

    private void UpdateCursorHit()
    {
      if (PointerEventData == null)
      return;

      _hitData.Clear();
      TouchlessApp.Instance.GraphicRaycaster.Raycast(PointerEventData, _hitData);

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
      for (var i = 0; i < candidates.Count; ++i)
      {
        if (candidates[i].gameObject == null)
          continue;

        return candidates[i];
      }
      return new RaycastResult();
    }

    private void DoClick()
    {
      if (!AppComponent.Config.Input.ClickEnabled) return;
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
      yield return new WaitForSecondsRealtime(AppComponent.Config.Input.ClickDuration_ms);
      SetMouseButtonDown(false);
    }

    private void OnDrawGizmosSelected()
    {
      if (Application.isPlaying && TouchlessApp.Instance.DebugHands && _hands.Count > 0)
      {
        if (UserInfo.Id == 0)
        {
          Gizmos.color = Color.green;
        }
        else
        {
          Gizmos.color = Color.yellow;
        }
        Gizmos.DrawSphere(CursorWorldPosition, 50f);
      }
    }
  }
}

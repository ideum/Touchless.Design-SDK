using System;
using System.Collections;
using System.Collections.Generic;
using TouchlessDesign;
using TouchlessDesign.Components;
using TouchlessDesign.Components.Input;
using TouchlessDesign.Components.Ipc;
using UnityEngine;

namespace TouchlessDesign
{
  public class TouchlessUser : MonoBehaviour
  {
    public TouchlessUserInfo UserInfo { get; private set; }
    public HoverStates HoverState { get;  private set; }
    public Vector2Int CursorScreenPosition { get; private set; }
    public Action<HoverStates, HoverStates> HoverStateChanged;
    private List<Hand> _hands = new List<Hand>();
    private Rect _bounds;
    private HoverStates _hoverState;
    private bool _isButtonDown;
    private bool _hasClicked;
    private bool _isClicking;

    public void Update()
    {
      if (TouchlessApp.Instance.DebugHands)
      {
        if (_hands.Count > 0)
        {
          transform.position = Camera.main.ScreenToWorldPoint(new Vector3(CursorScreenPosition.x, CursorScreenPosition.y, 0));
        }
      }
    }

    public void SetUserData(TouchlessUserInfo info)
    {
      UserInfo = info;
      HoverState = HoverStates.None;
      Debug.Log("Bounds width: " + info.BoundsWidth);
      Debug.Log("IP: " + info.IpAddress);
      _bounds = new Rect(UserInfo.BoundsX, UserInfo.BoundsY, UserInfo.BoundsWidth, UserInfo.BoundsHeight);
      Debug.Log("Bounds set to " + _bounds.width + _bounds.height);
    }

    public void DataMessageReceived(Msg msg)
    {
      UpdateHands(msg.Hands);
      UpdateHoverState(msg.HoverState);
      
      if(msg.Type == Msg.Types.ClickAndHoverQuery)
      {
        HoverState = msg.HoverState;
        SetMouseButtonDown(msg.Bool.Value);
      }
    }

    public void StateMessagedRecieved()
    {

    }

    public void UpdateHands(Hand[] hands)
    {
      _hands.Clear();
      foreach (Hand h in hands)
      {
        _hands.Add(h);
      }

      UpdateCursorState();
    }

    public void UpdateHoverState(HoverStates state)
    {
      HoverStates oldState = HoverState;
      HoverState = state;
      if(HoverState != state)
      {
        HoverStateChanged?.Invoke(oldState, state);
      }
    }

    private void UpdateCursorState()
    {
      if (_hands.Count == 0)
      {
        // Set position to zero
        CursorScreenPosition = new Vector2Int(0, 0);
      }
      else
      {
        // position
        Hand targetHand = _hands[0];
        AppComponent.Config.Input.NormalizedPosition(targetHand.X, targetHand.Y, targetHand.Z, out var h, out var v);
        var pixelX = Mathf.RoundToInt(h * _bounds.width + _bounds.xMin);
        var pixelY = Mathf.RoundToInt(v * _bounds.height + _bounds.yMin);

        var b = _bounds;
        if (pixelX < b.xMin) pixelX = (int)b.xMin;
        if (pixelX > b.xMax) pixelX = (int)b.xMax;
        if (pixelY > b.yMax) pixelY = (int)b.yMax;
        if (pixelY < b.yMin) pixelY = (int)b.yMin;

        CursorScreenPosition = new Vector2Int(pixelX, pixelY);

        //click
        var isGrabbing = targetHand.GrabStrength > AppComponent.Config.Input.GrabClickThreshold;
        // SetMouseDownConfidence(hand.GrabStrength);
        var hoverState = HoverState;
        if (isGrabbing && !_isButtonDown)
        {
          if (hoverState == HoverStates.Click)
          {
            if (!_hasClicked)
            {
              _hasClicked = true;
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

        if (!isGrabbing && _hasClicked)
        {
          _hasClicked = false;
        }
      }
    }

    private void DoClick()
    {
      if (!AppComponent.Config.Input.ClickEnabled) return;
      if (_isClicking) return;
      SetMouseButtonDown(true);
      StartClickCountdown();
    }

    private void SetMouseButtonDown(bool down)
    {
      if (_isButtonDown == down) return;
      _isButtonDown = down;
      if (_isClicking && !down)
      {
        StopClickCountdown();
      }

      // var status = down ? MouseEventFlags.LeftDown : MouseEventFlags.LeftUp;
      // mouse_event((uint)status, 0, 0, 0, 0);
    }

    private void StartClickCountdown()
    {
      _isClicking = true;
      StartCoroutine(ClickCountDown());
    }

    private void StopClickCountdown()
    {
      _isClicking = false;
      StopCoroutine(ClickCountDown());
    }

    private IEnumerator ClickCountDown()
    {
      yield return new WaitForSecondsRealtime(AppComponent.Config.Input.ClickDuration_ms);
      SetMouseButtonDown(false);
    }
  }
}

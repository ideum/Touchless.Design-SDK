using Ideum.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Ideum {
  [System.Serializable]
  public class TouchlessUser {

    public event Action<HoverStates> HoverStateChanged;

    /// <summary>
    /// The device ID of this touchless user.
    /// </summary>
    public int DeviceId;

    /// <summary>
    /// HoverState
    /// </summary>
    public HoverStates HoverState;

    /// <summary>
    /// The number of hands this user is tracking.
    /// </summary>
    public int HandCount { get; private set; }

    /// <summary>
    /// Rectangle described in normalized coordinates the bounding box for the client's position bounds.
    /// </summary>
    public Rect Bounds { get; private set; }

    /// <summary>
    /// Whether or not user pointer is "down"
    /// </summary>
    public bool IsButtonDown { get; private set; }

    /// <summary>
    /// Whether or not the user has changed it's pointer state to "down" in the last update
    /// </summary>
    public bool InitialPress;

    /// <summary>
    /// Whether or not the user has changed its pointer state to "up" in the last update
    /// </summary>
    public bool InitialRelease;

    /// <summary>
    /// Previous screen position of this touchless user
    /// </summary>
    public int PrevX;
    public int PrevY;

    /// <summary>
    /// The screen position of the primary hand of this user.
    /// </summary>
    public int X { get; private set; }
    public int Y { get; private set; }


    public Vector3 ScreenPosition { get { return new Vector2(X, Y); } }


    #region Pointer
    public bool HoverChanged;
    //public MouseState MouseState;
    // public PointerEventData PointerEventData { get; private set; }
    private GameObject _currentHoverGo;
    private GameObject _oldHoverGo;
//    private Vector2 screenDelta;
    private List<RaycastResult> _hitData = new List<RaycastResult>();

    /// <summary>
    /// This should be called every time data is updated from a <see cref="Ideum.Data.Msg.Types.UserUpdate"/> message
    /// </summary>
    public void Update(Msg msg) {
      //UpdateCursorPositon();
      PrevX = X;
      PrevY = Y;
      X = msg.X ?? -1;
      Y = msg.Y ?? -1;
      HandCount = msg.HandCount;

      IsButtonDown = msg.IsClicking;

      if(!InitialPress)
        InitialPress = msg.Bool ?? false;

      if(!InitialRelease)
        InitialRelease = msg.Bool2 ?? false;

      HoverStates prevState = HoverState;
      HoverState = msg.HoverState;
      //if (HoverState != prevState) {
      //  HoverStateChanged?.Invoke(HoverState);
      //}
      //PointerEventData = new PointerEventData(EventSystem.current);
      UpdateCursorPosition();
      UpdateCursorHit();
    }

    public GameObject GetOldHoverGo() {
      return _oldHoverGo;
    }

    private void UpdateCursorPosition() {
      // screenDelta = new Vector2(X - PrevX, Y - PrevY);
      //PointerEventData.delta = screenDelta;
      //PointerEventData.position = new Vector2(X, Y);
      //PointerEventData.pointerId = DeviceId;
    }

    public void SetHoverGo(GameObject go) {
      if (_currentHoverGo == go) return;
      _oldHoverGo = _currentHoverGo;
      _currentHoverGo = go;
      //PointerEventData.pointerEnter = go;
      HoverChanged = true;
    }

    private void UpdateCursorHit() {
      //if (PointerEventData == null)
       // return;

      _hitData.Clear();
      //EventSystem.current.RaycastAll(PointerEventData, _hitData);
      //TouchlessDesign.Instance.GraphicRaycaster.Raycast(PointerEventData, _hitData);

      if (_hitData.Count > 0) {
        var firstRaycast = FindFirstRaycast(_hitData);
        //PointerEventData.pointerCurrentRaycast = firstRaycast;
        //PointerEventData.pointerEnter = firstRaycast.gameObject;
        //SetHoverGo(PointerEventData.pointerEnter);
      }
      else {
        //PointerEventData.pointerCurrentRaycast = new RaycastResult();
        SetHoverGo(null);
      }
    }

    protected static RaycastResult FindFirstRaycast(List<RaycastResult> candidates) {
      RaycastResult result = new RaycastResult();
      for (var i = 0; i < candidates.Count; ++i) {
        if (candidates[i].gameObject == null)
          continue;

        return candidates[i];
      }
      return result;
    }


    #endregion

  }
}

using Ideum.Data;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextAnimator : MonoBehaviour, IStateAnimator {

  public TextMeshProUGUI UpperText;
  public TextMeshProUGUI LowerText;

  public string UpperIdle = "Touchless System";
  public string LowerIdle = "Hover <br> & Point";

  public string UpperClickHover = "Point <br>& Click";
  public string LowerClickHover = "Over a Selection";

  public string UpperClickSelect = "Selection";
  public string LowerClickSelect = "Made";

  public string UpperDragSelect = "Dragging";
  public string LowerDragSelect = "Object";

  public string UpperDragHover = "Close <br>Hand";
  public string LowerDragHover = "& Drag <br>to Rotate";

  public string UpperError = "Touchless";
  public string LowerError = "System";

  private RectTransform _upperTextRect;
  private float _upperRectHeight;

  public void AnimateTo(HoverStates state, bool clicked) {
    switch (state) {
      case HoverStates.None:
        UpperText.text = UpperIdle;
        LowerText.text = LowerIdle;
        _upperTextRect.sizeDelta = new Vector2(_upperTextRect.sizeDelta.x, _upperRectHeight);
        UpperText.color = Color.white;
        LowerText.color = Color.yellow;
        break;
      case HoverStates.Click:
        UpperText.text = clicked ? UpperClickSelect : UpperClickHover;
        LowerText.text = clicked ? LowerClickSelect : LowerClickHover;
        _upperTextRect.sizeDelta = new Vector2(_upperTextRect.sizeDelta.x, clicked ? _upperRectHeight / 2 : _upperRectHeight);
        UpperText.color = clicked ? Color.white : Color.yellow;
        LowerText.color = clicked ? Color.green : Color.white;
        break;
      case HoverStates.Drag:
        UpperText.text = clicked ? UpperDragSelect : UpperDragHover;
        LowerText.text = clicked ? LowerDragSelect : LowerDragHover;
        _upperTextRect.sizeDelta = new Vector2(_upperTextRect.sizeDelta.x, clicked ? _upperRectHeight / 2 : _upperRectHeight);
        UpperText.color = clicked ? Color.white : Color.yellow;
        LowerText.color = clicked ? Color.green : Color.white;
        break;
      case HoverStates.DragHorizontal:
        UpperText.text = clicked ? UpperDragSelect : UpperDragHover;
        LowerText.text = clicked ? LowerDragSelect : LowerDragHover;
        _upperTextRect.sizeDelta = new Vector2(_upperTextRect.sizeDelta.x, clicked ? _upperRectHeight / 2 : _upperRectHeight);
        UpperText.color = clicked ? Color.white : Color.yellow;
        LowerText.color = clicked ? Color.green : Color.white;
        break;
      case HoverStates.DragVertical:
        UpperText.text = clicked ? UpperDragSelect : UpperDragHover;
        LowerText.text = clicked ? LowerDragSelect : LowerDragHover;
        _upperTextRect.sizeDelta = new Vector2(_upperTextRect.sizeDelta.x, clicked ? _upperRectHeight / 2 : _upperRectHeight);
        UpperText.color = clicked ? Color.white : Color.yellow;
        LowerText.color = clicked ? Color.green : Color.white;
        break;
    }
  }

  public void HandleNoTouch() {
    UpperText.text = UpperError;
    LowerText.text = LowerError;
    _upperTextRect.sizeDelta = new Vector2(_upperTextRect.sizeDelta.x, _upperRectHeight / 2);
    UpperText.color = Color.white;
    LowerText.color = Color.red;
  }

  private void Awake() {
    UpperText.text = UpperIdle;
    LowerText.text = LowerIdle;

    _upperTextRect = UpperText.GetComponent<RectTransform>();
    _upperRectHeight = _upperTextRect.sizeDelta.y;
  }
}

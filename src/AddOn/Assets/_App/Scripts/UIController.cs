using Ideum.Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIController : MonoBehaviour {
  public HoverStates State;
  private bool _selected = false;
  public bool Selected {
    get { return _selected; }
  }

  private List<IStateAnimator> _animators;
  private Coroutine _clickRoutine;
  private bool _coroutineRunning;
  private bool _touchPresent = false;
  private bool _touchWarningActive = false;

  public void DoStateChange(HoverStates state, bool selected) {
    if (State == state && _selected == selected) return;
    if (state == HoverStates.Click && State == HoverStates.Click && !selected) return;
    if (_coroutineRunning || _touchWarningActive) return;

    State = state;
    _selected = selected;

    if(selected && state == HoverStates.Click) {
      _clickRoutine = StartCoroutine(SelectedTimer());
    } else if (_clickRoutine != null) {
      StopCoroutine(_clickRoutine);
      _clickRoutine = null;
      _coroutineRunning = false;
    }

    Log.Info(state);
    
    foreach(IStateAnimator animator in _animators) {
      animator.AnimateTo(state, selected);
    }   
  }

  public void NoTouchWarning(bool noTouch) {
    if(noTouch && !_touchPresent) {
      _touchPresent = true;
      _touchWarningActive = true;
      foreach (IStateAnimator animator in _animators) {
        animator.HandleNoTouch();
      }
      StartCoroutine(WarningTimer());
    } else if (!noTouch && _touchPresent) {
      _touchPresent = false;
    }
  }

  private IEnumerator SelectedTimer() {
    _coroutineRunning = true;
    yield return new WaitForSeconds(2);
    _coroutineRunning = false;
    DoStateChange(State, false);
  }

  private IEnumerator WarningTimer() {
    while (_touchPresent) {
      yield return new WaitForSeconds(0.5f);
    }
    _touchWarningActive = false;
    foreach (IStateAnimator animator in _animators) {
      animator.AnimateTo(State, _selected);
    }
  }

  private void Awake() {
    _animators = GetComponentsInChildren<IStateAnimator>().ToList();
    Log.Info("FOUND " + _animators.Count + " animators.");

    DoStateChange(HoverStates.None, false);
  }
}
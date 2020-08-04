using Ideum.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStateAnimator
{
  void AnimateTo(HoverStates state, bool clicked);
  void HandleNoTouch();
}

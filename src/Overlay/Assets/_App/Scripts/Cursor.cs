using Ideum.Data;
using UnityEngine;

public abstract class Cursor : MonoBehaviour
{
  public abstract void DoStateChange(HoverStates state, bool selected);

  public abstract void ShowNoTouch();
}

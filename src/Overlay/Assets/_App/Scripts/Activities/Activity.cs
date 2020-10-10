using Ideum.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ideum {
  public abstract class Activity : MonoBehaviour {

    public Action Completed;
    public Action<HandInfo> ChangeTableUI;

    public abstract void Activate(float delay);
    public abstract void Deactivate();
  }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TouchlessDesignCore.Examples
{
  public class TouchlessSample : MonoBehaviour
  {
    public TestCursor CursorPrefab;
    private Color[] UserColors;

    private void Start()
    {
      UserColors = new Color[] { Color.green, Color.yellow, Color.blue, Color.cyan, Color.white };
      if (TouchlessDesign.Instance.isStarted)
      {
        HandleTouchlessDesignStarted();
      }
      else
      {
        TouchlessDesign.OnStarted += HandleTouchlessDesignStarted;
      }
      TouchlessDesign.OnStopped += HandleTouchlessDesignStop;

    }
    private void HandleTouchlessDesignStarted()
    {
      Debug.Log("Touchless Design Started.");
      int index = 0;
      foreach (TouchlessUser user in TouchlessDesign.Instance.Users.Values)
      {
        var cursor = Instantiate(CursorPrefab, TouchlessDesign.Instance.Canvas.transform);
        cursor.SetTouchlessUser(user);
        cursor.Image.color = UserColors[index];
        index++;
      }
    }

    private void HandleTouchlessDesignStop()
    {
      Debug.Log("Touchless Design Stopped.");
    }

    private void HandleUserRemoved(TouchlessUser user)
    {
      throw new NotImplementedException();
    }

    private void Update()
    {

    }
  }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchlessUserInfo
{
  /// <summary>
  /// The unique id of this player
  /// </summary>
  public int Id;

  /// <summary>
  /// The expected ip address of this player
  /// </summary>
  public string IpAddress;

  /// <summary>
  /// Width of the user's play area
  /// </summary>
  public float BoundsWidth;

  /// <summary>
  /// Height of the user's play area
  /// </summary>
  public float BoundsHeight;

  /// <summary>
  /// X Offset of the user's play area
  /// </summary>
  public float BoundsX;

  /// <summary>
  /// Y offset of the user's play area
  /// </summary>
  public float BoundsY;
}

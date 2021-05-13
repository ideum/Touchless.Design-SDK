using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TouchlessDesignCore.Examples
{
  [RequireComponent(typeof(Image))]
  [RequireComponent(typeof(RectTransform))]

  public class TestCursor : MonoBehaviour
  {
    public Image Image { get; private set; }
    private TouchlessUser _touchlessUser;
    private RectTransform _rectTransform;
    private Text _userNumberText;

    private void Awake()
    {
      Image = GetComponent<Image>();
      _rectTransform = GetComponent<RectTransform>();
      _userNumberText = GetComponentInChildren<Text>();
    }

    public void SetTouchlessUser(TouchlessUser user)
    {
      _touchlessUser = user;
      _userNumberText.text = user.UserInfo.Id.ToString();
      user.HoverStateChanged += HandleHoverStateChanged;
    }

    private void HandleHoverStateChanged(HoverStates arg1, HoverStates arg2)
    {
      Debug.Log($"Hoverstate changed from {arg1} to {arg2}");
    }

    void Update()
    {
      _rectTransform.anchoredPosition = _touchlessUser.ScreenPosition;
      Image.enabled = _touchlessUser.IsActivated;
    }
  }
}

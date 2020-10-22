using DG.Tweening;
using System;
using TMPro;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;

namespace Ideum {
  public class DragActivity : Activity {
    public CanvasGroup InstructionsCG;
    public CanvasGroup TargetTextCG;
    public CanvasGroup CompleteCG;

    public SVGImage RightOuterRing;
    public SVGImage LeftOuterRing;
    public SVGImage RightInnerCirlce;
    public SVGImage LeftInnerCircle;

    public SVGImage DragImage;

    public Color RingColorInactive;
    public Color RingColorActive;

    public Color RightCircleColorInactive;
    public Color RightCircleColorActive;
    public Color LeftCircleColorInactive;
    public Color LeftCircleColorActive;

    public RectTransform TargetCircle;

    public DetectDrag DragCircle;

    private CanvasGroup _cg;
    private Sequence _seq;

    private bool _active = false;

    private HandInfo _handInfo;

    private RectTransform _leftRing;
    private RectTransform _rightRing;
    private float _baseRingSize;

    private TextMeshProUGUI _instructionText;

    private bool _complete = false;

    private void Awake() {
      _cg = GetComponent<CanvasGroup>();

      _cg.alpha = 0.0f;
      _cg.blocksRaycasts = false;
      _cg.interactable = false;

      DragCircle.DragStart = HandleDragStart;
      DragCircle.DragEnd = HandleDragEnd;
      DragCircle.DragUpdate = HandleDragUpdate;
      DragCircle.HoverEnter = HandleHoverEnter;
      DragCircle.HoverExit = HandleHoverExit;

      _leftRing = LeftOuterRing.GetComponent<RectTransform>();
      _rightRing = RightOuterRing.GetComponent<RectTransform>();

      _baseRingSize = _leftRing.rect.width;
      _instructionText = InstructionsCG.GetComponent<TextMeshProUGUI>();
    }

    public override void Activate(float delay) {
      if (_active) return;

      _active = true;
      RightInnerCirlce.color = RightCircleColorInactive;
      LeftInnerCircle.color = LeftCircleColorInactive;
      CompleteCG.alpha = 0.0f;
      InstructionsCG.alpha = 0.0f;
      TargetTextCG.alpha = 1.0f;
      DragImage.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
      _leftRing.sizeDelta = new Vector2(_baseRingSize, _baseRingSize);
      _rightRing.sizeDelta = new Vector2(_baseRingSize, _baseRingSize);
      _instructionText.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
      DragCircle.ResetRing(true);
      RightOuterRing.color = RingColorInactive;
      RightInnerCirlce.color = RightCircleColorInactive;
      LeftOuterRing.color = RingColorInactive;
      LeftInnerCircle.color = LeftCircleColorInactive;

      _seq?.Kill();
      _seq = DOTween.Sequence();

      DragCircle.Activate();
      _complete = false;

      _seq.AppendInterval(delay);
      _seq.Append(_cg.DOFade(1.0f, 0.5f));
      _seq.Join(InstructionsCG.DOFade(1.0f, 0.25f));
      _seq.OnComplete(() => {
        _cg.blocksRaycasts = true;
        _cg.interactable = true;
        _handInfo = new HandInfo();
        _handInfo.State = HandState.Close;
        ChangeTableUI?.Invoke(_handInfo);
      });
    }

    public override void Deactivate() {
      if (!_active) return;

      _active = false;

      _seq?.Kill();
      _seq = DOTween.Sequence();

      _handInfo = new HandInfo();
      ChangeTableUI?.Invoke(_handInfo);

      _cg.interactable = false;
      _cg.blocksRaycasts = false;
      _seq.Append(_cg.DOFade(0.0f, 0.5f));
    }

    private void CompleteAnimate() {
      _seq?.Kill();
      _seq = DOTween.Sequence();

      _seq.Append(InstructionsCG.DOFade(0.0f, 0.25f));
      _seq.Join(TargetTextCG.DOFade(0.0f, 0.25f));
      _seq.Join(DragImage.DOFade(0.0f, 0.25f));
      _seq.Join(RightOuterRing.DOColor(RingColorActive, 0.25f));
      _seq.Join(LeftOuterRing.DOColor(RingColorActive, 0.25f));
      _seq.Join(RightInnerCirlce.DOColor(RightCircleColorActive, 0.25f));
      _seq.Join(LeftInnerCircle.DOColor(LeftCircleColorActive, 0.25f));
      _seq.Append(CompleteCG.DOFade(1.0f, 0.25f));

      _seq.AppendInterval(1.0f);
      _seq.OnComplete(() => {
        Completed?.Invoke();
        _handInfo = new HandInfo();
        _handInfo.State = HandState.Close;
        ChangeTableUI?.Invoke(_handInfo);
      });
    }

    private void HandleHoverExit() {
      if (!_active) return;

      TouchlessDesign.SetHoverState(Data.HoverStates.None, 1);
    }

    private void HandleHoverEnter() {
      if (!_active) return;

      TouchlessDesign.SetHoverState(Data.HoverStates.Drag, 1);
    }

    private void HandleDragUpdate(Vector2 pos) {
      float difference = (TargetCircle.rect.width - DragCircle.Target.rect.width) / 2;

      if (DragCircle.Target.anchoredPosition.x >= TargetCircle.anchoredPosition.x - difference && DragCircle.Target.anchoredPosition.y >= TargetCircle.anchoredPosition.y - difference &&
        DragCircle.Target.anchoredPosition.x <= TargetCircle.anchoredPosition.x + difference && DragCircle.Target.anchoredPosition.y <= TargetCircle.anchoredPosition.y + difference) {
        CompleteAnimate();
        _complete = true;
        DragCircle.Deactivate();
        return;
      }
    }

    private void HandleDragEnd() {
      if (_complete) return;

      _seq?.Kill();
      _seq = DOTween.Sequence();

      _seq.Append(_leftRing.DOSizeDelta(new Vector2(_baseRingSize, _baseRingSize), 0.5f));
      _seq.Join(_rightRing.DOSizeDelta(new Vector2(_baseRingSize, _baseRingSize), 0.5f));
      _seq.Join(LeftInnerCircle.DOColor(LeftCircleColorInactive, 0.5f));
      _seq.Join(_instructionText.DOColor(new Color(1.0f, 1.0f, 1.0f, 1.0f), 0.5f));
      _seq.Join(DragImage.DOColor(new Color(1.0f, 1.0f, 1.0f, 1.0f), 0.5f));

      DragCircle.ResetRing();

      HandInfo info = new HandInfo();
      info.SensorHighlighted = false;
      info.State = HandState.Close;
      ChangeTableUI?.Invoke(info);
    }

    private void HandleDragStart() {
      _seq?.Kill();
      _seq = DOTween.Sequence();

      _seq.Append(_leftRing.DOSizeDelta(new Vector2(_baseRingSize - 100, _baseRingSize - 100), 0.5f));
      _seq.Join(_rightRing.DOSizeDelta(new Vector2(_baseRingSize + 100, _baseRingSize + 100), 0.5f));
      _seq.Join(LeftInnerCircle.DOColor(RingColorInactive, 0.5f));
      _seq.Join(_instructionText.DOColor(new Color(0.0f, 0.0f, 0.0f, 1.0f), 0.5f));
      _seq.Join(DragImage.DOColor(new Color(0.0f, 0.0f, 0.0f, 1.0f), 0.5f));

      HandInfo info = new HandInfo();
      info.SensorHighlighted = true;
      info.State = HandState.Close;
      ChangeTableUI?.Invoke(info);
    }
  }
}

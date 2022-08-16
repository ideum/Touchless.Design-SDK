using DG.Tweening;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;

namespace Ideum {
  public class ClickActivity : Activity {

    public CanvasGroup InstructionsCG;
    public CanvasGroup CompleteCG;

    public SVGImage OuterRing;
    public SVGImage InnerCircle;

    public Color RingColorInactive;
    public Color RingColorActive;

    public Color CircleColorInactive;
    public Color CircleColorActive;

    public DetectHover Hover;

    public Button Button;

    private CanvasGroup _cg;
    private Sequence _seq;

    private bool _active = false;

    private HandInfo _handInfo;

    private void Awake() {
      _cg = GetComponent<CanvasGroup>();

      _cg.alpha = 0.0f;
      _cg.blocksRaycasts = false;
      _cg.interactable = false;

      _handInfo = new HandInfo();

      Button.onClick.AddListener(HandleClick);
    }

    public override void Activate(float delay) {
      if (_active) return;

      _active = true;
      OuterRing.color = new Color(RingColorInactive.r, RingColorInactive.g, RingColorInactive.b, 0.0f);
      InnerCircle.color = CircleColorInactive;
      CompleteCG.alpha = 0.0f;
      InstructionsCG.alpha = 0.0f;

      _seq?.Kill();
      _seq = DOTween.Sequence();

      _handInfo = new HandInfo();
      _handInfo.Animating = true;
      _handInfo.AnimationStates.Add(HandState.Select);
      _handInfo.AnimationStates.Add(HandState.Point);
      ChangeTableUI?.Invoke(_handInfo);

      _seq.AppendInterval(delay);
      _seq.Append(_cg.DOFade(1.0f, 0.5f));
      _seq.Append(OuterRing.DOColor(RingColorInactive, 0.25f));
      _seq.Join(InstructionsCG.DOFade(1.0f, 0.25f));
      _seq.OnComplete(() => {
        _cg.blocksRaycasts = true;
        _cg.interactable = true;

        Hover.HoverEnter = HandlePointerEnter;
        Hover.HoverExit = HandlePointerExit;
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

    private void HandlePointerExit(int pointerId) {
      var user = TouchlessDesign.GetTouchlessUserFromPointerID(pointerId);
      if (!_active || user == null) return;

      TouchlessDesign.SetHoverState(user.DeviceId, Data.HoverStates.None, 1);
    }

    private void HandlePointerEnter(int pointerId) {
      var user = TouchlessDesign.GetTouchlessUserFromPointerID(pointerId);
      if (!_active || user == null) return;

      TouchlessDesign.SetHoverState(user.DeviceId, Data.HoverStates.Click, 1);
    }

    private void HandleClick() {
      if (!_active) return;

      _handInfo = new HandInfo();
      _handInfo.State = HandState.Select;
      _handInfo.SensorHighlighted = true;
      ChangeTableUI?.Invoke(_handInfo);

      CompleteAnimate();
    }

    private void CompleteAnimate() {
      _seq?.Kill();
      _seq = DOTween.Sequence();

      _seq.Append(InstructionsCG.DOFade(0.0f, 0.25f));
      _seq.Join(OuterRing.DOColor(RingColorActive, 0.25f));
      _seq.Join(InnerCircle.DOColor(CircleColorActive, 0.25f));
      _seq.Append(CompleteCG.DOFade(1.0f, 0.25f));

      _seq.AppendInterval(1.0f);
      _seq.OnComplete(() => {
        Completed?.Invoke();
        _handInfo = new HandInfo();
        ChangeTableUI?.Invoke(_handInfo);
      });
    }
  }
}

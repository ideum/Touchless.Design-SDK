using DG.Tweening;
using TMPro;
using Unity.VectorGraphics;
using UnityEngine;

namespace Ideum {
  public class PointActivity : Activity {

    public TextMeshProUGUI Counter;
    public CanvasGroup InstructionsCG;
    public CanvasGroup CompleteCG;

    public SVGImage OuterRing;
    public SVGImage InnerCircle;

    public Color RingColorInactive;
    public Color RingColorActive;

    public Color CircleColorInactive;
    public Color CircleColorActive;

    public DetectHover Hover;

    private CanvasGroup _cg;
    private Sequence _seq;

    private bool _active = false;
    private int _counter = 0;
    private bool _enterFlag = false;

    private HandInfo _handInfo;

    private void Awake() {
      _cg = GetComponent<CanvasGroup>();

      _cg.alpha = 0.0f;
      _cg.blocksRaycasts = false;
      _cg.interactable = false;

      _handInfo = new HandInfo();
    }

    public override void Activate(float delay) {
      if (_active) return;

      _active = true;
      _counter = 0;
      OuterRing.color = new Color(RingColorInactive.r, RingColorInactive.g, RingColorInactive.b, 0.0f);
      InnerCircle.color = CircleColorInactive;
      Counter.text = "" + _counter;
      CompleteCG.alpha = 0.0f;
      InstructionsCG.alpha = 0.0f;

      _handInfo = new HandInfo();
      ChangeTableUI?.Invoke(_handInfo);

      _seq?.Kill();
      _seq = DOTween.Sequence();

      _seq.AppendInterval(delay);
      _seq.Append(_cg.DOFade(1.0f, 0.5f));
      _seq.Append(OuterRing.DOColor(RingColorInactive, 0.25f));
      _seq.Join(InstructionsCG.DOFade(1.0f, 0.25f));
      _seq.Join(Counter.DOFade(1.0f, 0.25f));
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
      if (!_active || !_enterFlag) return;

      _handInfo = new HandInfo();
      _handInfo.MovementVectorShown = true;
      ChangeTableUI?.Invoke(_handInfo);

      _enterFlag = false;

      _seq?.Kill();
      _seq = DOTween.Sequence();

      _seq.Append(OuterRing.DOColor(RingColorInactive, 0.5f));
      _seq.Join(InnerCircle.DOColor(CircleColorInactive, 0.5f));
    }

    private void HandlePointerEnter(int pointerId) {
      if (!_active || _enterFlag) return;

      _handInfo = new HandInfo();
      _handInfo.MovementVectorShown = true;
      _handInfo.SensorHighlighted = true;
      ChangeTableUI?.Invoke(_handInfo);

      _counter++;
      if(_counter > 1) {
        CompleteAnimate();
        return;
      }

      Counter.text = "" + _counter;
      _enterFlag = true;

      _seq?.Kill();
      _seq = DOTween.Sequence();

      _seq.Append(OuterRing.DOColor(RingColorActive, 0.5f));
      _seq.Join(InnerCircle.DOColor(CircleColorActive, 0.5f)); 
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
        _handInfo = new HandInfo();
        ChangeTableUI?.Invoke(_handInfo);
        Completed?.Invoke();
      });
    }
  }
}
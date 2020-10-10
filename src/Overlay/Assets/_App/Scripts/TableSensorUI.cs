using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VectorGraphics;
using UnityEngine;

namespace Ideum {
  public class TableSensorUI : MonoBehaviour {

    public SVGImage TableGraphic;
    public Sprite AddonGraphicSprite;
    public Sprite PedestalGraphicSprite;

    public List<CanvasGroup> HandImages;

    public CanvasGroup MovementVectorImage;

    public TextMeshProUGUI SensorText;
    public SVGImage SensorBar;

    public Color ActiveColor;
    public Color InactiveColor;

    private Sequence _seq;

    public void Initialize(bool isPedestal) {
      SetDefaults();

      TableGraphic.sprite = isPedestal ? PedestalGraphicSprite : AddonGraphicSprite;
    }

    public void ChangeState(HandInfo info) {
      _seq?.Kill();
      _seq = DOTween.Sequence();

      _seq.Join(MovementVectorImage.DOFade(info.MovementVectorShown ? 1.0f : 0.0f, 0.5f));
      _seq.Join(SensorText.DOColor(info.SensorHighlighted ? ActiveColor : InactiveColor, 0.5f));
      _seq.Join(SensorBar.DOColor(info.SensorHighlighted ? ActiveColor : InactiveColor, 0.5f));

      _seq.Join(HandImages[0].DOFade(info.State == HandState.Point ? 1.0f : 0.0f, 0.5f));
      _seq.Join(HandImages[1].DOFade(info.State == HandState.Select ? 1.0f : 0.0f, 0.5f));
      _seq.Join(HandImages[2].DOFade(info.State == HandState.Close ? 1.0f : 0.0f, 0.5f));
 
      if(info.Animating && info.AnimationStates.Count > 1) {
        for (int i = 0; i < info.AnimationStates.Count; i++) {
          _seq.AppendInterval(2.0f);
          _seq.Append(HandImages[0].DOFade(info.AnimationStates[i] == HandState.Point ? 1.0f : 0.0f, 0.5f));
          _seq.Join(HandImages[1].DOFade(info.AnimationStates[i] == HandState.Select ? 1.0f : 0.0f, 0.5f));
          _seq.Join(HandImages[2].DOFade(info.AnimationStates[i] == HandState.Close ? 1.0f : 0.0f, 0.5f));
        }
      }

      if (info.Animating) {
        _seq.OnComplete(() => {
          ChangeState(info);
        });
      }
    }

    public void SetDefaults() {
      for(int i = 0; i < HandImages.Count; i++) {
        if(i == 0) {
          HandImages[i].alpha = 1.0f;
        } else {
          HandImages[i].alpha = 0.0f;
        }
      }

      MovementVectorImage.alpha = 0.0f;

      SensorText.color = InactiveColor;
      SensorBar.color = InactiveColor;
    }
  }
}

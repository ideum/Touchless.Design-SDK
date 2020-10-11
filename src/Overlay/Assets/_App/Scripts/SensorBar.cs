using System.Collections;
using System.Collections.Generic;
using Unity.VectorGraphics;
using UnityEngine;

namespace Ideum {

  public class SensorBar : MonoBehaviour {

    public SVGImage Background;

    public Color ActiveColor;
    public Color InactiveColor;

    private RectTransform _rect;

    private void Awake() {
      _rect = GetComponent<RectTransform>();

      Background.color = InactiveColor;
    }

    public void Resize(float scale) {
      _rect.localScale = new Vector3(scale, scale, scale);
    }

    public void SetStatus(bool active) {
      Background.color = active ? ActiveColor : InactiveColor;
    }
  }
}

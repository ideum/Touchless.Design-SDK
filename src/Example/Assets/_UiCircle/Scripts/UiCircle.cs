using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Ideum {
  [ExecuteInEditMode]
  [RequireComponent(typeof(RectTransform))]
  public class UiCircle : UIBehaviour {

    public float BorderWidth = 0;

    [Range(0,1)]
    public float Fuzziness = 0.03f;

    private RectTransform _transform;
    private Graphic _graphic;
    private Shader _shader;
    private Material _material;

    private static Sprite _defaultSprite;

    private static Sprite DefaultSprite {
      get {
        if (_defaultSprite != null) return _defaultSprite;
        var tex = Texture2D.whiteTexture;
        _defaultSprite = Sprite.Create(tex, new Rect(0,0, tex.width,tex.height), new Vector2(0.5f,0.5f));
        return _defaultSprite;
      }
    }

    private RectTransform GetTransform() {
      if (_transform == null) {
        _transform = GetComponent<RectTransform>();
      }
      return _transform;
    }

    private Material GetMaterial() {
      if (_shader == null) {
        _shader = Shader.Find("Ideum/Ui Circle");
      }

      if (_material == null) {
        _material = new Material(_shader) { name = "Ui Circle" };
      }
      return _material;
    }

    private Graphic GetGraphic() {
      if (_graphic == null) {
        _graphic = GetComponent<Graphic>();
      }
      return _graphic;
    }

    protected override void Awake() {
      base.Awake();
      Init();
    }

    protected override void OnEnable() {
      base.OnEnable();
      Init();
    }

    private void Init() {
      GetMaterial();

      GetGraphic();

      GetTransform();

      _material.mainTexture = _graphic.mainTexture;
      _graphic.material = _material;
    }

    void Update() {
      var xform = GetTransform();
      if (_material == null || xform==null) return;
      var dim = _transform.localToWorldMatrix.MultiplyVector(_transform.rect.size);
      var d = Mathf.Clamp01(BorderWidth / dim.x / 2);
      _material.SetFloat("_BorderWidth", d);
      _material.SetFloat("_Fuzziness", Fuzziness);
    }

    protected override void OnDestroy() {
      base.OnDestroy();
      if (_material == null) return;
#if UNITY_EDITOR
      DestroyImmediate(_material);
#else
      Destroy(_material);
#endif
      _material = null;
    }


    public Color color {
      get {
        return GetMaterial().color;
      }
      set { GetMaterial().color = color; }
    }

    public TweenerCore<Color, Color, ColorOptions> DOColor(Color endValue, float duration) {
      return GetGraphic().DOColor(endValue, duration);
    }
  }
}
using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Ideum {
  public class StagePanel : Panel {

    [Serializable]
    public class CameraAnchorsData {
      public Transform Origin, Zeus, Athena, Hercules;
    }

    public TouchlessDragSurface Surface;
    public Camera Camera;
    public StageItemModel ZeusModel, AthenaModel, HerculesModel;

    [Header("Anchors")]
    public CameraAnchorsData CameraAnchors;


    private Dictionary<StageItemModel, Transform> _cameraAnchorMap;
    private StageItemModel[] _models;


    public override void Init() {

      _cameraAnchorMap = new Dictionary<StageItemModel, Transform>() {
        { ZeusModel, CameraAnchors.Zeus},
        { AthenaModel, CameraAnchors.Athena },
        { HerculesModel, CameraAnchors.Hercules }
      };

      _models = new[] {ZeusModel, AthenaModel, HerculesModel};
      
      foreach (var m in _models) {
        m.Init();
      }

      Surface.gameObject.SetActive(false);
      Surface.Dragging += Surface_Dragging;
    }
    
    private void Surface_Dragging() {
      if (App.SelectedItem == null) return;
      var m = App.SelectedItem.Model;
      var delta = Surface.CurrentDelta;

      var xNorm = delta.x / 3840f;
      var rotY = m.Pivot.transform.localEulerAngles.y + -xNorm*360;

      m.Pivot.transform.localEulerAngles = new Vector3(0, rotY, 0);
    }

    public override void AppChangedSelection(ItemData selection) {
      if (selection == null) {
        Surface.gameObject.SetActive(false);
        Camera.transform.position = CameraAnchors.Origin.position;
        foreach (var m in _models) {
          m.transform.position = m.OriginalPosition;
          m.transform.localEulerAngles = Vector3.zero;
          m.Pivot.transform.localEulerAngles = Vector3.zero;
          m.gameObject.SetActive(true);
        }
      }
      else {
        Surface.gameObject.SetActive(true);
        if (_cameraAnchorMap.TryGetValue(selection.Model, out var a)) {
          Camera.transform.position = a.position;
          foreach (var m in _models) {
            m.gameObject.SetActive(m == selection.Model);
            m.transform.localEulerAngles = Vector3.zero;
            m.Pivot.transform.localEulerAngles = Vector3.zero;
          }
        }
        else {
          Log.Error("No matching anchor for this selection");
        }
      }
    }
  }
}

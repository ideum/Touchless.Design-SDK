using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEditor;
using UnityEngine;

namespace Ideum {
  public class StagePanel : Panel {

    [Serializable]
    public class CameraAnchorsData {
      public Transform Origin, Zeus, Athena, Hercules;
    }

    public float rotationSensativityX = 5f;
    public float rotationSensativityY = 10f;
    public TouchlessDragSurface Surface;
    public Camera Camera;
    public StageItemModel ZeusModel, AthenaModel, HerculesModel;

    [Header("Anchors")]
    public CameraAnchorsData CameraAnchors;


    private Dictionary<StageItemModel, Transform> _cameraAnchorMap;
    private StageItemModel[] _models;

    private bool zoomedOut = true;
    private StageItemModel lastModel;

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

    float rotX;
    float rotY;
    private void Surface_Dragging() {
      if (App.SelectedItem == null) return;
      var m = App.SelectedItem.Model;
      var delta = Surface.CurrentDelta;

      rotX += delta.y / rotationSensativityX;
      rotY += -delta.x / rotationSensativityY;

      rotX = KeepInCheck(rotX);
      rotY = KeepInCheck(rotY);

      m.Pivot.transform.localEulerAngles = new Vector3(rotX, rotY, 0);

      //var xNorm = delta.x / 3840f;
      //var yNorm = delta.y / 2160f;

      ////Acts weird at 90 and -90 degrees
      //var rotX = m.Pivot.transform.localEulerAngles.x + yNorm * 360;
      ////rotX = KeepInCheck(rotX);

      ////Works
      //var rotY = m.Pivot.transform.localEulerAngles.y + -xNorm*360;
      //rotY = KeepInCheck(rotY);

      //m.Pivot.transform.localEulerAngles = new Vector3(rotX, rotY, 0);
    }

    private float KeepInCheck(float rotX)
    {
      if (rotX < 0) rotX += 360;
      else if (rotX > 360) rotX -= 360;

      return rotX;
    }

    public override void AppChangedSelection(ItemData selection) {
      if (selection == null) {
        zoomedOut = true;
        Surface.gameObject.SetActive(false);
        //Camera.transform.DOMove(CameraAnchors.Origin.position, 0.5f);
        //Camera.transform.position = CameraAnchors.Origin.position;
        foreach (var m in _models) {
          //transition current bust back to og position
          if(m == lastModel)
          {
            m.transform.DOScale(1f, 0.5f);
            m.transform.DOLocalMove(m.OriginalPosition, 0.5f);
            m.transform.DOLocalRotate(Vector3.zero, 0.5f);
            m.Pivot.transform.DOLocalRotate(Vector3.zero, 0.5f);
          }
          //other busts are already reset and waiting
          else
          {
            m.transform.localScale = Vector3.one;
            m.transform.localPosition = m.OriginalPosition;
            m.transform.localEulerAngles = Vector3.zero;
            m.Pivot.transform.localEulerAngles = Vector3.zero;
          }
          
          m.gameObject.SetActive(true);
        }
      }
      else {
        Surface.gameObject.SetActive(true);
        if (_cameraAnchorMap.TryGetValue(selection.Model, out var a)) {
          //Camera.transform.DOMove(a.position, 0.8f);
          //Camera.transform.position = a.position;
          lastModel = selection.Model;
          foreach (var m in _models) {
            m.gameObject.SetActive(m == selection.Model);
            m.transform.localEulerAngles = Vector3.zero;
            m.Pivot.transform.localEulerAngles = Vector3.zero;
            m.transform.DOScale(1.3f, 0.5f);

            //select new bust while viewing bust
            if (!zoomedOut) 
            {
              m.transform.DOLocalMoveX(m == selection.Model ? m.selectedPosX : 5f, 0.5f);
            }
            //select bust from 'home' screen
            else {
              float offscreen = m.changeDirs ? (selection.Model.offscreenPosX < 0 ? 1 : -1) * m.offscreenPosX : m.offscreenPosX;
              m.transform.DOLocalMoveX(m == selection.Model ? m.selectedPosX : offscreen, 0.5f);
            }
          }
          zoomedOut = false;
        }
        else {
          Log.Error("No matching anchor for this selection");
        }
      }
    }
  }
}

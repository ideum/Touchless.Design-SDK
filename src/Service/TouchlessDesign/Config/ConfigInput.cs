using System;
using System.IO;

namespace TouchlessDesign.Config {
  public class ConfigInput {


    public string ToggleEmulationKeyCombination = "Control+Alt+I";
    public double ToggleEmulationToggleSpeed_ms = 1500;
    
    public float GrabClickThreshold = 0.75f;
    public int ClickDuration_ms = 500;
    public bool ClickEnabled = true;

    public int UpdateRate_ms = 9;
    public string InputProvider = "LeapMotionProvider";

    public bool FlipYAxis = true;
    
    public bool UseXY = false;
    
    public float MinX = -0.35f;
    public float MaxX =  0.35f;

    public float MinZ = -0.20f;
    public float MaxZ =  0.20f;
    
    public float MinY =  0.10f;
    public float MaxY =  0.60f;

    public float MinConfidence = 0.0f;


    #region Utility Methods

    public float Width() {
      return Math.Abs(MaxX  - MinX);
    }

    public float Height() {
      return Math.Abs(MaxY - MinY);
    }

    public float Depth() {
      return Math.Abs(MaxZ - MinZ);
    }

    public float ClampX(float x) {
      return Clamp(x, MinX, MaxX );
    }

    public float ClampY(float y) {
      return Clamp(y, MinY, MaxY);
    }

    public float ClampZ(float z) {
      return Clamp(z, MinZ, MaxZ);
    }

    private static float Clamp(float value, float min, float max) {
      if (value < min) return min;
      if (value > max) return max;
      return value;
    }

    public float NormalizedX(float worldX) {
      worldX = ClampX(worldX);
      return (worldX - MinX) / Width();
    }

    public float NormalizedY(float worldY) {
      worldY = ClampY(worldY);
      return (worldY - MinY) / Height();
    }

    public float NormalizedZ(float worldZ) {
      worldZ = ClampZ(worldZ);
      return (worldZ - MinZ) / Depth();
    }

    public void NormalizedPosition(float x, float y, float z, out float horizontal, out float vertical) {
      horizontal = NormalizedX(x);
      vertical = UseXY ? NormalizedY(y) : NormalizedZ(z);
      if (FlipYAxis) {
        vertical = 1 - vertical;
      }
    }

    #endregion

    private const string Filename = "input.json";

    public static ConfigInput Get(string dir) {
      var path = Path.Combine(dir, Filename);
      return Factory.Get(path, ()=>new ConfigInput());
    }

    public void Save(string dir) {
      var path = Path.Combine(dir, Filename);
      Factory.Save(path, this);
    }

    public void Apply(ConfigInput i) {
      ToggleEmulationKeyCombination = i.ToggleEmulationKeyCombination;
      ToggleEmulationToggleSpeed_ms = i.ToggleEmulationToggleSpeed_ms;
      GrabClickThreshold = i.GrabClickThreshold;
      UpdateRate_ms = i.UpdateRate_ms;
      InputProvider = i.InputProvider;
      FlipYAxis = i.FlipYAxis;
      UseXY = i.UseXY;
      MinX = i.MinX;
      MaxX = i.MaxX;
      MinZ = i.MinZ;
      MaxZ = i.MaxZ;
      MinY = i.MinY;
      MaxY = i.MaxY;
      MinConfidence = i.MinConfidence;
    }
  }
}
using System;
using System.Diagnostics;
using System.IO;
using Leap;

namespace TouchlessDesign {
  public class LeapSettings {
    
    private const string Filename = "leap.json";

    public float WorldBoundsLeft = -0.35f;
    public float WorldBoundsRight = 0.35f;
    public float WorldBoundsBack = -0.35f;
    public float WorldBoundsFront = 0.25f;
    public float WorldBoundsBottom = 0.1f;
    public float WorldBoundsTop = 0.6f;
    public float GrabClickThreshold = 0.75f;
    public bool FlipYAxis = false;
    public bool UseXY = false;
    public int UpdateRate_ms = 9;

    public float Width() {
      return Math.Abs(WorldBoundsRight - WorldBoundsLeft);
    }

    public float Height() {
      return Math.Abs(WorldBoundsTop - WorldBoundsBottom);
    }

    public float Depth() {
      return Math.Abs(WorldBoundsFront - WorldBoundsBack);
    }

    public float ClampX(float x) {
      return Clamp(x, WorldBoundsLeft, WorldBoundsRight);
    }

    public float ClampY(float y) {
      return Clamp(y, WorldBoundsBottom, WorldBoundsTop);
    }

    public float ClampZ(float z) {
      return Clamp(z, WorldBoundsBack, WorldBoundsFront);
    }

    private static float Clamp(float value, float min, float max) {
      if (value < min) return min;
      if (value > max) return max;
      return value;
    }

    public float NormalizedX(float worldX) {
      worldX = ClampX(worldX);
      return (worldX - WorldBoundsLeft) / Width();
    }

    public float NormalizedY(float worldY) {
      worldY = ClampY(worldY);
      return (worldY - WorldBoundsBottom) / Height();
    }

    public float NormalizedZ(float worldZ) {
      worldZ = ClampZ(worldZ);
      return (worldZ - WorldBoundsBack) / Depth();
    }

    public void NormalizedPosition(Vector p, out float horizontal, out float vertical) {
      horizontal = NormalizedX(p.x);
      vertical = UseXY ? NormalizedY(p.y) : NormalizedZ(p.z);
      if (FlipYAxis) {
        vertical = 1 - vertical;
      }
    }

    public static LeapSettings Get(string dir) {
      var path = Path.Combine(dir, Filename);
      return ConfigFactory.Get(path, Defaults);
    }

    public void Save(string dir) {
      var path = Path.Combine(dir, Filename);
      ConfigFactory.Save(path, this);
    }

    public static LeapSettings Defaults() {
      return new LeapSettings {
        WorldBoundsLeft = -0.35f,
        WorldBoundsRight = 0.35f,
        WorldBoundsBack = -0.35f,
        WorldBoundsFront = 0.25f,
        WorldBoundsBottom = 0.1f,
        WorldBoundsTop = 0.6f,
        GrabClickThreshold = 0.75f,
        FlipYAxis = false,
        UseXY = false,
        UpdateRate_ms = 9
      };
    }

  }
}
namespace TouchlessDesign {


  public interface IHotspot {

    /// <summary>
    /// The unique Id of this hotspot
    /// </summary>
    int Id { get; }

    /// <summary>
    /// Checks to see if the provided screen-space pixel coordinates are contained by this IHotspot instance
    /// </summary>
    /// <param name="x">screen-space x pixel coordinate</param>
    /// <param name="y">screen-space y pixel coordinate</param>
    /// <returns>true if the provided screen-space coordinates are contained by this IHotspot instance, false otherwise.</returns>
    bool ContainsPixel(int x, int y);

    /// <summary>
    /// The screen space pixel y-coordinate of the center point of this IHotspot
    /// </summary>
    int PixelX { get; }

    /// <summary>
    /// The screen space pixel y-coordinate of the center point of this IHotspot
    /// </summary>
    int PixelY { get; }
  }
}
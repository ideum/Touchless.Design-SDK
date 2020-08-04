namespace TouchlessDesign {
  public interface ICursor {
    
    /// <summary>
    /// True if cursor emulation is enabled, false otherwise
    /// </summary>
    bool IsEmulationEnabled { get; }

    /// <summary>
    /// True if the left mouse button is currently in a down state
    /// </summary>
    bool IsButtonDown { get; }

    /// <summary>
    /// the left-most pixel of the usable area of the cursor in screen coordinates
    /// </summary>
    int BoundsLeft { get; }

    /// <summary>
    /// the right-most pixel of the usable area of the cursor in screen coordinates
    /// </summary>
    int BoundsRight { get; }

    /// <summary>
    /// the top-most pixel of the usable area of the cursor in screen coordinates
    /// </summary>
    int BoundsTop { get; }

    /// <summary>
    /// the bottom-most pixel of the usable area of the cursor in screen coordinates
    /// </summary>
    int BoundsBottom { get; }
    
    /// <summary>
    /// The pixel-width of the bounding area
    /// </summary>
    int BoundsWidth { get; }
    
    /// <summary>
    /// The pixel-height of the bounding area
    /// </summary>
    int BoundsHeight { get; }

    /// <summary>
    /// Sets the pixel position of the cursor
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    void SetPosition(int x, int y);

    /// <summary>
    /// Sets the cursor's down state
    /// </summary>
    /// <param name="isDown">if true, the cursor's click state will be put in the down state</param>
    void SetMouseButtonDown(bool isDown);

    /// <summary>
    /// Sets the cursor's pixel position and whether or not the cursor should be in a down state.
    /// </summary>
    /// <param name="x">the desired x-coordinate in screen space</param>
    /// <param name="y">the desired y-coordinate in screen space</param>
    /// <param name="isDown">if true, the cursor will be set to a 'clicking' state</param>
    void SetPositionAndButton(int x, int y, bool isDown);

    /// <summary>
    /// Performs a click operation at the current position of the cursor. During this operation, the cursor's position will not change.
    /// </summary>
    void DoClick();
  }
}
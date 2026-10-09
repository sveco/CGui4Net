namespace CGui.Gui.Primitives
{
  /// <summary>
  /// Decides when a console resize is finished. Dragging a window edge changes the size many times a second,
  /// so a new size is reported only after it was seen unchanged on a few polls in a row.
  /// </summary>
  internal sealed class SizeChangeDetector
  {
    private readonly int _stablePolls;
    private int _appliedWidth;
    private int _appliedHeight;
    private int _candidateWidth;
    private int _candidateHeight;
    private int _stableCount;

    /// <param name="width">Size the screen is currently drawn for</param>
    /// <param name="height">Size the screen is currently drawn for</param>
    /// <param name="stablePolls">How many polls in a row must see the same new size before it is reported</param>
    public SizeChangeDetector(int width, int height, int stablePolls = 2)
    {
      _stablePolls = stablePolls < 1 ? 1 : stablePolls;
      Accept(width, height);
    }

    /// <summary>
    /// Call on every poll with the current size.
    /// Returns true once, when a size different from the last reported one has settled.
    /// </summary>
    public bool Poll(int width, int height)
    {
      if (width == _appliedWidth && height == _appliedHeight)
      {
        _candidateWidth = width;
        _candidateHeight = height;
        _stableCount = 0;
        return false;
      }

      if (width == _candidateWidth && height == _candidateHeight)
      {
        _stableCount++;
      }
      else
      {
        _candidateWidth = width;
        _candidateHeight = height;
        _stableCount = 1;
      }

      if (_stableCount < _stablePolls)
      {
        return false;
      }

      _appliedWidth = width;
      _appliedHeight = height;
      _stableCount = 0;
      return true;
    }

    /// <summary>
    /// The screen is drawn for this size, nothing to report. Used when the size was set by the application.
    /// </summary>
    public void Accept(int width, int height)
    {
      _appliedWidth = _candidateWidth = width;
      _appliedHeight = _candidateHeight = height;
      _stableCount = 0;
    }
  }
}

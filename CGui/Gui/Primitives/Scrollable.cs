namespace CGui.Gui.Primitives
{
  using System;

  /// <summary>
  /// Defines the <see cref="Scrollable" /> abstract class.
  /// </summary>
  public abstract class Scrollable : GuiElement, IScrollable
  {
    /// <summary>
    /// Defines the current position character
    /// </summary>
    private string ScrollBarChar = "█";

    /// <summary>
    /// Defines the scrollbar placeholder character
    /// </summary>
    private string ScrollBarCharInactive = "▒";

    /// <summary>
    /// Defines the character to thow first on scrollbar
    /// </summary>
    private string ScrollBarCharUp = "▲";

    /// <summary>
    /// Defines the character to thow last on scrollbar
    /// </summary>
    private string ScrollBarCharDown = "▼";

    /// <summary>
    /// Gets or sets the Offset
    /// </summary>
    public int Offset { get; set; }

    private int _defaultPage = 10;
    public int DefaultPage { get => _defaultPage; set => _defaultPage = value; }

    /// <summary>
    /// Gets the total items to scroll
    /// </summary>
    public abstract int TotalItems { get; }

    /// <summary>
    /// Gets or sets a value indicating whether to show scrollbar
    /// </summary>
    public bool ShowScrollBar { get; set; }

    /// <summary>
    /// Returns formatted display text for item including scrollbar
    /// </summary>
    /// <param name="Index">The zero-based <see cref="int"/>index of item</param>
    /// <param name="DisplayText">The <see cref="string"/> unformatted display text</param>
    /// <returns>The <see cref="string"/></returns>
    protected string GetDisplayText(int Index, string DisplayText)
    {
      switch (this.TextAlignment)
      {
        case TextAlignment.Left:
          DisplayText = DisplayText.PadRight(this.Width - 1 - (BorderWidth * 2), this.PadChar);
          break;

        case TextAlignment.Right:
          DisplayText = DisplayText.PadLeft(this.Width - 1 - (BorderWidth * 2), this.PadChar);
          break;

        case TextAlignment.Center:
          DisplayText = DisplayText.PadBoth(this.Width - 1 - (BorderWidth * 2), this.PadChar);
          break;
      }

      if (DisplayText.VisibleLength() > this.Width - 1)
      {
        if (DisplayText.VisibleLength() > 4)
        {
          DisplayText = DisplayText.Substring(0, this.Width - 4 - (BorderWidth * 2)) + "...";
        }
        else
        {
          DisplayText = DisplayText.Substring(0, this.Width - (BorderWidth * 2));
        }
      }

      var visibleHeight = Height - (BorderWidth * 2);
      if (ShowScrollBar && visibleHeight > 3 && visibleHeight < TotalItems)
      {
        var ratio = (double)visibleHeight / TotalItems;
        int size = Math.Max((int)Math.Ceiling(((double)visibleHeight) * ratio) - 2, 1);
        var top = Math.Ceiling(Offset * ratio);

        bool first = (Index - Offset == 0);
        bool last = Index - Offset == visibleHeight - 1;

        bool show = Index - Offset > top
            && Index - Offset <= size + top;

        if (first)
        {
          DisplayText = DisplayText + ConsoleWrapper.ColorReset + ScrollBarCharUp;
        }
        else if (last)
        {
          DisplayText = DisplayText + ConsoleWrapper.ColorReset + ScrollBarCharDown;
        }
        else if (show)
        {
          DisplayText = DisplayText + ConsoleWrapper.ColorReset + ScrollBarChar;
        }
        else
        {
          DisplayText = DisplayText + ConsoleWrapper.ColorReset + ScrollBarCharInactive;
        }
      }

      return DisplayText;
    }

    public virtual void ScrollUp() {
      ScrollUp(1);
    }
    public virtual void ScrollDown()
    {
      ScrollDown(1);
    }
    public abstract void ScrollUp(int Step);
    public abstract void ScrollDown(int Step);
  }
}

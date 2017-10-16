using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGui.Gui.Primitives
{
  public abstract class Scrollable : GuiElement, IScrollable
  {
    private string ScrollBarChar = "█";
    private string ScrollBarCharInactive = "▒";
    private string ScrollBarCharUp = "▲";
    private string ScrollBarCharDown = "▼";
    
    public int Offset { get; set; }
    public abstract int TotalItems { get; }

    public bool ShowScrollBar { get; set; }

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
        int size = (int)Math.Ceiling(((double)visibleHeight) * ratio) - 2;
        var top = Math.Ceiling(Offset * ratio);

        bool first = (Index - Offset == 0);
        bool last = Index - Offset == visibleHeight - 1;

        bool show = Index - Offset > top
            && Index - Offset < size + top;

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
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGui.Gui.Primitives
{
  /// <summary>
  /// UI element consising of single line across the screen.
  /// </summary>
  public class Row : GuiElement
  {
    private string _displayText;
    public string DisplayText
    {
      get { return _displayText; }
      set
      {
        _displayText = value;
      }
    }
    int _top;
    public override int Top { get => _top; set => _top = value; }
    public override int Left { get => 0; set { } }
    public override int Width { get => ConsoleWrapper.Instance.WindowWidth - 2; set { } }
    public override int Height { get => 1; set { } }

    protected string FormatDisplayText(string displayText)
    {
      string result = displayText;
      switch (this.TextAlignment)
      {
        case TextAlignment.Left:
          result = result.PadRight(this.Width - this.BorderWidth + 2, this.PadChar);
          break;

        case TextAlignment.Right:
          result = result.PadLeft(this.Width - this.BorderWidth + 2, this.PadChar);
          break;

        case TextAlignment.Center:
          result = result.PadBoth(this.Width - this.BorderWidth + 2, this.PadChar);
          break;
      }
      return result;
    }

    protected override void RenderControl()
    {
      lock (ConsoleWrapper.Instance.Lock)
      {
        ConsoleWrapper.Instance.CursorVisible = false;
        ConsoleWrapper.Instance.SaveColor();

        ConsoleWrapper.Instance.ForegroundColor = this.ForegroundColor;
        ConsoleWrapper.Instance.BackgroundColor = this.BackgroundColor;

        ConsoleWrapper.Instance.SetCursorPosition(this.Left, this.Top);
        ConsoleWrapper.Instance.Write(FormatDisplayText(DisplayText));

        ConsoleWrapper.Instance.SetCursorPosition(0, 0);
        ConsoleWrapper.Instance.RestoreColor();
      }
      IsDisplayed = true;
    }

    public override void Refresh()
    {
      Show();
    }

    bool _disposed;
    protected override void Dispose(bool disposing)
    {
      if (_disposed)
        return;

      _displayText = null;

      _disposed = true;
    }
  }
}

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
    public override int Width { get => ConsoleWrapper.WindowWidth; set { } }
    public override int Height { get => 1; set { } }

    private void RenderElement()
    {
      ConsoleWrapper.SetCursorPosition(this.Left, this.Top);
      ConsoleWrapper.WriteLine(FormatDisplayText(DisplayText));
    }

    protected string FormatDisplayText(string displayText)
    {
      string result = displayText;
      switch (this.TextAlignment)
      {
        case TextAlignment.Left:
          result = result.PadRight(this.Width, this.PadChar);
          break;

        case TextAlignment.Right:
          result = result.PadLeft(this.Width, this.PadChar);
          break;

        case TextAlignment.Center:
          result = result.PadBoth(this.Width, this.PadChar);
          break;
      }
      return result;
    }

    protected override void RenderControl()
    {
      lock (Console.Lock)
      {
        ConsoleWrapper.CursorVisible = false;
        var f = ConsoleWrapper.ForegroundColor;
        var b = ConsoleWrapper.BackgroundColor;
        ConsoleWrapper.ForegroundColor = this.ForegroundColor;
        ConsoleWrapper.BackgroundColor = this.BackgroundColor;
        this.RenderElement();
        ConsoleWrapper.SetCursorPosition(0, 0);
        ConsoleWrapper.ForegroundColor = f;
        ConsoleWrapper.BackgroundColor = b;
      }
      IsDisplayed = true;
    }

    public override void Refresh()
    {
      Show();
    }
  }
}

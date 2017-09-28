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
  public class Line : GuiElement
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
    public override int Width { get => Console.WindowWidth; set { } }
    public override int Height { get => 1; set { } }

    private void RenderElement()
    {
      Console.SetCursorPosition(this.Left, this.Top);
      Console.WriteLine(FormatDisplayText(DisplayText));
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

    public override void Show()
    {
      lock (Console.Lock)
      {
        Console.CursorVisible = false;
        var f = Console.ForegroundColor;
        var b = Console.BackgroundColor;
        Console.ForegroundColor = this.ForegroundColor;
        Console.BackgroundColor = this.BackgroundColor;
        this.RenderElement();
        Console.SetCursorPosition(0, 0);
        Console.ForegroundColor = f;
        Console.BackgroundColor = b;
      }
      IsDisplayed = true;
    }

    public override void Refresh()
    {
      Show();
    }
  }
}

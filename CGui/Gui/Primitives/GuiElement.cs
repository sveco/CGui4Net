using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGui.Gui.Primitives
{
  public enum TextAlignment
  {
    Left,
    Center,
    Right
  }
  /// <summary>
  /// Abstract class representing base UI component
  /// </summary>
  public abstract class GuiElement
  {
    public static readonly ConsoleWrapper Console = ConsoleWrapper.Instance;

    public bool IsDisplayed { get; set; }

    public ConsoleColor ForegroundColor = ConsoleWrapper.ForegroundColor;
    public ConsoleColor BackgroundColor = ConsoleWrapper.BackgroundColor;

    public ConsoleColor SelectedForegroundColor = ConsoleColor.DarkBlue;
    public ConsoleColor SelectedBackgroundColor = ConsoleColor.Green;

    public bool AutoRefresh = true;

    abstract public int Top { get; set; }
    abstract public int Left { get; set; }
    abstract public int Width { get; set; }
    abstract public int Height { get; set; }

    //public abstract void Show();
    public virtual void Show() {
      RenderBorder();
      RenderControl();
    }
    private void RenderBorder()
    {
      if (b.Style == BorderStyle.None)
        return;
      Rectangle r = new Rectangle()
      {
        BorderStyle = b.Style,
        BorderWeight = b.Weight,
        Top = this.Top,
        Left = this.Left,
        Width = this.Width,
        Height = this.Height
      };
      r.Show();
    }

    protected abstract void RenderControl();

    public abstract void Refresh();

    public TextAlignment TextAlignment = TextAlignment.Left;
    private Border b = new Border() { Style = BorderStyle.None, Weight = BorderWeight.Light };
    protected int BorderWidth { get { return (b.Style == BorderStyle.None ? 0 : 1); } }
    public BorderStyle BorderStyle { get => b.Style; set => b.Style = value; }
    public BorderWeight BorderWeight { get => b.Weight; set => b.Weight = value; }

    public char PadChar = ' ';
    protected void ClearCurrentLine()
    {
      lock (Console.Lock)
      {
        int currentLineCursor = ConsoleWrapper.CursorTop;
        ConsoleWrapper.SetCursorPosition(0, Top);
        ConsoleWrapper.Write(new string(' ', ConsoleWrapper.WindowWidth));
        ConsoleWrapper.SetCursorPosition(0, currentLineCursor);
      }
    }
  }
}

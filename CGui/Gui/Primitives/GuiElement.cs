using System;

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
  public abstract class GuiElement : IDisposable
  {
    public bool IsDisplayed { get; set; }

    public ConsoleColor ForegroundColor = ConsoleColor.White;
    public ConsoleColor BackgroundColor = ConsoleColor.Black;

    public ConsoleColor SelectedForegroundColor = ConsoleColor.DarkBlue;
    public ConsoleColor SelectedBackgroundColor = ConsoleColor.Green;

    public char PadChar = ' ';

    public bool AutoRefresh = true;

    public virtual int Top { get; set; }
    public virtual int Left { get; set; }
    public virtual int Width { get; set; }
    public virtual int Height { get; set; }

    //public abstract void Show();
    public virtual void Show() {
        RenderBorder();
        //BeginRenderControl();
        RenderControl();
        //EndRenderControl();
    }
    public virtual void RenderBorder()
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
        Height = this.Height,
        ForegroundColor = b.ForegroundColor,
        BackgroundColor = b.BackgroundColor
      };
      r.Show();
    }

    protected abstract void RenderControl();

    public virtual void Refresh()
    {
      Show();
    }

    public TextAlignment TextAlignment = TextAlignment.Left;
    private Border b = new Border() { Style = BorderStyle.None, Weight = BorderWeight.Light };
    public int BorderWidth { get { return (b.Style == BorderStyle.None ? 0 : 1); } }
    public BorderStyle BorderStyle { get => b.Style; set => b.Style = value; }
    public BorderWeight BorderWeight { get => b.Weight; set => b.Weight = value; }
    public ConsoleColor BorderForegroundColor
    {
      get => b.ForegroundColor;
      set => b.ForegroundColor = value;
    }
    public ConsoleColor BorderBackgroundColor
    {
      get => b.BackgroundColor;
      set => b.BackgroundColor = value;
    }
    protected void ClearCurrentLine()
    {
        int currentLineCursor = ConsoleWrapper.Instance.CursorTop;
        ConsoleWrapper.Instance.SetCursorPosition(0, Top);
        ConsoleWrapper.Instance.Write(new string(' ', ConsoleWrapper.Instance.WindowWidth));
        ConsoleWrapper.Instance.SetCursorPosition(0, currentLineCursor);
    }
    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }
    ~GuiElement()
    {
      Dispose(false);
    }
    protected abstract void Dispose(bool disposing);
  }
}

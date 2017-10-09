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

    public bool AutoRefresh = true;

    abstract public int Top { get; set; }
    abstract public int Left { get; set; }
    abstract public int Width { get; set; }
    abstract public int Height { get; set; }

    //public abstract void Show();
    public virtual void Show() {
        RenderBorder();
        //BeginRenderControl();
        RenderControl();
        //EndRenderControl();
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
        int currentLineCursor = ConsoleWrapper.Instance.CursorTop;
        ConsoleWrapper.Instance.SetCursorPosition(0, Top);
        ConsoleWrapper.Instance.Write(new string(' ', ConsoleWrapper.Instance.WindowWidth));
        ConsoleWrapper.Instance.SetCursorPosition(0, currentLineCursor);
    }
    bool _disposed;

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
      /*
    {
      if (_disposed)
        return;

      if (disposing)
      {
        // free other managed objects that implement
        // IDisposable only
      }

      // release any unmanaged objects
      // set the object references to null

      _disposed = true;
    }
    */
  }
}

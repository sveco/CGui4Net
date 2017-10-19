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
    private static int AbsWidth(int width)
    {
      if (width < 0) { return Console.WindowWidth + width; }
      return width;
    }

    private static int AbsHeight(int height)
    {
      if (height < 0) { return Console.WindowHeight + height; }
      return height;
    }

    public string Name { get; set; }
    public bool IsDisplayed { get; set; }

    private ConsoleColor foregroundColor = ConsoleWrapper.Instance.ForegroundColor;
    private ConsoleColor backgroundColor = ConsoleWrapper.Instance.BackgroundColor;
    public ConsoleColor ForegroundColor { get => foregroundColor; set => foregroundColor = value; }
    public ConsoleColor BackgroundColor { get => backgroundColor; set => backgroundColor = value; }

    public ConsoleColor selectedForegroundColor = ConsoleColor.DarkBlue;
    public ConsoleColor selectedBackgroundColor = ConsoleColor.Green;
    public ConsoleColor SelectedForegroundColor { get => selectedForegroundColor; set => selectedForegroundColor = value; }
    public ConsoleColor SelectedBackgroundColor { get => selectedBackgroundColor; set => selectedBackgroundColor = value; }


    private char _padChar = ' ';
    public char PadChar { get => _padChar; set => _padChar = value; }

    private bool autoRefresh = true;
    public bool AutoRefresh { get => autoRefresh; set => autoRefresh = value; }

    private int _top = 0;
    public virtual int Top {
      get => _top;
      set {
        _top = value;
        if (_top < 0) { _top = 0; }
      }}

    private int _left = 0;
    public virtual int Left {
      get => _left;
      set {
        _left = value;
        if (_left < 0) { _left = 0; }
      }
    }
    private int _width = 10;
    public virtual int Width {
      get => _width;
      set {
        _width = AbsWidth(value);
      }
    }
    private int _height = 10;
    public virtual int Height {
      get => _height;
      set {
        _height = AbsHeight(value);
      }
    }

    //public abstract void Show();
    public virtual void Show() {
        RenderBorder();
        //BeginRenderControl();
        RenderControl();
        //EndRenderControl();
    }
    public virtual void Clear() {
      for(int row = 0; row < this.Height; row ++)
      {
        ConsoleWrapper.Instance.Write(string.Empty.PadLeft(this.Width));
      }
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

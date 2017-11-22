namespace CGui.Gui
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using CGui.Gui.Primitives;

  /// <summary>
  /// Defines the <see cref="TextArea" />
  /// </summary>
  public class TextArea : Scrollable, IDisposable
  {

    private string _content;
    private bool _disposed;

    /// <summary>
    /// Lines of text
    /// </summary>
    private IList<string> _lines = new List<string>();

    private int _width = 0;

    /// <summary>
    /// Gets or sets the Content of <see cref="TextArea"/>
    /// </summary>
    public string Content
    {
      get { return _content; }
      set
      {
        _content = value;
        if (_width != 0)
        {
          ParseText();
        }
      }
    }

    /// <summary>
    /// Total number of displayed lines of text.
    /// </summary>
    public override int TotalItems
    {
      get { return _lines.Count(); }
    }

    /// <summary>
    /// Flag to indicate that TextArea should capture keyboard events.
    /// </summary>
    public bool WaitForInput { get; set; }

    /// <summary>
    /// Gets or sets the Width
    /// </summary>
    public override int Width
    {
      get { return AbsWidth(_width); }
      set
      {
        _width = AbsWidth(value);
        if (value > 0)
        {
          ParseText();
        }
      }
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="TextArea"/> class.
    /// </summary>
    public TextArea()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TextArea"/> class.
    /// </summary>
    /// <param name="content">The <see cref="string"/></param>
    public TextArea(string content)
    {
      this.Content = content;
      Offset = 0;
    }

    /// <summary>
    /// The OnItemKey
    /// </summary>
    /// <param name="key">The <see cref="ConsoleKeyInfo"/></param>
    /// <returns>The <see cref="bool"/></returns>
    public delegate bool OnItemKey(ConsoleKeyInfo key);

    /// <summary>
    /// Defines the OnItemKeyHandler
    /// </summary>
    public event OnItemKey OnItemKeyHandler;
    /// <summary>
    /// Refreshes the <see cref="GuiElement"/>
    /// </summary>
    public override void Refresh()
    {
      base.RenderBorder();
      RenderControl();
    }

    /// <summary>
    /// Scrolls down the text.
    /// </summary>
    /// <param name="Step">The <see cref="int"/></param>
    public override void ScrollDown(int Step)
    {
      if (_lines.Count == 0 || _lines.Count < Height) { return; }
      if (_lines.Count - Step > Height + Offset)
      {
        Offset = Offset + Step;
      }
      else
      {
        if (Offset < _lines.Count - Height)
          Offset = Math.Max(0, _lines.Count - Height);
      }
      RenderControl();
    }

    /// <summary>
    /// Scrolls up the text.
    /// </summary>
    /// <param name="Step">The <see cref="int"/></param>
    public override void ScrollUp(int Step)
    {
      if (_lines.Count == 0 || _lines.Count < Height) { return; }
      if (Offset > Step) { Offset = Offset - Step; } else { Offset = 0; }
      RenderControl();
    }

    /// <summary>
    /// The Show
    /// </summary>
    public override void Show()
    {
      base.Show();
      if (this.WaitForInput)
      {
        InputLoop();
      }
    }

    /// <summary>
    /// The Dispose
    /// </summary>
    /// <param name="disposing">The <see cref="bool"/></param>
    protected override void Dispose(bool disposing)
    {
      if (_disposed)
        return;

      _content = null;
      _lines = null;

      _disposed = true;
    }

    /// <summary>
    /// The RenderControl
    /// </summary>
    protected override void RenderControl()
    {
      lock (ConsoleWrapper.Instance.Lock)
      {
        ConsoleWrapper.Instance.CursorVisible = false;
        ConsoleWrapper.Instance.SaveColor();

        ConsoleWrapper.Instance.ForegroundColor = this.ForegroundColor;
        ConsoleWrapper.Instance.BackgroundColor = this.BackgroundColor;

        for (int i = 0; i < Math.Min(Height - (BorderWidth * 2), _lines.Count - Offset - (BorderWidth * 2)); i++)
        {
          ConsoleWrapper.Instance.SetCursorPosition(Left + BorderWidth, Top + i + BorderWidth);
          ConsoleWrapper.Instance.Write(GetDisplayText(Offset + i, _lines[Offset + i]));
        }

        ConsoleWrapper.Instance.SetCursorPosition(0, 0);
        ConsoleWrapper.Instance.RestoreColor();
      }
    }

    /// <summary>
    /// Handles keyboards events.
    /// </summary>
    private void InputLoop()
    {
      bool cont = true;
      do
      {
        var key = ConsoleWrapper.Instance.ReadKey(true);

        switch (key.Key)
        {
          case ConsoleKey.UpArrow:
            ScrollUp();
            break;

          case ConsoleKey.DownArrow:
            ScrollDown();
            break;

          case ConsoleKey.PageUp:
            ScrollUp(DefaultPage);
            break;

          case ConsoleKey.PageDown:
            ScrollDown(DefaultPage);
            break;

          case ConsoleKey.Escape:
            cont = false;
            break;

          default:
            if (OnItemKeyHandler != null)
            {
              cont = OnItemKeyHandler(key);
            }
            break;
        }

      } while (cont);
    }

    /// <summary>
    /// Parses text to lines.
    /// </summary>
    /// <returns>The <see cref="IList{string}"/></returns>
    private IList<string> ParseText()
    {
      _lines = new List<string>();
      if (!string.IsNullOrWhiteSpace(Content))
      {
        _lines = Content.Split(this.Width - 5).ToList();
      }

      return _lines;
    }
  }
}

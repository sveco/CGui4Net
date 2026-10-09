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

    /// <summary>
    /// Width as it was set. Negative is relative to the console, like for any other element.
    /// </summary>
    private int _width = 0;

    /// <summary>
    /// Width the lines were wrapped for. The text is wrapped again when the width has changed, for
    /// example when the console was resized.
    /// </summary>
    private int _parsedWidth = -1;

    private readonly object _linesLock = new object();

    /// <summary>
    /// Gets or sets the Content of <see cref="TextArea"/>
    /// </summary>
    public string Content
    {
      get { return _content; }
      set
      {
        _content = value;
        _parsedWidth = -1;
      }
    }

    /// <summary>
    /// Lines of text, wrapped for the current width.
    /// </summary>
    private IList<string> Lines
    {
      get
      {
        lock (_linesLock)
        {
          if (_width != 0 && _parsedWidth != Width)
          {
            ParseText();
          }
          return _lines;
        }
      }
    }

    /// <summary>
    /// Total number of displayed lines of text.
    /// </summary>
    public override int TotalItems
    {
      get { return Lines.Count(); }
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
        _width = value;
        _parsedWidth = -1;
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
      var lines = Lines;
      if (lines.Count == 0 || lines.Count < Height) { return; }
      if (lines.Count - Step > Height + Offset)
      {
        Offset = Offset + Step;
      }
      else
      {
        if (Offset < lines.Count - Height)
          Offset = Math.Max(0, lines.Count - Height);
      }
      RenderControl();
    }

    /// <summary>
    /// Scrolls up the text.
    /// </summary>
    /// <param name="Step">The <see cref="int"/></param>
    public override void ScrollUp(int Step)
    {
      var lines = Lines;
      if (lines.Count == 0 || lines.Count < Height) { return; }
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

        var lines = Lines;

        // the text can be shorter, or the area higher, than when it was scrolled
        Offset = ListLayout.ClampOffset(Offset, lines.Count, Height - (BorderWidth * 2));

        for (int i = 0; i < Math.Min(Height - (BorderWidth * 2), lines.Count - Offset - (BorderWidth * 2)); i++)
        {
          ConsoleWrapper.Instance.SetCursorPosition(Left + BorderWidth, Top + i + BorderWidth);
          ConsoleWrapper.Instance.Write(GetDisplayText(Offset + i, lines[Offset + i]));
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
      // a text area that is not part of the viewport on screen is a dialog on top of it, it is not redrawn
      // by the viewport when the console is resized
      using (Viewport.KeyLoopScope(this))
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
              ScrollUp(ScrollPageStep);
              break;

            case ConsoleKey.PageDown:
              ScrollDown(ScrollPageStep);
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
    }

    /// <summary>
    /// Parses text to lines.
    /// </summary>
    /// <returns>The <see cref="IList{string}"/></returns>
    private IList<string> ParseText()
    {
      _parsedWidth = Width;
      _lines = new List<string>();
      if (!string.IsNullOrWhiteSpace(Content))
      {
        // Split needs a chunk of at least 1, which a narrow text area does not have
        _lines = Content.Split(Math.Max(1, _parsedWidth - 5)).ToList();
      }

      return _lines;
    }
  }
}

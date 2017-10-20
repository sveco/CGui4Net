using CGui.Gui.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CGui.Gui
{
  public class TextArea : Scrollable, IDisposable
  {
    private static readonly object lockObject = new object();

    private int _width = 0;
    public override int Width
    {
      get { return _width; }
      set
      {
        _width = value;
        if (value > 0)
        {
          ParseText();
        }
      }
    }

    public bool WaitForInput { get; set; }

    public delegate bool OnItemKey(ConsoleKeyInfo key);
    public event OnItemKey OnItemKeyHandler;

    string _content;
    public string Content
    {
      get { return _content; }
      set
      {
        _content = value;
        if (_width > 0)
        {
          ParseText();
        }
      }
    }

    private IList<string> _lines = new List<string>();
    public override int TotalItems { get { return _lines.Count(); } }

    private IList<string> ParseText()
    {
      _lines = new List<string>();
      if (!string.IsNullOrWhiteSpace(Content))
      {
        _lines = Content.Split(this.Width - 5).ToList();
      }

      return _lines;
    }

    public TextArea() { }
    public TextArea(string content)
    {
      this.Content = content;
      Offset = 0;
    }
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

    public override void Show()
    {
      base.Show();
      if (this.WaitForInput)
      {
        InputLoop();
      }
    }

    public override void Refresh()
    {
      base.RenderBorder();
      RenderControl();
    }

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

    bool _disposed;
    protected override void Dispose(bool disposing)
    {
      if (_disposed)
        return;

      _content = null;
      _lines = null;

      _disposed = true;
    }

    public override void ScrollUp(int Step)
    {
      if (Offset > Step) { Offset = Offset - Step; } else { Offset = 0; }
      RenderControl();
    }

    public override void ScrollDown(int Step)
    {
      if (_lines.Count < Height) { return; }
      if (_lines.Count - 10 > Height + Offset)
      {
        Offset = Offset + 10;
      }
      else
      {
        if (Offset < _lines.Count - Height)
          Offset = Math.Max(0, _lines.Count - Height);
      }
      RenderControl();
    }
  }
}

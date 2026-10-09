using CGui.Gui.Primitives;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace CGui.Gui
{
  public class Viewport : GuiElement
  {
    /// <summary>
    /// Viewports that are showing, the last one is on top. Viewports are nested when an application opens
    /// another view from the keyboard handler of the first one.
    /// </summary>
    private static readonly List<Viewport> ActiveViewports = new List<Viewport>();

    static Viewport()
    {
      ConsoleWrapper.Instance.Resized += (sender, e) => TopViewport()?.HandleResize();
    }

    /// <summary>
    /// Creates a viewport. It does not change the size of the console window, set <see cref="Width"/> and
    /// <see cref="Height"/> to do that.
    /// </summary>
    public Viewport()
    {
    }

    /// <summary>
    /// When true, which is the default, the viewport that is on screen draws all its controls again when the
    /// console is resized. Relative sizes of the controls (negative Width or Height) are calculated again
    /// for the new size.
    /// </summary>
    public bool RefreshOnResize { get; set; } = true;

    /// <summary>
    /// Raised before the viewport is drawn again after the console was resized. Use it to adjust what is not
    /// a control of the viewport.
    /// </summary>
    public event EventHandler Resizing;

    /// <summary>
    /// Raised after the viewport was drawn again after the console was resized.
    /// </summary>
    public event EventHandler Resized;

    private int _laidOutWidth;
    private int _laidOutHeight;

    public override int Top { get => 0; set { } }
    public override int Left { get => 0; set { } }

    /// <summary>
    /// Width of the viewport is the width of the console window. Setting it resizes the console window and
    /// its buffer.
    /// </summary>
    public override int Width
    {
      get
      {
        return ConsoleWrapper.Instance.WindowWidth;
      }
      set
      {
        ConsoleWrapper.Instance.SetWindowSize(Math.Min(value, ConsoleWrapper.Instance.LargestWindowWidth), this.Height);
        ConsoleWrapper.Instance.BufferWidth = Math.Min(value, ConsoleWrapper.Instance.LargestWindowWidth);
      }
    }

    /// <summary>
    /// Height of the viewport is the height of the console window. Setting it resizes the console window and
    /// its buffer.
    /// </summary>
    public override int Height
    {
      get
      {
        return ConsoleWrapper.Instance.WindowHeight;
      }
      set
      {
        ConsoleWrapper.Instance.SetWindowSize(this.Width, Math.Min(value, ConsoleWrapper.Instance.LargestWindowHeight));
        ConsoleWrapper.Instance.BufferHeight = Math.Min(value, ConsoleWrapper.Instance.LargestWindowHeight);
      }
    }

    public Collection<GuiElement> Controls = new Collection<GuiElement>();

    protected override void RenderControl()
    {
      lock (ActiveViewports)
      {
        ActiveViewports.Add(this);
      }

      try
      {
        RememberSize();
        ConsoleWrapper.Clear();

        Parallel.ForEach(Controls, (e) => {
          if (e != null)
          {
            e.IsDisplayed = true;
            e.Show();
          }
        });
      }
      finally
      {
        Viewport parent;
        lock (ActiveViewports)
        {
          ActiveViewports.Remove(this);
          parent = ActiveViewports.Count > 0 ? ActiveViewports[ActiveViewports.Count - 1] : null;
        }

        // the console may have been resized while the parent was covered by this viewport
        parent?.CatchUp();
      }
    }

    public override void Refresh()
    {
      RememberSize();
      ConsoleWrapper.Clear();
      Parallel.ForEach(Controls, (e) => {
        if (e != null)
        {
          e.IsDisplayed = true;
          e.Refresh();
        }
      });
    }

    /// <summary>
    /// The viewport that is on top of the others on screen, or null.
    /// </summary>
    private static Viewport TopViewport()
    {
      lock (ActiveViewports)
      {
        return ActiveViewports.Count > 0 ? ActiveViewports[ActiveViewports.Count - 1] : null;
      }
    }

    /// <summary>
    /// Marks a keyboard loop of a control. When the control belongs to the viewport that is on top, the
    /// viewport is drawn again when the console is resized. Any other control, like a dialog on top of the
    /// viewport, would be wiped by that, so resizes are held back until its loop ends.
    /// </summary>
    internal static IDisposable KeyLoopScope(GuiElement control)
    {
      var top = TopViewport();
      if (top != null && top.Controls != null && top.Controls.Contains(control))
      {
        return NoScope.Instance;
      }
      return ConsoleWrapper.Instance.SuspendResize();
    }

    private sealed class NoScope : IDisposable
    {
      public static readonly NoScope Instance = new NoScope();

      public void Dispose()
      {
      }
    }

    /// <summary>
    /// Called on the thread that waits for a key when the console was resized.
    /// </summary>
    private void HandleResize()
    {
      if (!RefreshOnResize)
      {
        return;
      }

      Resizing?.Invoke(this, EventArgs.Empty);
      Refresh();
      Resized?.Invoke(this, EventArgs.Empty);
    }

    private void CatchUp()
    {
      int width;
      int height;
      try
      {
        width = ConsoleWrapper.Instance.WindowWidth;
        height = ConsoleWrapper.Instance.WindowHeight;
      }
      catch (System.IO.IOException)
      {
        return;
      }

      if (width != _laidOutWidth || height != _laidOutHeight)
      {
        HandleResize();
      }
    }

    private void RememberSize()
    {
      try
      {
        _laidOutWidth = ConsoleWrapper.Instance.WindowWidth;
        _laidOutHeight = ConsoleWrapper.Instance.WindowHeight;
      }
      catch (System.IO.IOException)
      {
        // no console to measure
      }
    }

    private bool _disposed;

    // a finalizer is not necessary, as it is inherited from
    // the base class

    protected override void Dispose(bool disposing)
    {
      if (_disposed)
        return;

      if (disposing)
      {
        // free other managed objects that implement
        // IDisposable only
        foreach (var control in Controls)
        {
          if (control != null)
          {
            if (control is IDisposable)
            {
              control.Dispose();
            }
          }
        }
        Controls = null;
      }

      // release any unmanaged objects
      // set object references to null
      _disposed = true;
    }
  }
}

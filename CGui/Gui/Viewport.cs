using CGui.Gui.Primitives;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace CGui.Gui
{
  public class Viewport : GuiElement
  {
    /// <summary>
    /// All viewports, to find the one a control belongs to. They are not kept alive by this list.
    /// </summary>
    private static readonly List<WeakReference<Viewport>> AllViewports = new List<WeakReference<Viewport>>();

    /// <summary>
    /// The control that waits for a key on the current thread, when it belongs to a viewport.
    /// </summary>
    [ThreadStatic]
    private static GuiElement _loopOwner;

    static Viewport()
    {
      ConsoleWrapper.Instance.Resized += (sender, e) =>
      {
        // raised on the thread that waits for a key, see ConsoleWrapper.ReadKey
        var owner = _loopOwner;
        if (owner != null)
        {
          ForControl(owner)?.HandleResize();
        }
      };
    }

    /// <summary>
    /// Creates a viewport. It does not change the size of the console window, set <see cref="Width"/> and
    /// <see cref="Height"/> to do that.
    /// </summary>
    public Viewport()
    {
      lock (AllViewports)
      {
        AllViewports.RemoveAll(r => !r.TryGetTarget(out Viewport v));
        AllViewports.Add(new WeakReference<Viewport>(this));
      }
    }

    /// <summary>
    /// When true, which is the default, the viewport draws all its controls again when the console is
    /// resized, while one of its controls waits for a key. Relative sizes of the controls (negative Width
    /// or Height) are calculated again for the new size.
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
    /// The viewport a control belongs to, or null. The most recently created viewport wins when a control
    /// was added to more than one.
    /// </summary>
    private static Viewport ForControl(GuiElement control)
    {
      lock (AllViewports)
      {
        for (int i = AllViewports.Count - 1; i >= 0; i--)
        {
          if (AllViewports[i].TryGetTarget(out Viewport viewport)
              && viewport.Controls != null
              && viewport.Controls.Contains(control))
          {
            return viewport;
          }
        }
      }
      return null;
    }

    /// <summary>
    /// Marks the keyboard loop of a control. The viewport the control belongs to is drawn again when the
    /// console is resized while the loop waits for a key. Any other control, like a dialog on top of a
    /// viewport, would be wiped by that, so resizes are held back until its loop ends.
    /// </summary>
    internal static IDisposable KeyLoopScope(GuiElement control)
    {
      var previous = _loopOwner;
      if (ForControl(control) != null)
      {
        _loopOwner = control;
        return new LoopScope(previous, null);
      }

      _loopOwner = null;
      return new LoopScope(previous, ConsoleWrapper.Instance.SuspendResize());
    }

    private sealed class LoopScope : IDisposable
    {
      private readonly GuiElement _previous;
      private IDisposable _suspension;

      public LoopScope(GuiElement previous, IDisposable suspension)
      {
        _previous = previous;
        _suspension = suspension;
      }

      public void Dispose()
      {
        _loopOwner = _previous;
        var suspension = Interlocked.Exchange(ref _suspension, null);
        suspension?.Dispose();
      }
    }

    /// <summary>
    /// Called by the keyboard loop of a control before it waits for the next key. The console may have been
    /// resized while the viewport was covered by another one, or while the loop was busy with a key.
    /// </summary>
    internal static void BeforeKeyWait(GuiElement control)
    {
      ForControl(control)?.CatchUp();
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

      // nothing was drawn yet when the size is not known
      if (_laidOutWidth != 0 && (width != _laidOutWidth || height != _laidOutHeight))
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

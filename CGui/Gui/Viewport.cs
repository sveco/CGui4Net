using CGui.Gui.Primitives;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace CGui.Gui
{
  public class Viewport : GuiElement
  {
    public Viewport()
    {
      Width = ConsoleWrapper.Instance.WindowWidth;
      Height = ConsoleWrapper.Instance.WindowHeight;
    }

    public override int Top { get => 0; set { } }
    public override int Left { get => 0; set { } }
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
      ConsoleWrapper.Clear();
      Parallel.ForEach(Controls, (e) => {
        if (e != null)
        {
          e.IsDisplayed = true;
          e.Refresh();
        }
      });
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

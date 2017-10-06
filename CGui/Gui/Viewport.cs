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
      this.Width = ConsoleWrapper.WindowWidth;
      this.Height = ConsoleWrapper.WindowHeight;
    }

    public override int Top { get => 0; set { } }
    public override int Left { get => 0; set { } }
    public override int Width
    {
      get
      {
        return ConsoleWrapper.WindowWidth;
      }
      set
      {
        ConsoleWrapper.SetWindowSize(Math.Min(value, ConsoleWrapper.LargestWindowWidth), this.Height);
      }
    }
    public override int Height
    {
      get
      {
        return ConsoleWrapper.WindowHeight;
      }
      set
      {
        ConsoleWrapper.SetWindowSize(this.Width, Math.Min(value, ConsoleWrapper.LargestWindowHeight));
      }
    }

    public Collection<GuiElement> Controls = new Collection<GuiElement>();

    protected override void RenderControl()
    {
      ConsoleWrapper.Clear();
      foreach (var e in Controls)
      {
        if (e != null) {
          e.IsDisplayed = true;
          e.Show();
        }
      }
    }

    public override void Refresh()
    {
      ConsoleWrapper.Clear();
      foreach (var e in Controls)
      {
        if (e != null)
        {
          e.IsDisplayed = true;
          e.Refresh();
        }
      }
    }
  }
}

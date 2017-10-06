using System;

namespace CGui.Gui.Primitives
{
  public class Rectangle : GuiElement
  {
    public override int Top { get; set; }
    public override int Left { get; set; }
    public override int Width { get; set; }
    public override int Height { get; set; }

    public override void Refresh()
    {
      throw new NotImplementedException();
    }

    Border b = new Border();
    public new BorderStyle BorderStyle
    {
      get { return b.Style; }
      set { b.Style = value; }
    }
    protected override void RenderControl()
    {
      lock (Console.Lock)
      {
        ConsoleWrapper.SetCursorPosition(Left, Top);
        ConsoleWrapper.Write(b.Get(PositionV.Top, PositionH.Left).ToString());
        for (int i = 0; i < Width - 2; i++)
        {
          ConsoleWrapper.Write(b.Get(PositionV.Top, PositionH.Middle).ToString());
        }
        ConsoleWrapper.Write(b.Get(PositionV.Top, PositionH.Right).ToString());
        for (int i = Top + 1; i < Height + Top - 1; i++)
        {
          ConsoleWrapper.SetCursorPosition(Left, i);
          ConsoleWrapper.Write(b.Get(PositionV.Middle, PositionH.Left).ToString());
          ConsoleWrapper.SetCursorPosition(Left + Width - 1, i);
          ConsoleWrapper.Write(b.Get(PositionV.Middle, PositionH.Right).ToString());
        }
        ConsoleWrapper.SetCursorPosition(Left, Top + Height - 1);
        ConsoleWrapper.Write(b.Get(PositionV.Bottom, PositionH.Left).ToString());
        for (int i = 0; i < Width - 2; i++)
        {
          ConsoleWrapper.Write(b.Get(PositionV.Bottom, PositionH.Middle).ToString());
        }
        ConsoleWrapper.Write(b.Get(PositionV.Bottom, PositionH.Right).ToString());
      }
    }
  }
}

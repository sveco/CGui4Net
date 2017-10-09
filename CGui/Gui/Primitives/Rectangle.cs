﻿using System;

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
      //lock (ConsoleWrapper.Instance.Lock)
      //{
        ConsoleWrapper.Instance.SetCursorPosition(Left, Top);
        ConsoleWrapper.Instance.Write(b.Get(PositionV.Top, PositionH.Left).ToString());
        for (int i = 0; i < Width - 2; i++)
        {
          ConsoleWrapper.Instance.Write(b.Get(PositionV.Top, PositionH.Middle).ToString());
        }
        ConsoleWrapper.Instance.Write(b.Get(PositionV.Top, PositionH.Right).ToString());
        for (int i = Top + 1; i < Height + Top - 1; i++)
        {
          ConsoleWrapper.Instance.SetCursorPosition(Left, i);
          ConsoleWrapper.Instance.Write(b.Get(PositionV.Middle, PositionH.Left).ToString());
          ConsoleWrapper.Instance.SetCursorPosition(Left + Width - 1, i);
          ConsoleWrapper.Instance.Write(b.Get(PositionV.Middle, PositionH.Right).ToString());
        }
        ConsoleWrapper.Instance.SetCursorPosition(Left, Top + Height - 1);
        ConsoleWrapper.Instance.Write(b.Get(PositionV.Bottom, PositionH.Left).ToString());
        for (int i = 0; i < Width - 2; i++)
        {
          ConsoleWrapper.Instance.Write(b.Get(PositionV.Bottom, PositionH.Middle).ToString());
        }
        ConsoleWrapper.Instance.Write(b.Get(PositionV.Bottom, PositionH.Right).ToString());
      //}
    }

    protected override void Dispose(bool disposing)
    {
      //Nothing to dispose of.
    }
  }
}

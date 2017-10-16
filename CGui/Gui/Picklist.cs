using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CGui.Gui.Primitives;

namespace CGui.Gui
{
  public class Picklist<T> : Scrollable, IDisposable where T : IListItem
  {
    private IList<T> listItems = new List<T>();
    public int SelectedItemIndex = 0;
    public int SelectionPosition = 0;
    private Action<T, Picklist<T>> ProcessItem;
    public override int TotalItems { get { return ListItems.Count(); } }

    private int ListHeight { get { return this.Height - (BorderWidth * 2); } }
    public IList<T> ListItems
    {
      get => listItems;
      internal set
      {
        listItems.Clear();
        if (value != null)
        {
          foreach (var i in value.OrderBy(x => x.Index).ToList())
          {
            i.PropertyChanged += ListItem_PropertyChanged;
            ListItems.Add(i);
          }
        }
      }
    }

    public delegate bool OnItemKey(ConsoleKeyInfo key, T selectedItem, Picklist<T> parent);
    public event OnItemKey OnItemKeyHandler;
    private void ListItem_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "DisplayText")
      {
        if (IsDisplayed)
        {
          UpdateItem(((ListItem)sender).Index + Offset);
        }
      }
    }

    public Picklist(IList<T> items)
    {
      ListItems.Clear();
      if (items != null)
      {
        foreach (var i in items.OrderBy(x => x.Index).ToList())
        {
          i.PropertyChanged += ListItem_PropertyChanged;
          ListItems.Add(i);
        }
      }
    }

    public Picklist(IList<T> items, Action<T, Picklist<T>> processItem)
    {
      ListItems.Clear();
      foreach (var i in items.OrderBy(x => x.Index).ToList())
      {
        i.PropertyChanged += ListItem_PropertyChanged;
        ListItems.Add(i);
      }
      this.ProcessItem = processItem;
    }
    protected override void RenderControl()
    {
      if (TotalItems == 0) { return; }
      this.IsDisplayed = true;
      for (int i = 0; i < Math.Min(ListHeight, TotalItems - Offset); i++)
      {
        RenderItem(i);
      }
    }

    public override void Show()
    {
      base.Show();
      Select();
    }

    public override void Refresh()
    {
      base.RenderBorder();
      this.RenderControl();
    }

    private void Select()
    {
      if (ProcessItem != null)
      {
        new Thread(() =>
        {
          Thread.CurrentThread.IsBackground = true;
          Parallel.ForEach<T>(this.ListItems, (item) =>
          {
            ProcessItem.Invoke(item, this);
          });
        }).Start();
      }

      bool cont = true;
      do
      {
        var key = ConsoleWrapper.Instance.ReadKey(true);
        var prevSelectionPosition = SelectionPosition;
        switch (key.Key)
        {
          case ConsoleKey.UpArrow:
            if (SelectedItemIndex > 0) { SelectedItemIndex--; }
            if (SelectionPosition - 1 < 0)
            {
              if (Offset > 0)
              {
                Offset--;
                RenderControl();
              }
            }
            else
            {
              SelectionPosition--;
              RenderItem(prevSelectionPosition);
              RenderItem(SelectionPosition);
            }

            break;
          case ConsoleKey.DownArrow:
            if (SelectedItemIndex + 1 < TotalItems) { SelectedItemIndex++; } else { break; }
            if (SelectionPosition + 1 >= ListHeight)
            {
              if (Offset + ListHeight < TotalItems) { Offset++; }
              RenderControl();
            }
            else
            {
              SelectionPosition++;
              RenderItem(prevSelectionPosition);
              RenderItem(SelectionPosition);
            }
            break;

          case ConsoleKey.PageUp:
            if (SelectedItemIndex > 10) { SelectedItemIndex -= 10; } else { SelectedItemIndex = 0; }
            if (SelectionPosition > 10)
            {

              SelectionPosition -= 10;
              RenderItem(prevSelectionPosition);
              RenderItem(SelectionPosition);

            }
            else
            {
              if (Offset == 0)
              {
                SelectionPosition = 0;
                RenderItem(prevSelectionPosition);
                RenderItem(SelectionPosition);
              }
              else
              {
                Offset -= 10;
                if (Offset < 0)
                {
                  SelectionPosition += Offset;
                  Offset = 0;
                }
                RenderControl();
              }
            }
            break;

          case ConsoleKey.PageDown:
            if (SelectedItemIndex + 10 < TotalItems) { SelectedItemIndex += 10; } else { SelectedItemIndex = TotalItems - 1; }
            if (SelectionPosition + 10 >= Math.Min(ListHeight, TotalItems - 1))
            {
              var prevSel = SelectionPosition;
              SelectionPosition = Math.Min(ListHeight - 1, TotalItems - 1);
              Offset += 10 - (SelectionPosition - prevSel);

              if (Offset > TotalItems - ListHeight)
              {
                Offset = TotalItems - ListHeight;
                if (Offset < 0) { Offset = 0; }
              }
              RenderControl();
            }
            else
            {
              SelectionPosition = Math.Min(SelectionPosition + 10, TotalItems - 1);
              RenderItem(prevSelectionPosition);
              RenderItem(SelectionPosition);
            }
            break;

          default:
            if (SelectedItemIndex < this.TotalItems && OnItemKeyHandler != null)
            {
              cont = OnItemKeyHandler(key, this.ListItems[SelectedItemIndex], this);
            }
            else
            {
              cont = false;
            }
            break;

        }

      } while (cont);
    }

    protected void RenderItem(int index)
    {
      lock (ConsoleWrapper.Instance.Lock)
      {
        ConsoleWrapper.Instance.CursorVisible = false;
        ConsoleWrapper.Instance.SaveColor();

        if (index >= ListItems.Count()) return;
        ConsoleWrapper.Instance.SetCursorPosition(this.Left + BorderWidth, this.Top + index + BorderWidth);
        if (SelectionPosition == index)
        {
          ConsoleWrapper.Instance.ForegroundColor = this.SelectedForegroundColor;
          ConsoleWrapper.Instance.BackgroundColor = this.SelectedBackgroundColor;
        }
        ConsoleWrapper.Instance.Write(GetDisplayText(index + Offset, ListItems[index + Offset].DisplayText));
        if (SelectionPosition == index)
        {
          ConsoleWrapper.Instance.ForegroundColor = this.ForegroundColor;
          ConsoleWrapper.Instance.BackgroundColor = this.BackgroundColor;
        }

        ConsoleWrapper.Instance.RestoreColor();
        ConsoleWrapper.Instance.SetCursorPosition(0, 0);
      }
    }

    public void UpdateItem(int Index)
    {
      if (Index >= Offset && Index < Offset + Height)
      {
        RenderItem(Index);
      }
    }
    /*
    protected string GetDisplayText(int index)
    {
      string result = ListItems[index].DisplayText;

      switch (this.TextAlignment)
      {
        case TextAlignment.Left:
          result = result.PadRight(this.Width-1 - (BorderWidth * 2), this.PadChar);
          break;

        case TextAlignment.Right:
          result = result.PadLeft(this.Width-1 - (BorderWidth *2), this.PadChar);
          break;

        case TextAlignment.Center:
          result = result.PadBoth(this.Width-1 - (BorderWidth * 2), this.PadChar);
          break;
      }

      if (result.Length > this.Width - 1)
      {
        if (result.Length > 4)
        {
          result = result.Substring(0, this.Width - 4 - (BorderWidth * 2)) + "...";
        }
        else
        {
          result = result.Substring(0, this.Width - (BorderWidth * 2));
        }
      }

      var visibleHeight = Height - (BorderWidth * 2);
      if (ShowScrollBar && visibleHeight > 3 && visibleHeight < TotalItems)
      {
        var ratio = (double)visibleHeight / TotalItems;
        int size = (int)Math.Ceiling(((double)visibleHeight) * ratio) - 2;
        var top = Math.Ceiling(Offset * ratio);

        bool first = (index - Offset == 0);
        bool last = index - Offset == visibleHeight - 1;

        bool show = index - Offset > top
            && index - Offset < size + top;

        if (first)
        {
          result = result + ConsoleWrapper.ColorReset + ScrollBarCharUp;
        }
        else if (last)
        {
          result = result + ConsoleWrapper.ColorReset + ScrollBarCharDown;
        }
        else if (show)
        {
          result = result + ConsoleWrapper.ColorReset + ScrollBarChar;
        }
        else
        {
          result = result + ConsoleWrapper.ColorReset + ScrollBarCharInactive;
        }
      }

      return result;
    }
    */
    public void UpdateList(IEnumerable<T> items)
    {
      ListItems.Clear();
      foreach (var i in items)
      {
        i.PropertyChanged += ListItem_PropertyChanged;
        ListItems.Add(i);
      }
    }

    bool _disposed;
    protected override void Dispose(bool disposing)
    {
      if (_disposed)
        return;

      if (disposing)
      {
        // free other managed objects that implement
        // IDisposable only
      }

      // release any unmanaged objects
      // set the object references to null
      listItems = null;

      _disposed = true;
    }
  }
}


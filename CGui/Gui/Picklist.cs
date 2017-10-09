using CGui.Gui.Primitives;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace CGui.Gui
{
  public class Picklist<T> : GuiElement, IDisposable
  {
    public int Offset = 0;
    public bool ShowScrollBar { get; set; }
    private string ScrollBarChar = "█";
    private IList<ListItem<T>> listItems = new List<ListItem<T>>();
    public int SelectedItemIndex = 0;
    public int SelectionPosition = 0;
    private Action<ListItem<T>, Picklist<T>> ProcessItem;
    public int TotalItems { get { return ListItems.Count(); } }

    public override int Top { get; set; }
    public override int Left { get; set; }
    public override int Width { get; set; }
    public override int Height { get; set; }

    private int ListHeight { get { return this.Height - (BorderWidth * 2); } }
    public IList<ListItem<T>> ListItems
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

    public delegate bool OnItemKey(ConsoleKeyInfo key, ListItem<T> selectedItem, Picklist<T> parent);
    public event OnItemKey OnItemKeyHandler;
    private void ListItem_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "DisplayText")
      {
        if (IsDisplayed) {
          UpdateItem(((ListItem<T>)sender).Index + Offset);
        }
      }
    }

    public Picklist(IList<ListItem<T>> items)
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

    public Picklist(IList<ListItem<T>> items, Action<ListItem<T>, Picklist<T>> processItem)
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
      this.RenderControl();
    }

    private void Select()
    {
      if (ProcessItem != null)
      {
        new Thread(() =>
        {
          Thread.CurrentThread.IsBackground = true;
          Parallel.ForEach<ListItem<T>>(this.ListItems, (item) =>
          {
            ProcessItem.Invoke(item, this);
          });
        }).Start();
      }

      bool cont = true;
      do
      {
        var key = ConsoleWrapper.Instance.ReadKey(true);
        var prevItem = SelectionPosition;
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
              RenderItem(prevItem);
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
              RenderItem(prevItem);
              RenderItem(SelectionPosition);
            }
            break;

          case ConsoleKey.PageUp:
            if (SelectedItemIndex > 10) { SelectedItemIndex -= 10; } else { SelectedItemIndex = 0; }
            if (SelectionPosition - 10 < 0)
            {
              SelectionPosition = 0;
              if (Offset >= 10)
              {
                Offset -= 10;

              }
              else { Offset = 0; }
              RenderControl();
            }
            else
            {
              SelectionPosition -= 10;
              RenderItem(prevItem);
              RenderItem(SelectionPosition);
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
              RenderItem(prevItem);
              RenderItem(SelectionPosition);
            }
            break;

          default:
            if (SelectedItemIndex < this.TotalItems)
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
        ConsoleWrapper.Instance.WriteLine(GetDisplayText(index + Offset));
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

      if (ShowScrollBar && Height < TotalItems)
      {
        var ratio = (double)Height / TotalItems;
        int size = (int)Math.Ceiling((double)Height * ratio);
        var top = Math.Ceiling(Offset * ratio);

        bool show = index - Offset > top
            && index - Offset < size + top;

        if (show)
        {
          result = result + ScrollBarChar;
        }
        else
        {
          result = result + this.PadChar;
        }
      }

      return result;
    }

    public void UpdateList(IEnumerable<ListItem<T>> items)
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


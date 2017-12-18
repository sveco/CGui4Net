namespace CGui.Gui
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;
  using CGui.Gui.Primitives;

  /// <summary>
  /// Defines the <see cref="Picklist{T}" />
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class Picklist<T> : Scrollable, IDisposable where T : IListItem
  {
    /// <summary>
    /// Index of currently selected item.
    /// </summary>
    public int SelectedItemIndex = 0;

    /// <summary>
    /// Position of current selection on screen.
    /// </summary>
    public int SelectionPosition = 0;

    private bool _disposed;

    /// <summary>
    /// Defines the list of items to display.
    /// </summary>
    private IList<T> listItems = new List<T>();

    /// <summary>
    /// Defines the ProcessItem
    /// </summary>
    private Action<T, Picklist<T>> ProcessItem;

    /// <summary>
    /// Gets or sets the ListItems
    /// </summary>
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

    /// <summary>
    /// Number of items in <see cref="Picklist{T}"/>
    /// </summary>
    public override int TotalItems
    {
      get { return ListItems.Count(); }
    }

    /// <summary>
    /// Gets the height of list itself (without borders)
    /// </summary>
    private int ListHeight
    {
      get { return this.Height - (BorderWidth * 2); }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Picklist{T}"/> class.
    /// </summary>
    public Picklist()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Picklist{T}"/> class.
    /// </summary>
    /// <param name="items">The <see cref="IList{T}"/></param>
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

    /// <summary>
    /// Initializes a new instance of the <see cref="Picklist{T}"/> class.
    /// </summary>
    /// <param name="items">The <see cref="IList{T}"/></param>
    /// <param name="processItem">The <see cref="Action{T, Picklist{T}}"/></param>
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

    /// <summary>
    /// Custom handler for keyboard events.
    /// </summary>
    /// <param name="key">The <see cref="ConsoleKeyInfo"/></param>
    /// <param name="selectedItem">The <see cref="T"/></param>
    /// <param name="parent">The <see cref="Picklist{T}"/></param>
    /// <returns>The <see cref="bool"/></returns>
    public delegate bool OnItemKey(ConsoleKeyInfo key, T selectedItem, Picklist<T> parent);

    /// <summary>
    /// Defines the OnItemKeyHandler
    /// </summary>
    public event OnItemKey OnItemKeyHandler;

    /// <summary>
    /// The Refresh
    /// </summary>
    public override void Refresh()
    {
      base.RenderBorder();
      this.RenderControl();
    }

    /// <summary>
    /// Scrolls <see cref="IScrollable"/> list down
    /// </summary>
    /// <param name="Step">The <see cref="int"/></param>
    public override void ScrollDown(int Step)
    {
      if (TotalItems == 0) return;
      var prevSelectionPosition = Math.Max(SelectionPosition, 0);
      if (SelectedItemIndex + Step < TotalItems) { SelectedItemIndex += Step; } else { SelectedItemIndex = TotalItems - 1; }
      if (SelectionPosition + Step >= Math.Min(ListHeight, TotalItems - 1))
      {
        var prevSel = SelectionPosition;
        SelectionPosition = Math.Min(ListHeight - 1, TotalItems - 1);
        Offset += Step - (SelectionPosition - prevSel);

        if (Offset > TotalItems - ListHeight)
        {
          Offset = TotalItems - ListHeight;
          if (Offset < 0) { Offset = 0; }
        }
        RenderControl();
      }
      else
      {
        SelectionPosition = Math.Min(SelectionPosition + Step, TotalItems - 1);
        RenderItem(prevSelectionPosition);
        RenderItem(SelectionPosition);
      }
    }

    /// <summary>
    /// Scrolls <see cref="IScrollable"/> list up
    /// </summary>
    /// <param name="Step">The <see cref="int"/></param>
    public override void ScrollUp(int Step)
    {
      if (TotalItems == 0) return;
      var prevSelectionPosition = Math.Max(SelectionPosition, 0);
      if (SelectedItemIndex > Step) { SelectedItemIndex -= Step; } else { SelectedItemIndex = 0; }
      if (SelectionPosition > Step)
      {
        SelectionPosition -= Step;
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
          Offset -= Step;
          if (Offset < 0)
          {
            SelectionPosition += Offset;
            Offset = 0;
          }
          RenderControl();
        }
      }
    }

    /// <summary>
    /// Displays see <see cref="GuiElement"/>
    /// </summary>
    public override void Show()
    {
      base.Show();

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

      Select();
    }

    /// <summary>
    /// Updates and renders single item in list if necessary
    /// </summary>
    /// <param name="Index">The <see cref="int"/></param>
    public void UpdateItem(int Index)
    {
      if (Index >= Offset && Index < Offset + Height)
      {
        RenderItem(Index);
      }
    }

    /// <summary>
    /// Replaces list of items
    /// </summary>
    /// <param name="items">The <see cref="IEnumerable{T}"/></param>
    public void UpdateList(IEnumerable<T> items)
    {
      ListItems.Clear();
      foreach (var i in items)
      {
        i.PropertyChanged += ListItem_PropertyChanged;
        ListItems.Add(i);
      }
    }

    /// <summary>
    /// Disposes the object.
    /// </summary>
    /// <param name="disposing">The <see cref="bool"/></param>
    protected override void Dispose(bool disposing)
    {
      if (_disposed)
        return;

      listItems = null;

      _disposed = true;
    }

    /// <summary>
    /// Renders <see cref="GuiElement"/>
    /// </summary>
    protected override void RenderControl()
    {
      if (TotalItems == 0) { return; }
      this.IsDisplayed = true;
      for (int i = 0; i < Math.Min(ListHeight, TotalItems - Offset); i++)
      {
        RenderItem(i);
      }
    }

    /// <summary>
    /// Renders single list item.
    /// </summary>
    /// <param name="index">The <see cref="int"/></param>
    protected void RenderItem(int index)
    {
      if (ListItems == null) return;
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

    /// <summary>
    /// Notifyes that property has changed and renders updated item. 
    /// </summary>
    /// <param name="sender">The <see cref="object"/></param>
    /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/></param>
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

    /// <summary>
    /// Handles key press on list
    /// </summary>
    private void Select()
    {
      bool cont = true;
      do
      {
        var key = ConsoleWrapper.Instance.ReadKey(true);
        var prevSelectionPosition = Math.Max(SelectionPosition, 0);
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

          default:
            if (SelectedItemIndex >= 0 && SelectedItemIndex < this.TotalItems && OnItemKeyHandler != null)
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
  }
}

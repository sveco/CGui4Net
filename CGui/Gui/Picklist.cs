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
    public class Picklist<T> : GuiElement
    {
        public int Offset = 0;
        public bool ShowScrollbar { get; set; }
        private string ScrollBarChar = "█";
        private IList<ListItem<T>> listItems = new List<ListItem<T>>();
        public int SelectedItemIndex = 0;
        public int SelectionPos = 0;
        private Action<ListItem<T>, Picklist<T>> ProcessItem;
        public int TotalItems {get { return ListItems.Count();}}

        public override int Top { get; set; }
        public override int Left { get; set; }
        public override int Width { get; set; }
        public override int Height { get; set; }
        public IList<ListItem<T>> ListItems { get => listItems;
            set
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
                UpdateItem(((ListItem<T>)sender).Index + Offset);
            }
        }

        public Picklist(IList<ListItem<T>> items) {
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

        public override void Show() {

            if (TotalItems == 0) { return; }

            Console.CursorVisible = false;
            this.RenderControl();
            this.Select();
        }

        public override void Refresh() {
            Console.CursorVisible = false;
            this.RenderControl();
        }

        private void Select()
        {
            if (ProcessItem != null)
            {
                new Thread(() =>
                {
                    Thread.CurrentThread.IsBackground = true;
                    /* run your code here */
                    Parallel.ForEach<ListItem<T>>(this.ListItems, (item) => {
                        ProcessItem.Invoke(item, this);
                    });
                }).Start();
            }

            bool cont = true;
            do
            {
                var key = Console.ReadKey(true);
                var prevItem = SelectionPos;
                switch (key.Key)
                {
                    case ConsoleKey.UpArrow:
                        if (SelectedItemIndex > 0) { SelectedItemIndex--; }
                        if (SelectionPos - 1 < 0) {
                            if (Offset > 0) {
                                Offset--;
                                RenderControl();
                            }
                        }
                        else {
                            SelectionPos--;
                            RenderItem(prevItem);
                            RenderItem(SelectionPos);
                        }

                        break;
                    case ConsoleKey.DownArrow:
                        if (SelectedItemIndex + 1 < TotalItems) { SelectedItemIndex++; } else { break; }
                        if (SelectionPos + 1 >= Height)
                        {
                            if (Offset + Height < TotalItems) { Offset++; }
                            RenderControl();
                        }
                        else {
                            SelectionPos++;
                            RenderItem(prevItem);
                            RenderItem(SelectionPos);
                        }
                        break;

                    case ConsoleKey.PageUp:
                        if (SelectedItemIndex > 10) { SelectedItemIndex -= 10; } else { SelectedItemIndex = 0; }
                        if (SelectionPos - 10 < 0)
                        {
                            SelectionPos = 0;
                            if (Offset >= 10)
                            {
                                Offset -= 10;

                            }
                            else { Offset = 0; }
                            RenderControl();
                        }
                        else
                        {
                            SelectionPos -= 10;
                            RenderItem(prevItem);
                            RenderItem(SelectionPos);
                        }
                        break;

                    case ConsoleKey.PageDown:
                        if (SelectedItemIndex + 10 < TotalItems) { SelectedItemIndex += 10; } else { SelectedItemIndex = TotalItems - 1; }
                        if (SelectionPos + 10 > Height)
                        {
                            var prevSel = SelectionPos;
                            SelectionPos = Height - 1;
                            Offset += 10 - (SelectionPos - prevSel);

                            if (Offset > TotalItems - Height) {
                                Offset = TotalItems - Height;
                            }
                            RenderControl();
                        }
                        else
                        {
                            SelectionPos += 10;
                            RenderItem(prevItem);
                            RenderItem(SelectionPos);
                        }
                        break;

                    default:
                        if (SelectedItemIndex < this.TotalItems) {
                            cont = OnItemKeyHandler(key, this.ListItems[SelectedItemIndex], this);
                        } else {
                            cont = false;
                        }
                        break;
                    
                }

            } while (cont);
        }

        Object thisLock = new Object();
        protected void RenderItem(int Index)
        {
            lock (thisLock)
            {
                if (Index >= ListItems.Count()) return;
                Console.SetCursorPosition(this.Left, this.Top + Index);
                if (SelectionPos == Index)
                {
                    Console.ForegroundColor = this.SelectedForegroundColor;
                    Console.BackgroundColor = this.SelectedBackgroundColor;
                }
                ConsoleWrapper.WriteLine(GetDisplayText(Index + Offset));
                if (SelectionPos == Index)
                {
                    Console.ForegroundColor = this.ForegroundColor;
                    Console.BackgroundColor = this.BackgroundColor;
                }
            }
        }

        private void RenderControl()
        {
            Console.ForegroundColor = this.ForegroundColor;
            Console.BackgroundColor = this.BackgroundColor;
            for (int i = 0; i < Math.Min(Height, TotalItems - Offset); i++)
            {
                RenderItem(i);
            }
        }

        public void UpdateItem(int Index) {
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
                    result = result.PadRight(this.Width, this.PadChar);
                    break;

                case TextAlignment.Right:
                    result = result.PadLeft(this.Width, this.PadChar);
                    break;

                case TextAlignment.Center:
                    result = result.PadBoth(this.Width, this.PadChar);
                    break;
            }

            if (result.Length > this.Width)
            {
                if (result.Length > 3)
                {
                    result = result.Substring(0, this.Width - 3) + "...";
                }
                else
                {
                    result = result.Substring(0, this.Width);
                }
            }

            if (ShowScrollbar && Height < TotalItems)
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
                else {
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
    }
}

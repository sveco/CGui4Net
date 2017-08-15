using CGui.Gui.Primitives;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CGui.Gui
{
    public class List<T> : GuiElement
    {
        public int MaxItems = 8;
        public int Offset = 0;
        public bool ShowScrollbar = false;
        private string ScrollBarChar = "█";
        public IList<ListItem<T>> ListItems;
        public int SelectedItemIndex = 0;
        public int SelectionPos = 0;
        private Action<ListItem<T>, List<T>> processItem;

        public int TotalItems {get { return ListItems.Count();}}

        public override int Top { get; set; }
        public override int Left { get; set; }
        public override int Width { get; set; }

        public delegate bool OnItemKey(ConsoleKey key, ListItem<T> selectedItem);
        public event OnItemKey OnItemKeyHandler;

        public List(IList<ListItem<T>> items) {
            this.ListItems = items;
        }

        public List(IList<ListItem<T>> items, Action<ListItem<T>, List<T>> processItem)
        {
            this.ListItems = items;
            this.processItem = processItem;
        }

        public override void Show() {
            Console.CursorVisible = false;
            this.RenderControl();
            this.Select();
        }

        private void Select()
        {

            if (processItem != null)
            {
                Parallel.ForEach<ListItem<T>>(this.ListItems, (item) => {
                    processItem.Invoke(item, this);
                 });
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
                        if (SelectedItemIndex + 1 < TotalItems) { SelectedItemIndex++; }
                        if (SelectionPos + 1 >= MaxItems)
                        {
                            if (Offset + MaxItems < TotalItems) { Offset++; }
                            RenderControl();
                        }
                        else {
                            SelectionPos++;
                            RenderItem(prevItem);
                            RenderItem(SelectionPos);
                        }
                        break;

                    default:
                        cont = OnItemKeyHandler(key.Key, this.ListItems[SelectedItemIndex]);
                        break;
                    
                }

            } while (cont);
        }

        protected void RenderItem(int Index)
        {
            Console.SetCursorPosition(this.Left, this.Top + Index);
            if (SelectionPos == Index)
            {
                Console.ForegroundColor = this.SelectedForegroundColor;
                Console.BackgroundColor = this.SelectedBackgroundColor;
            }
            Console.WriteLine(GetDisplayText(Index + Offset));
            if (SelectionPos == Index)
            {
                Console.ForegroundColor = this.ForegroundColor;
                Console.BackgroundColor = this.BackgroundColor;
            }
        }

        private void RenderControl() {
            for (int i = 0; i < Math.Min(MaxItems, TotalItems - Offset); i++)
            {
                RenderItem(i);
            }
        }

        public void UpdateItem(int Index) {
            if (Index >= Offset && Index < Offset + MaxItems)
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

            if (ShowScrollbar && MaxItems < TotalItems)
            {
                var ratio = (double)MaxItems / TotalItems;
                int size = (int)Math.Ceiling((double)MaxItems * ratio);
                var top = Math.Ceiling(Offset * ratio);
                Debug.WriteLine(top);

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
    }
}

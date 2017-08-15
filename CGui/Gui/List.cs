using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGui.Gui
{
    public class List<T>: GuiElement
    {
        public int MaxItems = 8;
        public int Offset = 0;
        public IList<ListItem<T>> ListItems { get; private set; }
        public int SelectedItemIndex = 0;
        public int SelectionPos = 0;
        public int TotalItems {
            get { return ListItems.Count(); }
        }

        public delegate void OnItemKey(ListItem<T> selectedItem);
        public event OnItemKey OnItemKeyHandler;

        public List(IList<ListItem<T>> items) {
            this.ListItems = items;
        }

        public override void Show() {
            Console.CursorVisible = false;
            this.RenderElement();
            this.Select();
        }

        private void Select()
        {
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
                                RenderElement();
                            }
                        }
                        else {
                            SelectionPos--;
                            UpdateItem(prevItem);
                            UpdateItem(SelectionPos);
                        }

                        break;
                    case ConsoleKey.DownArrow:
                        if (SelectedItemIndex + 1 < TotalItems) { SelectedItemIndex++; }
                        if (SelectionPos + 1 >= MaxItems)
                        {
                            if (Offset + MaxItems < TotalItems) { Offset++; }
                            RenderElement();
                        }
                        else {
                            SelectionPos++;
                            UpdateItem(prevItem);
                            UpdateItem(SelectionPos);
                        }
                        break;
                    case ConsoleKey.Escape:
                        return;

                    default:
                        OnItemKeyHandler(this.ListItems[SelectedItemIndex]);
                        break;
                    
                }

            } while (true);
        }

        protected void UpdateItem(int Index)
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

        protected void RenderElement() {
            for (int i = 0; i < Math.Min(MaxItems, TotalItems - Offset); i++)
            {
                UpdateItem(i);
            }
        }

        protected string GetDisplayText(int index)
        {
            string result = ListItems[index].DisplayText;
            switch (this.TextAlignment)
            {
                case TextAlignment.Left:
                    return result.PadRight(this.Width);

                case TextAlignment.Right:
                    return result.PadLeft(this.Width);

                case TextAlignment.Center:
                    return result.PadBoth(this.Width);
            }
            return result;
        }
    }
}

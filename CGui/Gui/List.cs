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
                var prevItem = SelectedItemIndex;
                switch (key.Key)
                {
                    case ConsoleKey.UpArrow:
                        if (SelectedItemIndex - 1 < Offset) { SelectedItemIndex = Math.Min(MaxItems, ListItems.Count()) - 1; }
                        else { SelectedItemIndex--; }
                        break;
                    case ConsoleKey.DownArrow:
                        if (SelectedItemIndex + 1 >= Math.Min(MaxItems, ListItems.Count())) { SelectedItemIndex = Offset; }
                        else { SelectedItemIndex++; }
                        break;
                    case ConsoleKey.Escape:
                        return;

                    default:
                        OnItemKeyHandler(this.ListItems[SelectedItemIndex]);
                        break;
                    
                }
                UpdateItem(prevItem);
                UpdateItem(SelectedItemIndex);
            } while (true);
        }

        protected void UpdateItem(int Index)
        {
            Console.SetCursorPosition(this.Left, this.Top + Index);
            if (SelectedItemIndex == Index)
            {
                Console.ForegroundColor = this.SelectedForegroundColor;
                Console.BackgroundColor = this.SelectedBackgroundColor;
            }
            Console.WriteLine(ListItems[Index].DisplayText);
            if (SelectedItemIndex == Index)
            {
                Console.ForegroundColor = this.ForegroundColor;
                Console.BackgroundColor = this.BackgroundColor;
            }
        }

        protected void RenderElement() {
            Console.SetCursorPosition(this.Left, this.Top);
            int displayed = 0;

            for (int i = Offset; i < Math.Min(MaxItems, ListItems.Count()); i++)
            {
                if (displayed < Offset) { continue; }

                UpdateItem(i);
            }
        }
    }
}

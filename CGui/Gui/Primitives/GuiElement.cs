using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGui.Gui.Primitives
{
    public enum TextAlignment {
        Left,
        Center,
        Right
    }
    public abstract class GuiElement
    {
        public ConsoleColor ForegroundColor = Console.ForegroundColor;
        public ConsoleColor BackgroundColor = Console.BackgroundColor;

        public ConsoleColor SelectedForegroundColor = ConsoleColor.DarkBlue;
        public ConsoleColor SelectedBackgroundColor = ConsoleColor.Green;

        public bool AutoRefresh = true;

        abstract public int Top { get; set; }
        abstract public int Left { get; set; }
        abstract public int Width { get; set; }
        abstract public int Height { get; set; }

        public abstract void Show();

        public void Refresh() {
            Show();
        }

        public TextAlignment TextAlignment = TextAlignment.Left;
        public char PadChar = ' ';
    }
}

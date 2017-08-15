using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGui.Gui
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

        public string Header { get; set; }
        public string Footer { get; set; }

        public int Top = 0;
        public int Left = 0;
        public int Width = 10;

        public abstract void Show();

        public TextAlignment TextAlignment = TextAlignment.Left;
    }
}

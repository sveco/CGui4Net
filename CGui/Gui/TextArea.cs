using CGui.Gui.Primitives;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CGui.Gui
{
    public class TextArea : GuiElement
    {
        private static readonly object lockObject = new object();

        public bool ShowScrollbar = false;
        private string ScrollBarChar = "█";

        public override int Top { get; set; }
        public override int Left { get; set; }
        private int _width = 0;
        public override int Width {
            get { return _width; }
            set
            {
                _width = value;
                if (value > 0)
                {
                    parseText();
                }
            }
        }

        public override int Height { get; set; }

        public int Offset { get; internal set; }
        public bool WaitForInput { get; set; }

        public delegate bool OnItemKey(ConsoleKeyInfo key);
        public event OnItemKey OnItemKeyHandler;

        string _content;
        public string Content {
            get { return _content; }
            set {
                _content = value;
                if (_width > 0)
                {
                    parseText();
                }
            }
        }

        private IList<string> _lines = new List<string>();
        public int LinesCount { get { return _lines.Count(); } }

        private IList<string> parseText () {
            _lines = new List<string>();
            if (!string.IsNullOrWhiteSpace(Content))
            {
                _lines = Content.Split(this.Width - 5).ToList();
            }

            return _lines;
        }

        public TextArea(string content)
        {
            this.Content = content;
            Offset = 0;
        }

        protected string GetDisplayText(int index)
        {
            string result = _lines[index];
            int desiredWith = (ShowScrollbar && Height < LinesCount) ? this.Width - 1 : this.Width;

            switch (this.TextAlignment)
            {
                case TextAlignment.Left:
                    result = result.PadRight(desiredWith, this.PadChar);
                    break;

                case TextAlignment.Right:
                    result = result.PadLeft(desiredWith, this.PadChar);
                    break;

                case TextAlignment.Center:
                    result = result.PadBoth(desiredWith, this.PadChar);
                    break;
            }

            if (ShowScrollbar && Height < LinesCount)
            {
                var ratio = (double)Height / LinesCount;
                int size = (int)Math.Ceiling((double)Height * ratio);
                var top = Math.Ceiling(Offset * ratio);
                Debug.WriteLine(top);

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

        private void renderControl() {
            lock (lockObject)
            {
                for (int i = 0; i < Math.Min(Height, _lines.Count - Offset); i++)
                {
                    Console.SetCursorPosition(Left, Top + i);
                    //Console.WriteLine(GetDisplayText(Offset + i));
                    ConsoleWrapper.WriteLine(GetDisplayText(Offset + i));
                }
            }
        }

        public override void Show()
        {
            renderControl();
            if (this.WaitForInput)
            {
                inputLoop();
            }
        }

        public override void Refresh()
        {
            renderControl();
        }

        private void inputLoop()
        {
            bool cont = true;
            do
            {
                var key = Console.ReadKey(true);

                switch (key.Key)
                {
                    case ConsoleKey.UpArrow:
                        if (Offset > 0) { Offset--; }
                        renderControl();
                        break;

                    case ConsoleKey.DownArrow:
                        if (_lines.Count > Height + Offset) {
                            Offset++;
                            renderControl();
                        }
                        break;

                    case ConsoleKey.PageUp:
                        if (Offset > 10) { Offset = Offset - 10; } else { Offset = 0; }
                        renderControl();
                        break;

                    case ConsoleKey.PageDown:
                        if (_lines.Count - 10 > Height + Offset)
                        {
                            Offset = Offset + 10;
                            renderControl();
                        }
                        break;

                    case ConsoleKey.Escape:
                        cont = false;
                        break;

                    default:
                        if (OnItemKeyHandler != null)
                        {
                            cont = OnItemKeyHandler(key);
                        }
                        break;

                }

            } while (cont);
        }
    }
}

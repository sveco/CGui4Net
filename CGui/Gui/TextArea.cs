using CGui.Gui.Primitives;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGui.Gui
{
    public class TextArea : GuiElement
    {
        public override int Top { get; set; }
        public override int Left { get; set; }
        private int _width = 10;
        public override int Width {
            get { return _width; }
            set
            {
                _width = value;
                parseText();
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
                parseText();
            }
        }

        private IList<string> _lines = new List<string>();
        public int LinesCount { get { return _lines.Count(); } }

        private IList<string> parseText () {
            _lines = new List<string>();
            if (!string.IsNullOrWhiteSpace(Content))
            {
                _lines = Content.Split(this.Width).ToList();
            //    var paragraphs = Content.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            //    foreach (var paragraph in paragraphs)
            //    {
            //        var lines = paragraph.Split(this.Width);
            //        _lines = _lines.Concat(lines).ToList();
            //    }
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
            return result;
        }

        private void renderControl() {
            for (int i = 0; i < Math.Min(Height, _lines.Count - Offset); i++)
            {
                Console.SetCursorPosition(Left, Top + i);
                Console.WriteLine(GetDisplayText(Offset + i).Replace("\n", ""));
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

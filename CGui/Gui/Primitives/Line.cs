using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGui.Gui.Primitives
{
    public class Line: GuiElement
    {
        private string _displayText;
        public string DisplayText {
            get { return _displayText; }
            set {
                _displayText = value;
                Show();
            }
        }
        int _top;
        public override int Top { get => _top; set => _top = value; }
        public override int Left { get => 0; set {}}
        public override int Width { get => Console.WindowWidth; set {}}

        protected void RenderElement()
        {
            Console.SetCursorPosition(this.Left, this.Top);
            Console.WriteLine(FormatDisplayText(DisplayText));
        }

        protected string FormatDisplayText(string displayText)
        {
            string result = displayText;
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
        public override void Show()
        {
            Console.CursorVisible = false;
            this.RenderElement();
            Console.SetCursorPosition(0, 0);
        }
    }
}

using CGui.Gui.Primitives;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace CGui.Gui
{
    public class Viewport : GuiElement
    {
        public Viewport()
        {
            this.Width = Console.WindowWidth;
            this.Height = Console.WindowHeight;
        }

        public override int Top { get => 0; set {}}
        public override int Left { get=> 0; set {}}
        public override int Width {
            get {
                return Console.WindowWidth;
            }
            set {
                Console.SetWindowSize(Math.Min(value,Console.LargestWindowWidth), this.Height);
            }
        }
        public override int Height {
            get {
                return Console.WindowHeight;
            }
            set {
                Console.SetWindowSize(this.Width, Math.Min(value, Console.LargestWindowHeight));
                }
            }

        public Collection<GuiElement> Controls = new Collection<GuiElement>();

        private void Clear()
        {
            Console.Clear();
        }

        public override void Show()
        {
            Clear();
            foreach (var e in Controls)
            {
                if (e != null) { e.Show(); }
            }
        }

        public override void Refresh()
        {
            Clear();
            foreach (var e in Controls)
            {
                if (e != null) { e.Refresh(); }
            }
        }
    }
}

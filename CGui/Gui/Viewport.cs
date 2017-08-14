using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGui.Gui
{
    public class Viewport
    {
        public Viewport()
        {
            this.Width = Console.WindowWidth;
            this.Height = Console.WindowHeight;
        }

        public int Width { get; private set; }
        public int Height { get; private set; }
    }
}

using CGui.Gui.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGui.Gui
{
    public class Footer : Line
    {
        public override int Top { get => Console.WindowHeight - 2; set { } }
        public Footer(string text)
        {
            DisplayText = text;
        }
    }
}

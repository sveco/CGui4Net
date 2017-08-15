using CGui.Gui.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGui.Gui
{
    public class Header : Line
    {
        public override int Top { get => 0; set { }}
        public Header (string text)
        {
            DisplayText = text;
        }
    }
}

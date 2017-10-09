using CGui.Gui.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGui.Gui
{
    public class Footer : Row
    {
        public override int Top { get => ConsoleWrapper.Instance.WindowHeight - 1; set { } }
        public Footer(string text)
        {
            DisplayText = text;
        }
    }
}

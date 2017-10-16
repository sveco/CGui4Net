using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGui.Gui.Primitives
{
  public interface IScrollable
  {
    int Offset { get; set; }
    bool ShowScrollBar { get; set; }
  }
}

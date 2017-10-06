using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGui.Gui.Primitives
{
  public class HorizontalLine : GuiElement
  {
    public override int Top { get; set; }
    public override int Left { get; set; }
    public override int Width { get => 1; set {} }
    public override int Height { get; set; }

    public override void Refresh()
    {
      throw new NotImplementedException();
    }

    protected override void RenderControl()
    {
      
    }
  }
}

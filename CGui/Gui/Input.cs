using CGui.Gui.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGui.Gui
{
  public class Input : Line
  {
    public string Prompt { get; set; }
    public Input(string prompt)
    {
      DisplayText = prompt;

    }

    public string InputText
    {
      get
      {
        base.Show();
        lock (Console.Lock)
        {
          Console.SetCursorPosition(this.DisplayText.Length + 1, Top);
          var s = Console.ReadLineWithCancel();
          this.ClearCurrentLine();
          return s;
        }
      }
    }
  }
}

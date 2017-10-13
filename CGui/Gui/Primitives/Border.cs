using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGui.Gui.Primitives
{
  public enum BorderWeight
  {
    Light = 0,
    Heavy = 1
  }

  public enum BorderStyle
  {
    None = -1,
    Single = 0,
    Double = 1,
    DoubleDash = 2,
    TripleDash = 3,
    QuadrupleDash = 4
  }

  public enum PositionV
  {
    Top = 0,
    Middle = 1,
    Bottom = 2
  }

  public enum PositionH
  {
    Left = 0,
    Middle = 1,
    Right = 2
  }

  public class Border
  {
    private BorderStyle _style = BorderStyle.Single;
    private BorderWeight _weight = BorderWeight.Light;
    private ConsoleColor _foregroundColor = ConsoleWrapper.Instance.ForegroundColor;
    private ConsoleColor _backgroundColor = ConsoleWrapper.Instance.BackgroundColor;

    char[,,][] borders =
        {{
          {
            new char[] {'┌','─','┐'},
            new char[] {'│',' ','│'},
            new char[] {'└','─','┘'}
          },{
            new char[] {'┏','━','┓'},
            new char[] {'┃',' ','┃'},
            new char[] {'┗','━','┛'}
        }},
        {{
            new char[] {'╔','═','╗'},
            new char[] {'║',' ','║'},
            new char[] {'╚','═','╝'}
          },{
            new char[] {'╔','═','╗'},
            new char[] {'║',' ','║'},
            new char[] {'╚','═','╝'}
        }},
        {{
            new char[] {'┌','╌','┐'},
            new char[] {'╎',' ','╎'},
            new char[] {'└','╌','┘' }
          },{
            new char[] {'┏','╍','┓'},
            new char[] {'╏',' ','╏'},
            new char[] {'┗','╍','┛' }
        }},
        {{
            new char[] {'┌','┄','┐'},
            new char[] {'┆',' ','┆'},
            new char[] {'└','┄','┘'}
          },{
            new char[] {'┏','┅','┓'},
            new char[] {'┇',' ','┇'},
            new char[] {'┗','┅','┛'}
        }},
        {{
            new char[] {'┌','┈','┐'},
            new char[] {'┊',' ','┊'},
            new char[] {'└','┈','┘' }
          },{
            new char[] {'┏','┉','┓'},
            new char[] {'┋',' ','┋'},
            new char[] {'┗','┉','┛'}
        }}
      };

    public BorderWeight Weight { get => _weight; set => _weight = value; }
    public BorderStyle Style { get => _style; set => _style = value; }

    public ConsoleColor ForegroundColor { get => _foregroundColor; set => _foregroundColor = value; }
    public ConsoleColor BackgroundColor { get => _backgroundColor; set => _backgroundColor = value; }

    public char Get(PositionV v, PositionH h)
    {
      return borders[(int)Style, (int)Weight,(int)v][(int)h];
    }
  }
}

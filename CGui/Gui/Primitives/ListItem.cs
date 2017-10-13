using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CGui.Gui.Primitives
{
  public abstract class ListItem : INotifyPropertyChanged, IListItem
  {
    private int _index;
    public int Index {
      get { return _index; }
      set {
        if (_index != value)
        {
          _index = value;
          NotifyPropertyChanged();
        }
      }
    }
    private string _displayText = string.Empty;
    public string DisplayText {
        get { return _displayText; }
        set {
          if (_displayText != value)
          {
            _displayText = value;
            NotifyPropertyChanged();
          }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}

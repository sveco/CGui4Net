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
    public class ListItem<T> : INotifyPropertyChanged
    {
        public int Index;
        private string _displayText = string.Empty;
        public string DisplayText {
            get { return _displayText; }
            set {
                _displayText = value;
                NotifyPropertyChanged();
            }
        }
        public T Value;

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

using System.ComponentModel;

namespace CGui.Gui.Primitives
{
  public interface IListItem
  {
    string DisplayText { get; set; }
    int Index { get; set; }

    event PropertyChangedEventHandler PropertyChanged;
  }
}
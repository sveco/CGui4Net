using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CGui.Gui.Primitives;

namespace CGui.Gui
{
  public class DialogChoice : ListItem {
    public object Value { get; set; }
  }

  public class Dialog : GuiElement
  {
    public event EventHandler<DialogChoice> ItemSelected;

    protected virtual void OnItemSelected(DialogChoice c)
    {
      ItemSelected?.Invoke(this, c);
    }

    public string Text { get; set; }

    private Dictionary<string, object> _buttons;
    public Dictionary<string, object> Buttons { get => _buttons; private set => _buttons = value; }
    public int SelectedItemIndex { get; set; }

    public Dialog(string Text, Dictionary<string, object> Buttons) {
      this.Text = Text;
      this.Buttons = Buttons;
      this.BorderStyle = BorderStyle.Double;

      this.Height = _buttons.Count + 4;
      this.Top = (ConsoleWrapper.Instance.WindowHeight - this.Height) / 2;
      this.Width = Text.Length + 8;
      this.Left = (ConsoleWrapper.Instance.WindowWidth - this.Width) / 2;
    }

    public override void Show()
    {
      this.Clear();
      base.Show();
    }

    protected override void RenderControl()
    {
      TextArea t = new TextArea(Text);
      t.Top = this.Top + 1;
      t.Left = this.Left + 1;
      t.Width = this.Width -2;
      t.Height = 1;
      t.TextAlignment = TextAlignment.Center;
      t.WaitForInput = false;

      Picklist<DialogChoice> p = new Picklist<DialogChoice>(
        _buttons.Select(x => new DialogChoice() { DisplayText = x.Key, Value = x.Value }).ToList());
      p.Top = this.Top + 3;
      p.Left = this.Left + 1;
      p.Width = this.Width - 2;
      p.Height = _buttons.Count;
      p.SelectedItemIndex = this.SelectedItemIndex;
      p.OnItemKeyHandler += P_OnItemKeyHandler;

      t.Show();
      p.Show();
    }

    private bool P_OnItemKeyHandler(ConsoleKeyInfo key, DialogChoice selectedItem, Picklist<DialogChoice> parent)
    {
      //throw new NotImplementedException();
      OnItemSelected(selectedItem);
      return false;
    }

    private bool _disposed;
    protected override void Dispose(bool disposing)
    {
      if (_disposed)
        return;

      if (disposing)
      {
        // free other managed objects that implement
        // IDisposable only
      }

      // release any unmanaged objects
      // set object references to null
      _disposed = true;
    }
  }
}

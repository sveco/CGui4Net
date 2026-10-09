using CGui.Gui;
using CGui.Gui.Primitives;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGuiDemo
{
  class MyListItem : ListItem {
    public string Value { get; set; }
  }

  class Program
  {
    private static Viewport _mainview;

    static MyListItem[] list = {
            new MyListItem() { Value = "1", DisplayText = "Test 1", Index = 0},
            new MyListItem() { Value = "2", DisplayText = "Test 2", Index = 1},
            new MyListItem() { Value = "3", DisplayText = "Test 3", Index = 2},
            new MyListItem() { Value = "4", DisplayText = "Input", Index = 3},
            new MyListItem() { Value = "5", DisplayText = "Text Area", Index = 4},
            new MyListItem() { Value = "6", DisplayText = "Dialog", Index = 5},
            new MyListItem() { Value = "7", DisplayText = "Test 7", Index = 6},
            new MyListItem() { Value = "8", DisplayText = "Test 8 Loooooooooooooooooooooooooooooong text", Index = 7},
            new MyListItem() { Value = "9", DisplayText = "Test 9", Index = 8},
            new MyListItem() { Value = "10", DisplayText = "Test 10", Index = 9},
            new MyListItem() { Value = "11", DisplayText = "Test 11", Index = 10},
            new MyListItem() { Value = "12", DisplayText = "Test 12", Index = 11},
            new MyListItem() { Value = "13", DisplayText = "Test 13", Index = 12},
            new MyListItem() { Value = "14", DisplayText = "Test 14", Index = 13},
            new MyListItem() { Value = "15", DisplayText = "Test 15", Index = 14},
            new MyListItem() { Value = "16", DisplayText = "I am scrollable too..", Index = 15}
        };

    public static void HandleKey(MyListItem item)
    {
      Debug.WriteLine(item.Value);
    }

    static Header h;
    static Footer f;

    /// <summary>
    /// Shows a list or a text that fills the console window, to try what happens when the window is
    /// resized: CGuiDemo --resize list, or CGuiDemo --resize text. Esc quits.
    /// </summary>
    [System.Runtime.InteropServices.DllImport("kernel32.dll")]
    static extern IntPtr GetStdHandle(int handle);

    [System.Runtime.InteropServices.DllImport("kernel32.dll")]
    static extern bool GetConsoleMode(IntPtr handle, out uint mode);

    [System.Runtime.InteropServices.DllImport("kernel32.dll")]
    static extern bool SetConsoleMode(IntPtr handle, uint mode);

    /// <summary>
    /// The scrollbar is drawn with ANSI colour codes. The console shows them as text unless virtual
    /// terminal processing is on, which is not the default in older consoles.
    /// </summary>
    static void EnableVirtualTerminal()
    {
      const int StdOutputHandle = -11;
      const uint EnableVirtualTerminalProcessing = 0x0004;
      var handle = GetStdHandle(StdOutputHandle);
      uint mode;
      if (GetConsoleMode(handle, out mode))
      {
        SetConsoleMode(handle, mode | EnableVirtualTerminalProcessing);
      }
    }

    static void ResizeDemo(string mode)
    {
      EnableVirtualTerminal();
      var view = new Viewport();

      view.Controls.Add(new Header("CGui resize demo (" + mode + ")") { TextAlignment = TextAlignment.Center, PadChar = '=' });
      view.Controls.Add(new Footer(" Esc:Quit Up/Down:Move PageUp/PageDown:Scroll - this footer is long on purpose, it is shortened when the window is narrow ") { PadChar = '=' });

      if (mode == "text" || mode == "late")
      {
        var paragraph = "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.";
        var text = string.Join(Environment.NewLine, Enumerable.Range(1, 40).Select(i => i + ". " + paragraph));

        // negative sizes are relative to the console: the width of the console minus 3, and so on
        var area = new TextArea(text)
        {
          Top = 1,
          Left = 2,
          Width = -3,
          Height = -3,
          ShowScrollBar = true,
          WaitForInput = mode == "text"
        };
        view.Controls.Add(area);

        if (mode == "late")
        {
          // The viewport is shown first and returns at once, because no control waits for keys yet. The
          // keyboard loop of the text area starts afterwards, outside of Viewport.Show. Applications that
          // load their content in the background do this.
          view.Show();
          area.WaitForInput = true;
          area.Show();
          return;
        }
      }
      else
      {
        var items = Enumerable.Range(1, 60)
          .Select(i => new MyListItem() { Value = i.ToString(), DisplayText = "Item " + i, Index = i - 1 })
          .ToArray();
        var l = new Picklist<MyListItem>(items, null)
        {
          Top = 1,
          Left = 2,
          Width = -3,
          Height = -3,
          ShowScrollBar = true,
          SelectedForegroundColor = ConsoleColor.White,
          SelectedBackgroundColor = ConsoleColor.Magenta
        };
        l.OnItemKeyHandler += (key, selectedItem, parent) => key.Key != ConsoleKey.Escape;
        view.Controls.Add(l);
      }

      view.Show();
    }

    static void Main(string[] args)
    {
      if (args.Length > 0 && args[0] == "--resize")
      {
        ResizeDemo(args.Length > 1 ? args[1] : "list");
        return;
      }

      _mainview = new Viewport();
      _mainview.Height = 20;
      _mainview.Width = 100;


      h = new CGui.Gui.Header("Test App 1");
      h.TextAlignment = TextAlignment.Center;
      h.PadChar = '=';

      _mainview.Controls.Add(h);

      f = new CGui.Gui.Footer("Status bar here  ");
      f.TextAlignment = TextAlignment.Right;
      f.PadChar = '=';

      _mainview.Controls.Add(f);

      var l = new CGui.Gui.Picklist<MyListItem>(list, null)
      {
        Left = 1,
        Top = 2,
        Width = 38,
        Height = 12,
        ShowScrollBar = true,
        TextAlignment = TextAlignment.Left,
        SelectedBackgroundColor = ConsoleColor.Magenta,
        BorderStyle = BorderStyle.Single,
        BorderForegroundColor = ConsoleColor.DarkMagenta
      };
      l.OnItemKeyHandler += List_OnItemKeyHandler;
      _mainview.Controls.Add(l);

      _mainview.Show();
    }

    private static bool List_OnItemKeyHandler(ConsoleKeyInfo key, MyListItem selectedItem, Picklist<MyListItem> parent)
    {
      if (key.Key == ConsoleKey.Enter)
      {
        Debug.WriteLine(selectedItem.Value);
        if (selectedItem.Value == "4")
        {
          var input = new Input("Input some text. Hit escape to cancel.")
          {
            Top = 15
          };
          //Referencing the value is what triggers prompt
          Debug.WriteLine(input.InputText);
        }
        else if (selectedItem.Value == "5")
        {
          TextArea text = new TextArea(@"Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.
Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.")
          {
            Top = 2,
            Left = 50,
            Height = 12,
            Width = 20,
            ShowScrollBar = true,
            WaitForInput = true,
            BorderStyle = BorderStyle.Double
          };

          text.Show();

          if (_mainview != null)
          {
            _mainview.Refresh();
          }
        }
        else if (selectedItem.Value == "6")
        {
          Dictionary<string, object> choices = new Dictionary<string, object>();
          choices.Add("Yes", 1);
          choices.Add("No", 2);
          choices.Add("Maybe", 3);
          var dialog = new Dialog("Are you ok?", choices);

          dialog.Show();

          if (_mainview != null)
          {
            _mainview.Refresh();
          }

        }

      }
      if (key.Key == ConsoleKey.Escape)
      {
        ///returning false exits the keyboard loop
        return false;
      }
      ///return true to continue capturing keyboard input
      return true;
    }
  }
}

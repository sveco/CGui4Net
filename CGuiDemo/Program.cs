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
    class Program
    {
        static ListItem<string>[] list = {
            new ListItem<string>() { DisplayText = "Test 1", Index = 1, Value = "Test Value 1" },
            new ListItem<string>() { DisplayText = "Test 2", Index = 1, Value = "Test Value 2" },
            new ListItem<string>() { DisplayText = "Test 3", Index = 1, Value = "Test Value 3" },
            new ListItem<string>() { DisplayText = "Test 4", Index = 1, Value = "Test Value 4" },
            new ListItem<string>() { DisplayText = "Test 5", Index = 1, Value = "Test Value 5" },
            new ListItem<string>() { DisplayText = "Test 6", Index = 1, Value = "Test Value 6" },
            new ListItem<string>() { DisplayText = "Test 7", Index = 1, Value = "Test Value 7" },
            new ListItem<string>() { DisplayText = "Test 8", Index = 1, Value = "Test Value 8" },
            new ListItem<string>() { DisplayText = "Test 9", Index = 1, Value = "Test Value 9" },
            new ListItem<string>() { DisplayText = "Test 10", Index = 1, Value = "Test Value 10" },
            new ListItem<string>() { DisplayText = "Test 11", Index = 1, Value = "Test Value 11" },
            new ListItem<string>() { DisplayText = "Test 12", Index = 1, Value = "Test Value 12" },
            new ListItem<string>() { DisplayText = "Test 13", Index = 1, Value = "Test Value 13" },
            new ListItem<string>() { DisplayText = "Test 14", Index = 1, Value = "Test Value 14" },
            new ListItem<string>() { DisplayText = "Test 15", Index = 1, Value = "Test Value 15" },
            new ListItem<string>() { DisplayText = "Test 16", Index = 1, Value = "Test Value 16" }
        };

        public static void HandleKey(ListItem<string> item)
        {
            Debug.WriteLine(item.Value);
        }

        static Header h;
        static Footer f;

        static void Main(string[] args)
        {
            h = new CGui.Gui.Header("Test App 1");
            h.TextAlignment = TextAlignment.Center;
            h.PadChar = '=';
            h.Show();

            f = new CGui.Gui.Footer("Status bar here  ");
            f.TextAlignment = TextAlignment.Right;
            f.PadChar = '=';
            f.Show();

            var l = new CGui.Gui.List<string>(list);
            l.OnItemKeyHandler += List_OnItemKeyHandler;
            l.Left = 0;
            l.Top = 2;
            l.Width = 40;
            l.TextAlignment = TextAlignment.Left;
            l.ShowScrollbar = false;
            l.Show();
        }



        private static bool List_OnItemKeyHandler(ConsoleKey key, ListItem<string> selectedItem)
        {
            if (key == ConsoleKey.Enter)
            {
                //Debug.WriteLine(selectedItem.Value);
                if (f != null)
                {
                    f.DisplayText = "Selected: " + selectedItem.Value;
                }
            }
            if (key == ConsoleKey.Escape)
            {
                return false;
            }

            return true;
        }
    }
}

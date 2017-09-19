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
            new ListItem<string>() { DisplayText = "Test 1", Index = 0, Value = "Test Value 1" },
            new ListItem<string>() { DisplayText = "Test 2", Index = 1, Value = "Test Value 2" },
            new ListItem<string>() { DisplayText = "Test 3", Index = 2, Value = "Test Value 3" },
            new ListItem<string>() { DisplayText = "Test 4", Index = 3, Value = "Test Value 4" },
            new ListItem<string>() { DisplayText = "Test 5", Index = 4, Value = "Test Value 5" },
            new ListItem<string>() { DisplayText = "Test 6", Index = 5, Value = "Test Value 6" },
            new ListItem<string>() { DisplayText = "Test 7", Index = 6, Value = "Test Value 7" },
            new ListItem<string>() { DisplayText = "Test 8 Loooooooooooooooooooooooooooooong text", Index = 7, Value = "Test Value 8" },
            new ListItem<string>() { DisplayText = "Test 9", Index = 8, Value = "Test Value 9" },
            new ListItem<string>() { DisplayText = "Test 10", Index = 9, Value = "Test Value 10" },
            new ListItem<string>() { DisplayText = "Test 11", Index = 10, Value = "Test Value 11" },
            new ListItem<string>() { DisplayText = "Test 12", Index = 11, Value = "Test Value 12" },
            new ListItem<string>() { DisplayText = "Test 13", Index = 12, Value = "Test Value 13" },
            new ListItem<string>() { DisplayText = "Test 14", Index = 13, Value = "Test Value 14" },
            new ListItem<string>() { DisplayText = "Test 15", Index = 14, Value = "Test Value 15" },
            new ListItem<string>() { DisplayText = "Test 16", Index = 15, Value = "Test Value 16" }
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

            void processItem(ListItem<string> i, CGui.Gui.Picklist<string> parent)
            {
                var rnd = new Random().Next(5000);
                System.Threading.Thread.Sleep(rnd);
                i.DisplayText += " - Updated";
                parent.UpdateItem(i.Index);
            }

            var l = new CGui.Gui.Picklist<string>(list, /* processItem*/ null)
            {
                Left = 0,
                Top = 2,
                Width = 40,
                Height = 6,
                ShowScrollbar = true,
                TextAlignment = TextAlignment.Left,
            };
            l.OnItemKeyHandler += List_OnItemKeyHandler;
            l.Show();
        }

        private static bool List_OnItemKeyHandler(ConsoleKeyInfo key, ListItem<string> selectedItem, Picklist<string> parent)
        {
            if (key.Key == ConsoleKey.Enter)
            {
                Debug.WriteLine(selectedItem.Value);
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

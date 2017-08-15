﻿using CGui.Gui;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGui
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
            new ListItem<string>() { DisplayText = "Test 13", Index = 1, Value = "Test Value 13" }
        };

        public static void HandleKey(ListItem<string> item)
        {
            Debug.WriteLine(item.Value);
        }

        static void Main(string[] args)
        {
            var l = new Gui.List<string>(list);
            l.OnItemKeyHandler += List_OnItemKeyHandler;
            l.Left = 10;
            l.Top = 2;
            l.TextAlignment = TextAlignment.Center;
            l.Show();

            Console.ReadKey();
        }

        private static void List_OnItemKeyHandler(ListItem<string> selectedItem)
        {
            Debug.WriteLine(selectedItem.Value);
        }
    }
}

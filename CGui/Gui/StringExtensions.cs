using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGui.Gui
{
    public static class StringExtensions
    {
        public static string PadBoth(this string str, int length)
        {
            return PadBoth(str, length, ' ');
        }
        public static string PadBoth(this string str, int length, char padChar)
        {
            int spaces = length - str.Length;
            int padLeft = spaces / 2 + str.Length;
            return str.PadLeft(padLeft, padChar).PadRight(length, padChar);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CGui.Gui
{
    /// <summary>
    /// Class that converts color tags to actual console color when rendering line.
    /// </summary>
    public static class ConsoleWrapper
    {
        static string regexExcape = @"\p{C}\[([fb]?)\:?(\w+)\]";
        static Regex regex = new Regex(regexExcape);
        private static ConsoleColor previousForeground;
        private static ConsoleColor previousBackground;

        public static void WriteLine(string s)
        {
            SaveColor();
            var chunks = Regex.Split(s, regexExcape, RegexOptions.ExplicitCapture);
            var matches = regex.Matches(s);

            for (int i = 0; i < chunks.Length; i++)
            {
                if (i > 0)
                {
                    var match = matches[i - 1];
                    var cap0 = match.Groups[0];
                    var cap1 = match.Groups[1];
                    var cap2 = match.Groups[2];

                    if (cap2.Value == "Reset")
                    {
                        RestoreColor();
                    }
                    else if (cap0 != null && cap1 != null && cap2 != null)
                    {
                        switch (cap1.Value)
                        {
                            case "f":
                                Console.ForegroundColor = cap2.Value.GetColor();
                                break;
                            case "b":
                                Console.BackgroundColor = cap2.Value.GetColor();
                                break;
                        }
                    }
                }
                Console.Write(chunks[i]);
            }
            RestoreColor();
            Console.Write(Environment.NewLine);
        }

        private static void SaveColor()
        {
            previousForeground = Console.ForegroundColor;
            previousBackground = Console.BackgroundColor;
        }

        private static void RestoreColor()
        {
            Console.ForegroundColor = previousForeground;
            Console.BackgroundColor = previousBackground;
        }
    }
}

namespace CGui.Gui
{
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
	using System.Threading.Tasks;

	public static class EnumExtensions
	{
		public static string GetColorCode(this ConsoleColor c, bool lowercase)
		{
			switch (c)
			{
				case ConsoleColor.Black:
					return "\x1b[30m]";
				case ConsoleColor.Blue:
					return "\x1B[94m";
				case ConsoleColor.DarkBlue:
					return "\x1B[34m";
				case ConsoleColor.Cyan:
					return "\x1B[96m";
				case ConsoleColor.DarkCyan:
					return "\x1B[36m";
				case ConsoleColor.Gray:
					return "\x1B[37m";
				case ConsoleColor.DarkGray:
					return "\x1B[90m";
				case ConsoleColor.Green:
					return "\x1B[92m";
				case ConsoleColor.DarkGreen:
					return "\x1B[92m";
				case ConsoleColor.Magenta:
					return "\x1B[95m";
				case ConsoleColor.DarkMagenta:
					return "\x1B[35m";
				case ConsoleColor.Red:
					return "\x1B[91m";
				case ConsoleColor.DarkRed:
					return "\x1B[31m";
				case ConsoleColor.White:
					return "\x1B[97m";
				case ConsoleColor.Yellow:
					return "\x1B[93m";
				case ConsoleColor.DarkYellow:
					return "\x1B[33m";

			}
			return string.Empty;
		}
	}
}

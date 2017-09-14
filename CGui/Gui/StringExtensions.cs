using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

		public static IEnumerable<string> Split(this string str, int chunkSize)
		{
			if (chunkSize < 1) throw new ArgumentException("Chunk size must be > 0.", "chunkSize");

			char[] splitChars = new char[] { ' ' };

			if (str.Length <= chunkSize) { return str.Split(new string[] { Environment.NewLine, "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList(); }
			StringBuilder sb = new StringBuilder();

			var paragraphs = str.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();

			foreach (var paragraph in paragraphs)
			{
				var words = paragraph.Split(splitChars);
				var currentLine = "";
				foreach (var word in words)
				{
					if (currentLine.VisibleLength() + word.VisibleLength() < chunkSize)	{
						currentLine += " " + word;
					}
					else {
						sb.AppendLine(currentLine.Trim().PadRight(chunkSize - currentLine.VisibleLength()));
						currentLine = "";
					}
				}
				sb.AppendLine(currentLine.PadRight(chunkSize - currentLine.VisibleLength()));
			}

			return sb.ToString()
				.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
		}

		public static IEnumerable<string> Split2(this string str, int chunkSize)
		{
			char[] splitChars = new char[] { ' ', '-', '\t' };
			str = str.Replace('\t', ' ');

			if (chunkSize < 1) throw new ArgumentException("Chunk size must be > 0.", "chunkSize");
			if (str.Length <= chunkSize) { return str.Split(new string[] { Environment.NewLine, "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList(); }
			int pos, next;
			StringBuilder sb = new StringBuilder();

			// Parse each line of text
			for (pos = 0; pos < str.Length; pos = next)
			{
				// Find end of line
				int eol = str.IndexOf(Environment.NewLine, pos);
				if (eol == -1)
					next = eol = str.Length;
				else
					next = eol + Environment.NewLine.Length;

				// Copy this line of text, breaking into smaller lines as needed
				if (eol > pos)
				{
					do
					{
						int len = eol - pos;
						if (len > chunkSize)
							len = BreakLine(str, pos, chunkSize);
						sb.Append(str, pos, len);
						sb.Append(Environment.NewLine);

						// Trim whitespace following break
						pos += len;
						while (pos < eol && Char.IsWhiteSpace(str[pos]))
							pos++;
					} while (eol > pos);
				}
				else sb.Append(Environment.NewLine); // Empty line
			}
			return sb.ToString()
					.Split(new string[] { Environment.NewLine, "\n" }, StringSplitOptions.RemoveEmptyEntries);
		}

		private static int BreakLine(string text, int pos, int max)
		{
			// Find last whitespace in line
			int i = max;
			while (i >= 0 && !Char.IsWhiteSpace(text[pos + i]))
				i--;

			// If no whitespace found, break at maximum length
			if (i < 0)
				return max;

			// Find start of whitespace
			while (i >= 0 && Char.IsWhiteSpace(text[pos + i]))
				i--;

			// Return length of text before whitespace
			return i + 1;
		}

		public static int VisibleLength(this string str)
		{
			//var stripedAnsi = Regex.Replace(str, @"\e\[(\d+;)*(\d+)?[ABCDHJKfmsu]", "");
			var stripedAnsi = Regex.Replace(str, @"\x1b\[(\d+;)*(\d+)?[ABCDHJKfmsu]", "");
			var stripedControl = Regex.Replace(stripedAnsi, @"[^\x20-\x7F]", "");
			return stripedControl.Length;
		}
	}
}

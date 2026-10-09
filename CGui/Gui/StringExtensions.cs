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

      char[] splitChars = { ' ' };

      if (str.Length <= chunkSize) { return str.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).ToList(); }
      StringBuilder sb = new StringBuilder();

      var paragraphs = str.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).ToList();

      foreach (var paragraph in paragraphs)
      {
        //make sure there are no wild CR's or LF's
        var tmp = paragraph
          .Replace("\r", string.Empty)
          .Replace("\n", string.Empty);

        var words = tmp.Split(splitChars);
        var currentLine = "";
        bool firstWord = true;
        foreach (var word in words)
        {
          if (currentLine.VisibleLength() + word.VisibleLength() < chunkSize)
          {
            currentLine += (firstWord ? "" : " ") + word;
          }
          else if (word.VisibleLength() > chunkSize)
          {
            currentLine += (firstWord ? "" : " ") + word.Substring(0, chunkSize - currentLine.VisibleLength()) + "...";
          }
          else
          {
            var a = currentLine.Length != currentLine.VisibleLength() ? 4 : 0;
            sb.AppendLine(currentLine.PadRight(chunkSize + (currentLine.Length - currentLine.VisibleLength() + a)));

            if (word.VisibleLength() > chunkSize && chunkSize > 3)
            {
              currentLine = word.Substring(0, chunkSize - 3) + "...";
            }
            else
            {
              currentLine = word;
            }
          }
          firstWord = false;
        }

        var x = currentLine.Length != currentLine.VisibleLength() ? 4 : 0;
        var line = currentLine.PadRight(chunkSize + (currentLine.Length - currentLine.VisibleLength() + x));
        sb.AppendLine(line);
      }

      return sb.ToString()
        .Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
    }

    public static int VisibleLength(this string str)
    {
      var stripedControl = Regex.Replace(str, @"\p{C}\[([fb]?)\:?(\w+)\]", "");
      return stripedControl.Length;
    }

    /// <summary>
    /// Shortens the text to at most <paramref name="maxVisible"/> visible characters. Colour tags do not
    /// count and are always kept, so the colours after the cut stay balanced.
    /// </summary>
    public static string TruncateVisible(this string str, int maxVisible)
    {
      if (str == null || maxVisible < 0 || str.VisibleLength() <= maxVisible) return str;

      var tag = new Regex(@"\G\p{C}\[([fb]?)\:?(\w+)\]");
      var result = new StringBuilder();
      int visible = 0;
      int i = 0;
      while (i < str.Length)
      {
        var match = tag.Match(str, i);
        if (match.Success)
        {
          result.Append(match.Value);
          i += match.Length;
          continue;
        }
        if (visible < maxVisible)
        {
          result.Append(str[i]);
          visible++;
        }
        i++;
      }
      return result.ToString();
    }

    /// <summary>
    /// Like PadLeft, but colour tags do not count towards the length.
    /// </summary>
    public static string PadLeftVisible(this string str, int totalWidth, char padChar)
    {
      return str.PadLeft(totalWidth + (str.Length - str.VisibleLength()), padChar);
    }

    /// <summary>
    /// Like PadRight, but colour tags do not count towards the length.
    /// </summary>
    public static string PadRightVisible(this string str, int totalWidth, char padChar)
    {
      return str.PadRight(totalWidth + (str.Length - str.VisibleLength()), padChar);
    }

    /// <summary>
    /// Like PadBoth, but colour tags do not count towards the length.
    /// </summary>
    public static string PadBothVisible(this string str, int totalWidth, char padChar)
    {
      return str.PadBoth(totalWidth + (str.Length - str.VisibleLength()), padChar);
    }

    public static ConsoleColor GetColor(this string color)
    {
      ConsoleColor result = new ConsoleColor();
      if (Enum.TryParse<ConsoleColor>(color, out result))
      {
        return result;
      }
      else
      {
        throw new ArgumentException("Unknow color name, see https://msdn.microsoft.com/en-us/library/system.consolecolor(v=vs.110).aspx for valid color names.");
      }
    }
  }
}

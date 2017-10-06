﻿using System;
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
  public class ConsoleWrapper
  {
    /// <summary>
    /// Use to lock console operations
    /// </summary>
    internal object Lock = new object();

    #region Singleton implementation
    private static readonly ConsoleWrapper instance;

    internal static string ReadLineWithCancel()
    {
      string result = null;
      int length = 0;

      StringBuilder buffer = new StringBuilder();
      //The key is read passing true for the intercept argument to prevent
      //any characters from displaying when the Escape key is pressed.
      CursorVisible = true;
      ConsoleKeyInfo info = Console.ReadKey(true);
      while (info.Key != ConsoleKey.Enter && info.Key != ConsoleKey.Escape)
      {
        if (info.Key == ConsoleKey.Backspace)
        {
          if (length > 0)
          {
            Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
            Console.Write(" ");
            Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
            buffer.Remove(buffer.Length - 1, 1);
            length--;
          }
        }
        else if(
          Char.IsLetterOrDigit(info.KeyChar) ||
          Char.IsPunctuation(info.KeyChar) ||
          Char.IsSymbol(info.KeyChar) ||
          Char.IsWhiteSpace(info.KeyChar)
          )
        {
          Console.Write(info.KeyChar.ToString());
          buffer.Append(info.KeyChar);
          length++;
        }
        info = Console.ReadKey(true);
      }
      CursorVisible = false;
      if (info.Key == ConsoleKey.Enter)
      {
        result = buffer.ToString();
      }

      return result;
    }

    // Explicit static constructor to tell C# compiler
    // not to mark type as beforefieldinit
    static ConsoleWrapper()
    {
      instance = new ConsoleWrapper();
    }

    private ConsoleWrapper()
    {
      CursorVisible = false;
      Console.OutputEncoding = Encoding.UTF8;
    }

    public static ConsoleWrapper Instance
    {
      get
      {
        return instance;
      }
    }

    internal static void Clear()
    {
      Console.Clear();
    }
    #endregion

    #region Console methods
    public static ConsoleColor ForegroundColor
    {
      get { return Console.ForegroundColor; }
      set { Console.ForegroundColor = value; }
    }
    public static ConsoleColor BackgroundColor
    {
      get { return Console.BackgroundColor; }
      set { Console.BackgroundColor = value; }
    }

    public static int CursorTop {
      get { return Console.CursorTop; }
      set { Console.CursorTop = value; }
    }
    public static int WindowWidth
    {
      get { return Console.WindowWidth; }
      set { Console.WindowWidth = value; }
    }
    public static int WindowHeight
    {
      get { return Console.WindowHeight; }
      set { Console.WindowHeight = value; }
    }

    public static int LargestWindowWidth
    {
      get { return Console.LargestWindowWidth; }
    }
    public static int LargestWindowHeight
    {
      get { return Console.LargestWindowHeight; }
    }
    public static bool CursorVisible
    {
      get { return Console.CursorVisible; }
      set { Console.CursorVisible = value; }
    }

    internal static void SetCursorPosition(int left, int top)
    {
      Console.SetCursorPosition(left, top);
    }

    internal static void Write(string value)
    {
      Console.Write(value);
    }

    internal string ReadLine()
    {
      Console.CursorVisible = true;
      var res = Console.ReadLine();
      Console.CursorVisible = false;
      return res;
    }

    internal static void SetWindowSize(int width, int height)
    {
      Console.SetWindowSize(width, height);
    }
    #endregion

    static string regexExcape = @"\p{C}\[([fb]?)\:?(\w+)\]";
    static Regex regex = new Regex(regexExcape);
    private static ConsoleColor previousForeground;
    private static ConsoleColor previousBackground;

    public static void WriteLine(string value)
    {
      SaveColor();
      var chunks = Regex.Split(value, regexExcape, RegexOptions.ExplicitCapture);
      var matches = regex.Matches(value);

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

    internal static ConsoleKeyInfo ReadKey(bool intercept)
    {
      return Console.ReadKey(intercept);
    }
  }
}

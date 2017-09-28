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
    public object Lock = new object();

    #region Singleton implementation
    private static readonly ConsoleWrapper instance = new ConsoleWrapper();

    // Explicit static constructor to tell C# compiler
    // not to mark type as beforefieldinit
    static ConsoleWrapper()
    {
    }

    private ConsoleWrapper()
    {
      CursorVisible = false;
    }

    public static ConsoleWrapper Instance
    {
      get
      {
        return instance;
      }
    }

    internal void Clear()
    {
      Console.Clear();
    }
    #endregion

    #region Console methods
    public ConsoleColor ForegroundColor
    {
      get { return Console.ForegroundColor; }
      set { Console.ForegroundColor = value; }
    }
    public ConsoleColor BackgroundColor
    {
      get { return Console.BackgroundColor; }
      set { Console.BackgroundColor = value; }
    }

    public int CursorTop { get; internal set; }
    public int WindowWidth {
      get { return Console.WindowWidth; }
      set { Console.WindowWidth = value; }
    }
    public int WindowHeight
    {
      get { return Console.WindowHeight; }
      set { Console.WindowHeight = value; }
    }

    public int LargestWindowWidth {
      get { return Console.LargestWindowWidth; }
    }
    public int LargestWindowHeight {
      get { return Console.LargestWindowHeight; }
    }
    public bool CursorVisible {
      get { return Console.CursorVisible; }
      set { Console.CursorVisible = value; }
    }

    internal void SetCursorPosition(int left, int top)
    {
      Console.SetCursorPosition(left, top);
    }

    internal void Write(string value)
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

    internal void SetWindowSize(int width, int height)
    {
      Console.SetWindowSize(width, height);
    }
    #endregion

    static string regexExcape = @"\p{C}\[([fb]?)\:?(\w+)\]";
    static Regex regex = new Regex(regexExcape);
    private static ConsoleColor previousForeground;
    private static ConsoleColor previousBackground;

    public void WriteLine(string s)
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

    internal ConsoleKeyInfo ReadKey(bool intercept)
    {
      return Console.ReadKey(intercept);
    }
  }
}

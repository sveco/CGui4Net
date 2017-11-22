namespace CGui.Gui
{
  using System;
  using System.Text;
  using System.Text.RegularExpressions;

///<summary>
///Class that converts color tags to actual console color when rendering line.
/// </summary>
  public class ConsoleWrapper {
    public static readonly string ColorReset = "\x1b[Reset]";

    /// <summary>
    /// Use to lock console operations
    /// </summary>
    internal object Lock = new object();

    private static readonly ConsoleWrapper instance;

    /// <summary>
    /// The ReadLineWithCancel
    /// </summary>
    /// <returns>The <see cref="string"/></returns>
    internal string ReadLineWithCancel()
    {
      string result = null;
      int length = 0;

      StringBuilder buffer = new StringBuilder();
      //The key is read passing true for the intercept argument to prevent
      //any characters from displaying when the Escape key is pressed.
      CursorVisible = true;
      ConsoleKeyInfo info = Console.ReadKey(true);
      while (info.Key != ConsoleKey.Enter
             && info.Key != ConsoleKey.Escape) {
        if (info.Key == ConsoleKey.Backspace) {
          if (length > 0) {
            Console.SetCursorPosition(Console.CursorLeft - 1,
                                      Console.CursorTop);
            Console.Write(" ");
            Console.SetCursorPosition(Console.CursorLeft - 1,
                                      Console.CursorTop);
            buffer.Remove(buffer.Length - 1, 1);
            length--;
          }
        } else
          if (
            Char.IsLetterOrDigit(info.KeyChar) ||
            Char.IsPunctuation(info.KeyChar) ||
            Char.IsSymbol(info.KeyChar) ||
            Char.IsWhiteSpace(info.KeyChar)
          ) {
            Console.Write(info.KeyChar.ToString());
            buffer.Append(info.KeyChar);
            length++;
          }
        info = Console.ReadKey(true);
      }
      CursorVisible = false;
      if (info.Key == ConsoleKey.Enter) {
        result = buffer.ToString();
      }

      return result;
    }

    // Explicit static constructor to tell C# compiler
    // not to mark type as beforefieldinit
    /// <summary>
    /// Initializes static members of the <see cref="ConsoleWrapper"/> class.
    /// </summary>
    static ConsoleWrapper()
    {
      instance = new ConsoleWrapper();
    }

    /// <summary>
    /// Prevents a default instance of the <see cref="ConsoleWrapper"/> class from being created.
    /// </summary>
    private ConsoleWrapper()
    {
      CursorVisible = false;
      SaveColor();
      Console.OutputEncoding = Encoding.UTF8;
    }

    /// <summary>
    /// Gets the Instance
    /// </summary>
    public static ConsoleWrapper Instance
    {
      get {
        return instance;
      }
    }

    /// <summary>
    /// The Clear
    /// </summary>
    internal static void Clear()
    {
      Console.Clear();
    }

    /// <summary>
    /// Gets or sets the ForegroundColor
    /// </summary>
    public ConsoleColor ForegroundColor
    {
      get { return Console.ForegroundColor; }
      set { Console.ForegroundColor = value; }
    }

    /// <summary>
    /// Gets or sets the BackgroundColor
    /// </summary>
    public ConsoleColor BackgroundColor
    {
      get { return Console.BackgroundColor; }
      set { Console.BackgroundColor = value; }
    }

    /// <summary>
    /// Gets or sets the CursorTop
    /// </summary>
    public int CursorTop
    {
      get { return Console.CursorTop; }
      set { Console.CursorTop = value; }
    }

    /// <summary>
    /// Gets or sets the WindowWidth
    /// </summary>
    public int WindowWidth
    {
      get { return Console.WindowWidth; }
      set { Console.WindowWidth = value; }
    }

    /// <summary>
    /// Gets or sets the WindowHeight
    /// </summary>
    public int WindowHeight
    {
      get { return Console.WindowHeight; }
      set { Console.WindowHeight = value; }
    }

    /// <summary>
    /// Gets or sets the BufferWidth
    /// </summary>
    public int BufferWidth
    {
      get { return Console.BufferWidth; }
      set { Console.BufferWidth = value; }
    }

    /// <summary>
    /// Gets or sets the BufferHeight
    /// </summary>
    public int BufferHeight
    {
      get { return Console.BufferHeight; }
      set { Console.BufferHeight = value; }
    }

    /// <summary>
    /// Gets the LargestWindowWidth
    /// </summary>
    public int LargestWindowWidth
    {
      get { return Console.LargestWindowWidth; }
    }

    /// <summary>
    /// Gets the LargestWindowHeight
    /// </summary>
    public int LargestWindowHeight
    {
      get { return Console.LargestWindowHeight; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether CursorVisible
    /// </summary>
    public bool CursorVisible
    {
      get { return Console.CursorVisible; }
      set { Console.CursorVisible = value; }
    }

    /// <summary>
    /// The SetCursorPosition
    /// </summary>
    /// <param name="left">The <see cref="int"/></param>
    /// <param name="top">The <see cref="int"/></param>
    internal void SetCursorPosition(int left, int top)
    {
      Console.SetCursorPosition(left, top);
    }

    /// <summary>
    /// The ReadLine
    /// </summary>
    /// <returns>The <see cref="string"/></returns>
    internal string ReadLine()
    {
      Console.CursorVisible = true;
      var res = Console.ReadLine();
      Console.CursorVisible = false;
      return res;
    }

    /// <summary>
    /// The SetWindowSize
    /// </summary>
    /// <param name="width">The <see cref="int"/></param>
    /// <param name="height">The <see cref="int"/></param>
    internal void SetWindowSize(int width, int height)
    {
      Console.SetWindowSize(width, height);
    }

    internal static string regexExcape = @"\p{C}\[([fb]?)\:?(\w+)\]";

    internal static Regex regex = new Regex(regexExcape);

    private ConsoleColor previousForeground;

    private ConsoleColor previousBackground;

    /// <summary>
    /// The Write
    /// </summary>
    /// <param name="value">The <see cref="string"/></param>
    internal void Write(string value)
    {
      var chunks = Regex.Split(value, regexExcape,
                               RegexOptions.ExplicitCapture);
      var matches = regex.Matches(value);

      for (int i = 0; i < chunks.Length; i++) {
        if (i > 0) {
          var match = matches[i - 1];
          var cap0 = match.Groups[0];
          var cap1 = match.Groups[1];
          var cap2 = match.Groups[2];

          if (cap2.Value == "Reset") {
            RestoreColor();
          } else
            if (cap0 != null && cap1 != null
                && cap2 != null) {
              switch (cap1.Value) {
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
    }

    /// <summary>
    /// The WriteLine
    /// </summary>
    /// <param name="value">The <see cref="string"/></param>
    public void WriteLine(string value)
    {
      Write(value);
      Console.Write(Environment.NewLine);
    }

    /// <summary>
    /// The SaveColor
    /// </summary>
    public void SaveColor()
    {
      previousForeground = Console.ForegroundColor;
      previousBackground = Console.BackgroundColor;
    }

    /// <summary>
    /// The RestoreColor
    /// </summary>
    public void RestoreColor()
    {
      Console.ForegroundColor = previousForeground;
      Console.BackgroundColor = previousBackground;
    }

    /// <summary>
    /// The ReadKey
    /// </summary>
    /// <param name="intercept">The <see cref="bool"/></param>
    /// <returns>The <see cref="ConsoleKeyInfo"/></returns>
    internal ConsoleKeyInfo ReadKey(bool intercept)
    {
      return Console.ReadKey(intercept);
    }
  }
}

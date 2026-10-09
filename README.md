# CGui4Net

Library with controls to create application with console-based user interface.

Console Gui for .net

**PM> Install-Package Cgui -Version 1.3.0**

Curently supported controls:
- Picklist - Shows a list of options and allows user to pick one
- TextArea - Renders scrollable text
- Header - Row rendered on top
- Footer - Row rendered on bottom
- Input - Allows user to input text
- Viewport - groups UI elements and renders them asynchronously

**Usage**

This library was created to provide simple text based UI for console applications. I use it it my project [cfeed - console feed reader](https://github.com/sveco/CRR "cfeed on GitHub").

***Picklist***

Picklist enables you to show scrollable list of items, and handle keyboard keys, wheter on selected item or globally. Example usage:

```C#
var l = new CGui.Gui.Picklist<string>(list, null)
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
```

Here "list" variable is just list of strings, that are displayed in menu. List_OnItemKeyHandler is defined as:

```C#
private static bool List_OnItemKeyHandler
(ConsoleKeyInfo key, ListItem<string> selectedItem, Picklist<string> parent)
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
```

***Resizing the console***

Controls can be sized relative to the console: a negative Width or Height is the size of the console minus that number, so `Width = -3` is the console width minus 3. Such a control follows the console when the window is resized.

```C#
var l = new CGui.Gui.Picklist<string>(list, null)
{
    Top = 1,
    Left = 2,
    Width = -3,   // console width - 3
    Height = -3,  // console height - 3
    ShowScrollBar = true
};
```

A viewport that is on screen draws all its controls again when the console is resized, once the new size has settled. The lists keep the selected item visible, the text areas wrap their text for the new width, and a header or footer that is longer than the window is shortened. This happens while a control waits for a key, so it needs no extra code. Set `Viewport.RefreshOnResize` to false to turn it off. The events `Viewport.Resizing` (before the redraw) and `Viewport.Resized` (after it) can be used to adjust elements that are not controls of the viewport. `ConsoleWrapper.Instance.Resized` is raised for every settled resize.

A list or text area that waits for keys but is not a control of the viewport, like a dialog, is not redrawn on top of it. The resize is applied when it closes.

A `Viewport` does not change the size of the console window when it is created. Set `Viewport.Width` and `Viewport.Height` to resize the window, for example once at startup. To try resizing, run `CGuiDemo --resize list` or `CGuiDemo --resize text`.

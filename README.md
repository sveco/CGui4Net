# CGui4Net

Library with controls to create application with console-based user interface.

Console Gui for .net

**PM> Install-Package Cgui -Version 1.0.0.2**

Curently supported controls:
- Picklist
- TextArea
- Header
- Footer
- Input

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

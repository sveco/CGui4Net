# End to end check that CGui redraws when its console is resized, with the resize mode of CGuiDemo.
# Starts the demo in a hidden console, resizes that console with the Windows console API, and reads the
# screen back after every size. Windows only. The demo is copied, nothing in the repository is touched.
#
# Usage: powershell -File scripts\resize-check.ps1 [-DemoDir CGuiDemo\bin\Debug] [-Dump]
# Exit code 0 when every check passed.
param(
    [string]$DemoDir = (Join-Path (Split-Path $PSScriptRoot -Parent) 'CGuiDemo\bin\Debug'),
    [int]$SettleMs = 1000,
    [switch]$Dump   # print the screen after every size
)
$ErrorActionPreference = 'Stop'

Add-Type -TypeDefinition @'
using System;
using System.Runtime.InteropServices;
using System.Text;

public static class ConsoleProbe
{
    [StructLayout(LayoutKind.Sequential)] public struct COORD { public short X; public short Y; public COORD(short x, short y) { X = x; Y = y; } }
    [StructLayout(LayoutKind.Sequential)] public struct SMALL_RECT { public short Left, Top, Right, Bottom; }
    [StructLayout(LayoutKind.Sequential)] public struct CSBI { public COORD dwSize; public COORD dwCursorPosition; public ushort wAttributes; public SMALL_RECT srWindow; public COORD dwMaximumWindowSize; }
    [StructLayout(LayoutKind.Explicit, Size = 20)] public struct INPUT_RECORD
    {
        [FieldOffset(0)] public ushort EventType;
        [FieldOffset(4)] public int bKeyDown;
        [FieldOffset(8)] public ushort wRepeatCount;
        [FieldOffset(10)] public ushort wVirtualKeyCode;
        [FieldOffset(12)] public ushort wVirtualScanCode;
        [FieldOffset(14)] public ushort UnicodeChar;
        [FieldOffset(16)] public uint dwControlKeyState;
    }

    [DllImport("kernel32.dll", SetLastError = true)] static extern bool FreeConsole();
    [DllImport("kernel32.dll", SetLastError = true)] static extern bool AttachConsole(uint pid);
    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)] static extern IntPtr CreateFile(string name, uint access, uint share, IntPtr sec, uint disp, uint flags, IntPtr template);
    [DllImport("kernel32.dll", SetLastError = true)] static extern bool GetConsoleScreenBufferInfo(IntPtr h, out CSBI info);
    [DllImport("kernel32.dll", SetLastError = true)] static extern bool SetConsoleScreenBufferSize(IntPtr h, COORD size);
    [DllImport("kernel32.dll", SetLastError = true)] static extern bool SetConsoleWindowInfo(IntPtr h, bool absolute, ref SMALL_RECT rect);
    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)] static extern bool ReadConsoleOutputCharacter(IntPtr h, StringBuilder sb, uint len, COORD origin, out uint read);
    [DllImport("kernel32.dll", SetLastError = true)] static extern bool ReadConsoleOutputAttribute(IntPtr h, ushort[] attrs, uint len, COORD origin, out uint read);
    [DllImport("kernel32.dll", SetLastError = true)] static extern bool WriteConsoleInput(IntPtr h, INPUT_RECORD[] buffer, uint length, out uint written);

    static IntPtr output = IntPtr.Zero;
    static IntPtr input = IntPtr.Zero;

    public static string Attach(uint pid)
    {
        FreeConsole();
        if (!AttachConsole(pid)) return "AttachConsole failed, error " + Marshal.GetLastWin32Error();
        // GENERIC_READ | GENERIC_WRITE, FILE_SHARE_READ | FILE_SHARE_WRITE, OPEN_EXISTING
        output = CreateFile("CONOUT$", 0xC0000000, 3, IntPtr.Zero, 3, 0, IntPtr.Zero);
        input = CreateFile("CONIN$", 0xC0000000, 3, IntPtr.Zero, 3, 0, IntPtr.Zero);
        if (output == new IntPtr(-1) || input == new IntPtr(-1)) return "CreateFile CONOUT$/CONIN$ failed, error " + Marshal.GetLastWin32Error();
        return null;
    }

    public static void Detach() { FreeConsole(); output = IntPtr.Zero; input = IntPtr.Zero; }

    public static string Resize(short w, short h)
    {
        CSBI info;
        if (!GetConsoleScreenBufferInfo(output, out info)) return "GetConsoleScreenBufferInfo failed, error " + Marshal.GetLastWin32Error();
        short bufferHeight = 300;
        // the buffer must never be smaller than the window: grow it first, then move the window, then trim it
        if (!SetConsoleScreenBufferSize(output, new COORD(Math.Max(w, info.dwSize.X), Math.Max(bufferHeight, info.dwSize.Y)))) return "grow buffer failed, error " + Marshal.GetLastWin32Error();
        SMALL_RECT r = new SMALL_RECT(); r.Left = 0; r.Top = 0; r.Right = (short)(w - 1); r.Bottom = (short)(h - 1);
        if (!SetConsoleWindowInfo(output, true, ref r)) return "SetConsoleWindowInfo failed, error " + Marshal.GetLastWin32Error();
        if (!SetConsoleScreenBufferSize(output, new COORD(w, bufferHeight))) return "trim buffer failed, error " + Marshal.GetLastWin32Error();
        return null;
    }

    public static string Size()
    {
        CSBI info;
        if (!GetConsoleScreenBufferInfo(output, out info)) return "0x0";
        return (info.srWindow.Right - info.srWindow.Left + 1) + "x" + (info.srWindow.Bottom - info.srWindow.Top + 1) + "@" + info.srWindow.Top;
    }

    public static string[] ReadWindow()
    {
        CSBI info;
        GetConsoleScreenBufferInfo(output, out info);
        int w = info.srWindow.Right - info.srWindow.Left + 1;
        int h = info.srWindow.Bottom - info.srWindow.Top + 1;
        string[] rows = new string[h];
        for (int i = 0; i < h; i++)
        {
            // the API does not null terminate, so cut the text at the number of characters it reports
            StringBuilder sb = new StringBuilder(w + 1);
            uint read;
            ReadConsoleOutputCharacter(output, sb, (uint)w, new COORD(info.srWindow.Left, (short)(info.srWindow.Top + i)), out read);
            string text = sb.ToString();
            rows[i] = text.Substring(0, (int)Math.Min(read, (uint)text.Length));
        }
        return rows;
    }

    /// <summary>Background colour (0-15) of the first cell of every row of the window.</summary>
    public static int[] ReadBackgrounds()
    {
        CSBI info;
        GetConsoleScreenBufferInfo(output, out info);
        int h = info.srWindow.Bottom - info.srWindow.Top + 1;
        int[] result = new int[h];
        for (int i = 0; i < h; i++)
        {
            ushort[] attrs = new ushort[4];
            uint read;
            // column 2 is where the list starts, the first columns are not part of it
            ReadConsoleOutputAttribute(output, attrs, 1, new COORD((short)(info.srWindow.Left + 2), (short)(info.srWindow.Top + i)), out read);
            result[i] = (attrs[0] >> 4) & 0xF;
        }
        return result;
    }

    public static void SendKey(ushort virtualKey, char ch)
    {
        INPUT_RECORD[] records = new INPUT_RECORD[2];
        for (int i = 0; i < 2; i++)
        {
            records[i].EventType = 1; // KEY_EVENT
            records[i].bKeyDown = i == 0 ? 1 : 0;
            records[i].wRepeatCount = 1;
            records[i].wVirtualKeyCode = virtualKey;
            records[i].wVirtualScanCode = 0;
            records[i].UnicodeChar = ch;
        }
        uint written;
        WriteConsoleInput(input, records, 2, out written);
    }
}
'@

if (-not (Test-Path (Join-Path $DemoDir 'CGuiDemo.exe'))) { throw "CGuiDemo.exe not found in $DemoDir, build the solution first" }

$report = New-Object System.Collections.ArrayList
$failures = 0
function Add-Line($text) { [void]$script:report.Add($text) }
function Check($name, $ok, $detail) {
    if (-not $ok) { $script:failures++ }
    Add-Line ('  {0,-4} {1}{2}' -f $(if ($ok) { 'PASS' } else { 'FAIL' }), $name, $(if ($detail) { '  (' + $detail + ')' } else { '' }))
}
$VK = @{ Escape = 0x1B; PageDown = 0x22; Down = 0x28 }
function Press($vk, $times = 1) { for ($i = 0; $i -lt $times; $i++) { [ConsoleProbe]::SendKey([uint16]$vk, [char]0); Start-Sleep -Milliseconds 40 } }
function Set-Size($w, $h) {
    $err = [ConsoleProbe]::Resize([int16]$w, [int16]$h)
    if ($err) { throw $err }
    Start-Sleep -Milliseconds $SettleMs
}

function Run-Scenario($mode, $sizes) {
    Add-Line "== $mode"
    $work = Join-Path $env:TEMP ('cgui-resize-' + [guid]::NewGuid().ToString('N'))
    New-Item -ItemType Directory $work | Out-Null
    Get-ChildItem $DemoDir -File | Where-Object { $_.Extension -in '.exe', '.dll', '.config' } | Copy-Item -Destination $work
    $proc = Start-Process -FilePath (Join-Path $work 'CGuiDemo.exe') -ArgumentList @('--resize', $mode) -WorkingDirectory $work -WindowStyle Hidden -PassThru
    try {
        Start-Sleep -Milliseconds 2000
        if ($proc.HasExited) { throw "CGuiDemo.exe exited early with code $($proc.ExitCode)" }
        $err = [ConsoleProbe]::Attach([uint32]$proc.Id)
        if ($err) { throw $err }

        Set-Size 100 30
        if ($mode -eq 'list') { Press $VK.Down 40 }          # select Item 41
        else { Press $VK.PageDown 3 }                        # scroll the text

        foreach ($s in $sizes) {
            $w = $s[0]; $h = $s[1]
            Set-Size $w $h
            $rows = [ConsoleProbe]::ReadWindow()
            Add-Line ("{0}x{1} (console reports {2})" -f $w, $h, [ConsoleProbe]::Size())
            if ($Dump) { for ($i = 0; $i -lt $rows.Count; $i++) { Add-Line ('    {0,2}|{1}' -f $i, $rows[$i].TrimEnd()) } }
            $first = $rows[0].TrimEnd(); $last = $rows[$rows.Count - 1].TrimEnd()
            $headerRows = @($rows | Where-Object { $_.Contains('CGui resize demo') }).Count
            $footerRows = @($rows | Where-Object { $_.Contains('Esc:Quit') }).Count
            Check 'header on the first row' ($first.Contains('CGui resize demo')) $first.Trim()
            Check 'header spans the new width' ($first.Length -ge $w - 3) ("{0} of {1} columns" -f $first.Length, $w)
            Check 'footer on the last row' ($last.Contains('Esc:Quit')) $last.Trim()
            Check 'footer spans the new width' ($last.Length -ge $w - 3) ("{0} of {1} columns" -f $last.Length, $w)
            Check 'no stale header or footer left behind' ($headerRows -eq 1 -and $footerRows -eq 1) ("$headerRows header row(s), $footerRows footer row(s)")

            $body = $rows[1..($rows.Count - 2)]
            if ($mode -eq 'list') {
                $items = @($body | Where-Object { $_ -match 'Item \d+' }).Count
                $expected = [Math]::Min(60, $h - 3)
                Check 'list fills the new height' ($items -eq $expected) ("$items item rows, expected $expected")
                $bg = [ConsoleProbe]::ReadBackgrounds()
                # ConsoleColor.Magenta is 13
                $selected = @(0..($rows.Count - 1) | Where-Object { $bg[$_] -eq 13 -and $rows[$_] -match 'Item 41\b' }).Count
                Check 'selected item (Item 41) is visible and highlighted' ($selected -eq 1) ("$selected highlighted row(s) with Item 41")
            }
            else {
                $scrollbar = [char[]]@(0x25B2, 0x25BC, 0x2588, 0x2592)
                $wide = @($body | Where-Object { $t = $_.TrimEnd(); $t.Length -ge $w - 1 -and $scrollbar -notcontains $t[$t.Length - 1] }).Count
                Check 'text wrapped for the new width' ($wide -eq 0) ("$wide row(s) reach the window edge")
                $textRows = @($body | Where-Object { $_.Trim().Length -gt 0 }).Count
                Check 'text fills the new height' ($textRows -ge $h - 4) ("$textRows rows with text, expected at least " + ($h - 4))
            }
        }

        Press $VK.Escape
        Start-Sleep -Milliseconds 1500
        Check 'quits with Esc' ($proc.HasExited) $null
    }
    catch {
        $script:failures++
        Add-Line ("ERROR: " + $_.Exception.Message)
    }
    finally {
        [ConsoleProbe]::Detach()
        if (-not $proc.HasExited) { Stop-Process -Id $proc.Id -Force }
        Start-Sleep -Milliseconds 300
        Remove-Item $work -Recurse -Force -ErrorAction SilentlyContinue
    }
}

$sizes = @(@(100, 30), @(70, 12), @(140, 45), @(50, 15), @(20, 6), @(100, 30), @(120, 40))
Run-Scenario 'list' $sizes
Run-Scenario 'text' $sizes

$report | ForEach-Object { $_ }
if ($failures -eq 0) { 'ALL CHECKS PASSED' } else { "$failures CHECK(S) FAILED" }
exit $(if ($failures -eq 0) { 0 } else { 1 })

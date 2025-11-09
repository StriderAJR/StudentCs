using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace StrategyGame.ConsoleGame.UI.CustomConsole;

internal static class ConsoleFontHelper
{
    private const int STD_OUTPUT_HANDLE = -11;

    [StructLayout(LayoutKind.Sequential)]
    private struct COORD
    {
        public short X;
        public short Y;
        public COORD(short x, short y) { X = x; Y = y; }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct CONSOLE_FONT_INFO_EX
    {
        public uint cbSize;
        public uint nFont;
        public COORD dwFontSize;
        public uint FontFamily;
        public uint FontWeight;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string FaceName;
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr GetStdHandle(int nStdHandle);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern bool GetCurrentConsoleFontEx(IntPtr hConsoleOutput, bool bMaximumWindow, ref CONSOLE_FONT_INFO_EX lpConsoleCurrentFontEx);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern bool SetCurrentConsoleFontEx(IntPtr hConsoleOutput, bool bMaximumWindow, ref CONSOLE_FONT_INFO_EX lpConsoleCurrentFontEx);

    public static bool TrySetFont(string faceName, short fontSizeY = 16)
    {
        try
        {
            IntPtr h = GetStdHandle(STD_OUTPUT_HANDLE);
            var info = new CONSOLE_FONT_INFO_EX();
            info.cbSize = (uint)Marshal.SizeOf<CONSOLE_FONT_INFO_EX>();

            // Try to read current settings; if it fails still attempt to set
            try { GetCurrentConsoleFontEx(h, false, ref info); } catch { }

            info.FaceName = faceName;
            info.dwFontSize = new COORD(0, fontSizeY);
            // Common values used by examples: FontFamily = 54 (FF_DONTCARE | TMPF_TRUETYPE), Weight = 400 (normal)
            info.FontFamily = 54;
            info.FontWeight = 400;

            return SetCurrentConsoleFontEx(h, false, ref info);
        }
        catch
        {
            return false;
        }
    }

    public static bool TrySetFontList(IEnumerable<string> faceNames, out string chosen)
    {
        chosen = null;
        foreach (var f in faceNames)
        {
            if (TrySetFont(f))
            {
                chosen = f;
                return true;
            }
        }
        return false;
    }
}

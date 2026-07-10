using System.Runtime.InteropServices;

namespace StrategyGame.ConsoleGame.UI.ConsoleUtils;

internal static class NativeWin
{
    [DllImport("kernel32.dll", SetLastError = true)]
    internal static extern IntPtr GetConsoleWindow();

    [DllImport("user32.dll", SetLastError = true)]
    internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [DllImport("kernel32.dll", SetLastError = true)]
    internal static extern bool SetConsoleOutputCP(uint wCodePageID);

    public const int SW_MAXIMIZE = 3;
    public const int SW_HIDE = 0;
    public const int SW_SHOW = 5;

    public static void Maximize()
    {
        var h = GetConsoleWindow();
        if (h != IntPtr.Zero)
            ShowWindow(h, SW_MAXIMIZE);
    }
}

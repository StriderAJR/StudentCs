using StrategyGame.ConsoleGame.Game.MapTypes;
using StrategyGame.ConsoleGame.UI.CustomConsole;
using System.Diagnostics;

namespace StrategyGame.ConsoleGame.UI.ConsoleUtils;

internal static class ConsoleUtils
{
    private static string QuoteArgument(string a)
    {
        if (string.IsNullOrEmpty(a)) return "\"\"";
        if (a.Contains(' ') || a.Contains('"'))
            return "\"" + a.Replace("\"", "\\\"") + "\"";
        return a;
    }

    public static void SetupConsoleWindow()
    {
        // explicitly request UTF-8 output
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        // On Windows additionally set the console output code page to UTF-8 (65001)
        if (OperatingSystem.IsWindows())
        {
            try
            {
                NativeWin.SetConsoleOutputCP(65001);
            }
            catch
            {
                // ignore
            }
        }

        // Попытаться развернуть окно консоли на весь экран (Windows)
        if (OperatingSystem.IsWindows())
        {
            try
            {
                NativeWin.Maximize();
                // дать ОС немного времени, чтобы изменить размер окна
                Thread.Sleep(80);
            }
            catch
            {
                // игнорировать ошибки
            }

            try
            {
                // попытаться установить максимальные размеры буфера/окна
                int w = Console.LargestWindowWidth;
                int h = Console.LargestWindowHeight;
                Console.SetBufferSize(w, h);
                Console.SetWindowSize(Math.Min(Console.WindowWidth, w), Math.Min(Console.WindowHeight, h));
            }
            catch
            {
                // некоторые консоли (IDE) не позволяют менять размер
            }
        }

        // ограничить размер буфера, чтобы не появлялись полосы прокрутки
        try
        {
            Console.BufferHeight = Console.WindowHeight;
            Console.BufferWidth = Console.WindowWidth;
        }
        catch
        {
            // игнорировать, если не поддерживается
        }

        // Detect if current font supports Unicode box shapes; if not, fall back to monospace glyphs
        if (OperatingSystem.IsWindows())
        {
            try
            {
                var face = ConsoleFontHelper.GetCurrentFontFaceName();
                if (!string.IsNullOrEmpty(face))
                {
                    // Common Unicode-capable fonts: "Consolas", "Lucida Console", "Cascadia Code", "DejaVu Sans Mono"
                    string[] unicodeFonts = new[] { "Consolas", "Lucida Console", "Cascadia Code", "DejaVu Sans Mono", "Courier New" };
                    bool ok = unicodeFonts.Any(f => face.IndexOf(f, StringComparison.OrdinalIgnoreCase) >= 0);
                    if (!ok)
                    {
                        MapSymbols.Settings.UseMonospace = true;
                    }
                }
            }
            catch { }
        }

        // Гарантировать, что внутренний буфер GameConsole соответствует реальному размеру консоли
        GameConsole.Buffer.Init(Console.WindowWidth, Console.WindowHeight);
        // отключаем отображение курсора через общий буфер
        GameConsole.Buffer.CursorVisible = false;
    }

    public static bool TryRelaunchInWindowsTerminal(string[] args)
    {
        if (MapSymbols.Settings.UseMonospace)
            return false;

        try
        {
            if (!OperatingSystem.IsWindows())
                return false;

            // If already running inside Windows Terminal, do nothing
            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WT_SESSION")))
                return false;

            // Locate current executable
            string exe = Environment.ProcessPath ?? Process.GetCurrentProcess().MainModule?.FileName ?? AppContext.BaseDirectory;

            // Build arguments string
            string argString = string.Join(" ", args.Select(QuoteArgument));

            // Compose PowerShell command to run the executable (keeps the terminal open)
            string psCommand = $"powershell -NoExit -Command \"& '{exe}' {argString}\"";

            var psi = new ProcessStartInfo("wt.exe", psCommand)
            {
                UseShellExecute = true,
                WorkingDirectory = Environment.CurrentDirectory,
                // ask the shell to start the new window maximized (best-effort)
                WindowStyle = ProcessWindowStyle.Maximized
            };

            IntPtr console = IntPtr.Zero;
            try
            {
                console = NativeWin.GetConsoleWindow();
                if (console != IntPtr.Zero)
                {
                    // hide original console while attempting to start Windows Terminal
                    NativeWin.ShowWindow(console, NativeWin.SW_HIDE);
                }

                var proc = Process.Start(psi);
                if (proc == null)
                {
                    // failed to start Windows Terminal - unhide and fall back
                    if (console != IntPtr.Zero)
                        NativeWin.ShowWindow(console, NativeWin.SW_SHOW);

                    MapSymbols.Settings.UseMonospace = true;
                    return false;
                }

                // Best-effort: wait briefly for WT to create a window and maximize it.
                // Note: WT may spawn child processes; this is a best-effort attempt.
                try
                {
                    for (int i = 0; i < 40; i++)
                    {
                        proc.Refresh();
                        if (proc.MainWindowHandle != IntPtr.Zero)
                        {
                            try { NativeWin.ShowWindow(proc.MainWindowHandle, NativeWin.SW_MAXIMIZE); } catch { }
                            break;
                        }
                        Thread.Sleep(50);
                    }
                }
                catch { }

                // successfully started WT - exit current process (console remains hidden)
                return true;
            }
            catch
            {
                try
                {
                    if (console != IntPtr.Zero)
                        NativeWin.ShowWindow(console, NativeWin.SW_SHOW);
                }
                catch { }

                // fallback to monospace symbols when relaunch fails
                try
                {
                    MapSymbols.Settings.UseMonospace = true;
                }
                catch { }
                return false;
            }
        }
        catch
        {
            return false;
        }
    }
}
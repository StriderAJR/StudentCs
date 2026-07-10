using StrategyGame.ConsoleGame.UI.CustomConsole;

namespace StrategyGame.ConsoleGame.UI.Windows;

public static class SplashScreen
{
    // Show ASCII art splash. Plays a short melody via Console.Beep when possible.
    public static void Show(int durationMs = 3000, bool playMusic = true)
    {
        string[] art = new[]
        {
            @"  _____ _                _             _____                      ",
            @" / ____| |              | |           / ____|                     ",
            @"| (___ | |__   __ _ _ __| | _____    | (___   ___  _ __ ___  _ __ ",
            @" \___ \| '_ \\ / _` | '__| |/ / __|    \___ \ / _ \| '_ ` _ \\| '_ \",
            @"  ____) | | | | (_| | |  |   <\__ \    ____) | (_) | | | | | | |_) |",
            @" |_____/|_| |_|\__,_|_|  |_|\_\\___/   |_____/ \___/|_| |_| |_| .__/ ",
            @"                                                               | |    ",
            @"                                                               |_|    "
        };

        var originalFg = GameConsole.ForegroundColor;
        var originalBg = GameConsole.BackgroundColor;

        try
        {
            GameConsole.Clear();
            GameConsole.ForegroundColor = ConsoleColor.Cyan;

            int w = GameConsole.WindowWidth;
            int h = GameConsole.WindowHeight;

            int artWidth = 0;
            foreach (var l in art) if (l.Length > artWidth) artWidth = l.Length;

            int startX = Math.Max(0, (w - artWidth) / 2);
            int startY = Math.Max(0, (h - art.Length) / 2 - 1);

            for (int i = 0; i < art.Length; i++)
            {
                GameConsole.SetCursorPosition(startX, startY + i);
                GameConsole.Write(art[i]);
            }

            string subtitle = "A tiny console strategy game";
            GameConsole.SetCursorPosition(Math.Max(0, (w - subtitle.Length) / 2), startY + art.Length + 1);
            GameConsole.Write(subtitle);

            string hint = "Press any key to continue...";
            GameConsole.SetCursorPosition(Math.Max(0, (w - hint.Length) / 2), Math.Min(h - 1, startY + art.Length + 3));
            GameConsole.Write(hint);

            GameConsole.Flush();

            CancellationTokenSource cts = new CancellationTokenSource();

            Task? musicTask = null;
            if (playMusic)
            {
                musicTask = Task.Run(() => PlaySimpleMelody(cts.Token));
            }

            // Wait for key or timeout
            var sw = System.Diagnostics.Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < durationMs)
            {
                if (Console.KeyAvailable)
                {
                    Console.ReadKey(true);
                    break;
                }
                Thread.Sleep(50);
            }

            cts.Cancel();
            musicTask?.Wait(1000);
        }
        catch
        {
            // ignore
        }
        finally
        {
            GameConsole.ForegroundColor = originalFg;
            GameConsole.BackgroundColor = originalBg;
            GameConsole.Clear();
            GameConsole.Flush();
        }
    }

    private static void PlaySimpleMelody(CancellationToken token)
    {
        // Very small melody (frequencies in Hz, durations in ms)
        int[] freqs = new[] { 659, 659, 0, 659, 0, 523, 659, 784 }; // short tune
        int[] durs = new[] { 150, 150, 100, 150, 100, 150, 150, 300 };

        try
        {
            for (int i = 0; i < freqs.Length; i++)
            {
                if (token.IsCancellationRequested) break;
                int f = freqs[i];
                int d = durs[i];
                if (f > 0)
                {
                    try { Console.Beep(f, d); }
                    catch { Thread.Sleep(d); }
                }
                else
                {
                    Thread.Sleep(d);
                }
            }
        }
        catch { }
    }
}

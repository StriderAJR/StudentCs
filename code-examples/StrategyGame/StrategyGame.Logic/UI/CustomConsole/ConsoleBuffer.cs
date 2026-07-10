namespace StrategyGame.ConsoleGame.UI.CustomConsole;

/// <summary>
/// In-memory console buffer. Draw into this buffer and call Flush() to render
/// to the real console. Snapshots allow restoring previous screen state.
/// This is a simple implementation intended for the console UI in this
/// project.
/// </summary>
public sealed class ConsoleBuffer
{
    private readonly object locker = new();
    private char[,] chars;
    private ConsoleColor[,] foregroundColors;
    private ConsoleColor[,] backgroundColors;

    public int WindowWidth { get; private set; }
    public int WindowHeight { get; private set; }

    public int CursorLeft { get; private set; }
    public int CursorTop { get; private set; }

    public ConsoleColor ForegroundColor { get; set; } = ConsoleColor.Gray;
    public ConsoleColor BackgroundColor { get; set; } = ConsoleColor.Black;

    public bool CursorVisible { get; set; }

    public ConsoleBuffer()
    {
        Init(Console.WindowWidth, Console.WindowHeight);
        CursorVisible = false;
        Console.CursorVisible = CursorVisible;
    }

    public void Init(int width, int height)
    {
        lock (locker)
        {
            WindowWidth = Math.Max(1, width);
            WindowHeight = Math.Max(1, height);
            chars = new char[WindowHeight, WindowWidth];
            foregroundColors = new ConsoleColor[WindowHeight, WindowWidth];
            backgroundColors = new ConsoleColor[WindowHeight, WindowWidth];

            Clear();
        }
    }

    public void Clear()
    {
        lock (locker)
        {
            for (int y = 0; y < WindowHeight; y++)
            for (int x = 0; x < WindowWidth; x++)
            {
                chars[y, x] = ' ';
                foregroundColors[y, x] = ForegroundColor;
                backgroundColors[y, x] = BackgroundColor;
            }
        }
    }

    public void SetCursorPosition(int left, int top)
    {
        CursorLeft = Math.Clamp(left, 0, Math.Max(0, WindowWidth - 1));
        CursorTop = Math.Clamp(top, 0, Math.Max(0, WindowHeight - 1));
    }

    public void Write(string s)
    {
        if (string.IsNullOrEmpty(s))
            return;

        lock (locker)
        {
            int x = CursorLeft;
            int y = CursorTop;
            foreach (char c in s)
            {
                // ignore carriage return (\r) because Environment.NewLine on Windows is "\r\n";
                // we handle '\n' explicitly and storing '\r' would produce control characters
                // in the buffer which break the Flush output.
                if (c == '\r')
                    continue;

                if (c == '\n')
                {
                    x = 0;
                    y++;
                    if (y >= WindowHeight) break;
                    continue;
                }

                if (x >= 0 && x < WindowWidth && y >= 0 && y < WindowHeight)
                {
                    chars[y, x] = c;
                    foregroundColors[y, x] = ForegroundColor;
                    backgroundColors[y, x] = BackgroundColor;
                }

                x++;
            }

            CursorLeft = x;
            CursorTop = Math.Min(y, WindowHeight - 1);
        }
    }

    public void Write(char c) => Write(c.ToString());

    public ConsoleKeyInfo ReadKey(bool intercept = false)
    {
        // Input still goes to the real console
        return Console.ReadKey(intercept);
    }

    public ConsoleBufferSnapshot GetSnapshot()
    {
        lock (locker)
        {
            var snap = new ConsoleBufferSnapshot(WindowWidth, WindowHeight);
            for (int y = 0; y < WindowHeight; y++)
            for (int x = 0; x < WindowWidth; x++)
            {
                snap.chars[y, x] = chars[y, x];
                snap.ForegroundColors[y, x] = foregroundColors[y, x];
                snap.BackgroundColors[y, x] = backgroundColors[y, x];
            }
            snap.CursorLeft = CursorLeft;
            snap.CursorTop = CursorTop;
            snap.ForegroundColor = ForegroundColor;
            snap.BackgroundColor = BackgroundColor;
            return snap;
        }
    }

    public void RestoreSnapshot(ConsoleBufferSnapshot snap)
    {
        if (snap == null) return;
        lock (locker)
        {
            if (snap.width != WindowWidth || snap.height != WindowHeight)
            {
                Init(snap.width, snap.height);
            }

            for (int y = 0; y < WindowHeight; y++)
            for (int x = 0; x < WindowWidth; x++)
            {
                chars[y, x] = snap.chars[y, x];
                foregroundColors[y, x] = snap.ForegroundColors[y, x];
                backgroundColors[y, x] = snap.BackgroundColors[y, x];
            }

            CursorLeft = snap.CursorLeft;
            CursorTop = snap.CursorTop;
            ForegroundColor = snap.ForegroundColor;
            BackgroundColor = snap.BackgroundColor;
        }
    }

    public void Flush()
    {
        lock (locker)
        {
            Console.CursorVisible = CursorVisible;
            for (int y = 0; y < WindowHeight; y++)
            {
                Console.SetCursorPosition(0, y);

                int x = 0;
                while (x < WindowWidth)
                {
                    // write runs of same color to reduce color switches
                    var runForegroundColor = foregroundColors[y, x];
                    var runBackgroundColor = backgroundColors[y, x];
                    int runStart = x;
                    int runLength = 0;
                    while (x < WindowWidth && foregroundColors[y, x] == runForegroundColor
                            && backgroundColors[y, x] == runBackgroundColor)
                    {
                        runLength++;
                        x++;
                    }

                    Console.ForegroundColor = runForegroundColor;
                    Console.BackgroundColor = runBackgroundColor;

                    // build string for this run
                    char[] runBuffer = new char[runLength];
                    for (int i = 0; i < runLength; i++)
                        runBuffer[i] = chars[y, runStart + i];

                    Console.Write(runBuffer);
                }
            }

            Console.SetCursorPosition(CursorLeft, CursorTop);
        }
    }
}

using System;

namespace StrategyGame.ConsoleGame.UI.CustomConsole;

public static class GameConsole
{
    public static ConsoleBuffer Buffer { get; private set; }

    static GameConsole()
    {
        // initialize buffer with current console size
        Buffer = new ConsoleBuffer();
    }

    public static ConsoleKeyInfo ReadKey(bool intercept = false)
    {
        return Buffer.ReadKey(intercept);
    }

    public static int WindowWidth => Buffer.WindowWidth;
    public static int WindowHeight => Buffer.WindowHeight;

    // Forwarded properties
    public static ConsoleColor ForegroundColor
    {
        get => Buffer.ForegroundColor;
        set => Buffer.ForegroundColor = value;
    }

    public static ConsoleColor BackgroundColor
    {
        get => Buffer.BackgroundColor;
        set => Buffer.BackgroundColor = value;
    }

    public static bool CursorVisible
    {
        get => Buffer.CursorVisible;
        set => Buffer.CursorVisible = value;
    }

    public static int CursorLeft => Buffer.CursorLeft;
    public static int CursorTop => Buffer.CursorTop;

    // Forwarded methods
    public static void SetCursorPosition(int left, int top) => Buffer.SetCursorPosition(left, top);
    public static void Write(string s) => Buffer.Write(s);
    public static void Write(char c) => Buffer.Write(c);
    public static void Clear() => Buffer.Clear();
    public static void Flush() => Buffer.Flush();

    public static ConsoleBufferSnapshot GetSnapshot() => Buffer.GetSnapshot();
    public static void RestoreSnapshot(ConsoleBufferSnapshot snapshot) => Buffer.RestoreSnapshot(snapshot);
}

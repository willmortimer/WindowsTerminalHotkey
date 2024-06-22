using System;
using System.Runtime.InteropServices;
using System.Diagnostics;

class Program
{
    [DllImport("user32.dll")]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll")]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    [DllImport("user32.dll")]
    private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [DllImport("user32.dll")]
    private static extern bool SetForegroundWindow(IntPtr hWnd);

    private const int HOTKEY_ID = 1;
    private const uint MOD_ALT = 0x0001;
    private const uint VK_SPACE = 0x20;

    private const int SW_HIDE = 0;
    private const int SW_SHOW = 5;

    static void Main(string[] args)
    {
        RegisterHotKey(IntPtr.Zero, HOTKEY_ID, MOD_ALT, VK_SPACE);

        Console.WriteLine("Hotkey registered. Press Alt+Space to toggle Windows Terminal.");
        Console.WriteLine("Press any key to exit...");

        while (!Console.KeyAvailable)
        {
            if (GetMessage(out MSG msg, IntPtr.Zero, 0, 0) > 0)
            {
                if (msg.message == WM_HOTKEY)
                {
                    ToggleWindowsTerminal();
                }
            }
        }

        UnregisterHotKey(IntPtr.Zero, HOTKEY_ID);
    }

    private static void ToggleWindowsTerminal()
    {
        IntPtr hWnd = FindWindow("CASCADIA_HOSTING_WINDOW_CLASS", null);

        if (hWnd != IntPtr.Zero)
        {
            // Windows Terminal is running, toggle visibility
            if (IsWindowVisible(hWnd))
            {
                ShowWindow(hWnd, SW_HIDE);
            }
            else
            {
                ShowWindow(hWnd, SW_SHOW);
                SetForegroundWindow(hWnd);
            }
        }
        else
        {
            // Windows Terminal is not running, start it
            Process.Start("wt.exe");
        }
    }

    [DllImport("user32.dll")]
    private static extern bool IsWindowVisible(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern int GetMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

    [StructLayout(LayoutKind.Sequential)]
    public struct MSG
    {
        public IntPtr hwnd;
        public uint message;
        public IntPtr wParam;
        public IntPtr lParam;
        public uint time;
        public POINT pt;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;
    }

    private const int WM_HOTKEY = 0x0312;
}
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

public static class WindowsTerminalManager
{
    [DllImport("user32.dll")]
    private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [DllImport("user32.dll")]
    private static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern bool IsWindowVisible(IntPtr hWnd);

    private const int SW_HIDE = 0;
    private const int SW_SHOW = 5;

    public static void ToggleWindowsTerminal()
    {
        IntPtr hWnd = FindWindow("CASCADIA_HOSTING_WINDOW_CLASS", string.Empty); // Use empty string instead of null

        if (hWnd != IntPtr.Zero)
        {
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
            Process.Start("wt.exe");
        }
    }
}

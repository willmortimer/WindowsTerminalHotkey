using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Threading;

public static class WindowsTerminalManager
{
    public enum WindowPosition
    {
        FullScreen,
        LeftHalf,
        RightHalf,
        TopHalf,
        BottomHalf,
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight,
        Centered,
        Custom
    }

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern IntPtr FindWindow(string lpClassName, string? lpWindowName);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool IsWindowVisible(IntPtr hWnd);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool BringWindowToTop(IntPtr hWnd);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool AttachThreadInput(IntPtr idAttach, IntPtr idAttachTo, bool fAttach);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out IntPtr lpdwProcessId);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr GetCurrentThreadId();

    [DllImport("user32.dll")]
    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    [DllImport("user32.dll")]
    private static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

    [DllImport("user32.dll")]
    private static extern int GetSystemMetrics(int nIndex);

    [DllImport("user32.dll")]
    private static extern bool SystemParametersInfo(uint uiAction, uint uiParam, IntPtr pvParam, uint fWinIni);

    [DllImport("user32.dll")]
    private static extern bool IsIconic(IntPtr hWnd);

    [DllImport("dwmapi.dll")]
    private static extern int DwmEnableComposition(bool bEnable);

    [DllImport("dwmapi.dll")]
    private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

    [StructLayout(LayoutKind.Sequential)]
    private struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    private const int SM_CXSCREEN = 0;
    private const int SM_CYSCREEN = 1;
    private const int SW_HIDE = 0;
    private const int SW_SHOW = 5;
    private const int SW_MINIMIZE = 6;
    private const int SW_RESTORE = 9;
    private const uint SWP_SHOWWINDOW = 0x0040;
    private const uint SWP_HIDEWINDOW = 0x0080;
    private const uint SWP_NOACTIVATE = 0x0010;
    private const uint SWP_ASYNCWINDOWPOS = 0x4000;
    private const uint SWP_NOOWNERZORDER = 0x0200;
    private const uint SWP_NOMOVE = 0x0002;
    private const uint SWP_NOSIZE = 0x0001;
    private const uint SPI_SETANIMATION = 0x0049;
    private const int DWMWA_TRANSITIONS_FORCEDISABLED = 3;
    private static readonly IntPtr HWND_TOP = new(0);
    private static readonly object _lockObject = new();
    private static bool _isProcessing;

    private static void DisableWindowAnimations()
    {
        try
        {
            SystemParametersInfo(SPI_SETANIMATION, 0, IntPtr.Zero, 0);
        }
        catch
        {
            // Ignore any errors, animations will still work
        }
    }

    private static void DisableTransitions(IntPtr hWnd)
    {
        try
        {
            int value = 1;
            DwmSetWindowAttribute(hWnd, DWMWA_TRANSITIONS_FORCEDISABLED, ref value, sizeof(int));
        }
        catch
        {
            // Ignore any errors
        }
    }

    public static void ToggleWindowsTerminal(WindowPosition position, int? customX = null, int? customY = null, int? customWidth = null, int? customHeight = null)
    {
        // Use a lock to prevent multiple simultaneous toggles
        if (!Monitor.TryEnter(_lockObject, 100))
        {
            Console.WriteLine("Toggle operation in progress, please wait...");
            return;
        }

        try
        {
            if (_isProcessing)
            {
                return;
            }

            _isProcessing = true;
            DisableWindowAnimations();

            IntPtr hWnd = FindWindow("CASCADIA_HOSTING_WINDOW_CLASS", null);
            if (hWnd == IntPtr.Zero)
            {
                // No terminal instance found, start a new one
                Process.Start(new ProcessStartInfo
                {
                    FileName = "wt.exe",
                    UseShellExecute = true
                });
                return;
            }

            // Disable DWM transitions for this window
            DisableTransitions(hWnd);

            IntPtr foregroundWindow = GetForegroundWindow();
            bool isVisible = IsWindowVisible(hWnd);
            bool isIconic = IsIconic(hWnd);

            if (foregroundWindow == hWnd && isVisible && !isIconic)
            {
                // Terminal is focused and visible, hide it
                SetWindowPos(hWnd, IntPtr.Zero, 0, 0, 0, 0, 
                    SWP_HIDEWINDOW | SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE | SWP_ASYNCWINDOWPOS);
                Console.WriteLine("Hiding terminal");
            }
            else
            {
                // Show and position the window
                if (isIconic)
                {
                    ShowWindow(hWnd, SW_RESTORE);
                }

                SetWindowPosition(hWnd, position, customX, customY, customWidth, customHeight);
                
                // Ensure window is visible and focused
                SetWindowPos(hWnd, HWND_TOP, 0, 0, 0, 0, 
                    SWP_SHOWWINDOW | SWP_NOMOVE | SWP_NOSIZE | SWP_ASYNCWINDOWPOS);
                
                Thread.Sleep(10); // Small delay to ensure window is ready
                SetForegroundWindow(hWnd);
                Console.WriteLine("Showing and focusing terminal");
            }
        }
        finally
        {
            _isProcessing = false;
            Monitor.Exit(_lockObject);
        }
    }

    private static void SetWindowPosition(IntPtr hWnd, WindowPosition position, int? customX, int? customY, int? customWidth, int? customHeight)
    {
        int screenWidth = GetSystemMetrics(SM_CXSCREEN);
        int screenHeight = GetSystemMetrics(SM_CYSCREEN);

        int x = 0, y = 0, width = 0, height = 0;

        switch (position)
        {
            case WindowPosition.FullScreen:
                x = 0;
                y = 0;
                width = screenWidth;
                height = screenHeight;
                break;

            case WindowPosition.LeftHalf:
                width = screenWidth / 2;
                height = screenHeight;
                break;

            case WindowPosition.RightHalf:
                x = screenWidth / 2;
                width = screenWidth / 2;
                height = screenHeight;
                break;

            case WindowPosition.TopHalf:
                width = screenWidth;
                height = screenHeight / 2;
                break;

            case WindowPosition.BottomHalf:
                y = screenHeight / 2;
                width = screenWidth;
                height = screenHeight / 2;
                break;

            case WindowPosition.TopLeft:
                width = screenWidth / 2;
                height = screenHeight / 2;
                break;

            case WindowPosition.TopRight:
                x = screenWidth / 2;
                width = screenWidth / 2;
                height = screenHeight / 2;
                break;

            case WindowPosition.BottomLeft:
                y = screenHeight / 2;
                width = screenWidth / 2;
                height = screenHeight / 2;
                break;

            case WindowPosition.BottomRight:
                x = screenWidth / 2;
                y = screenHeight / 2;
                width = screenWidth / 2;
                height = screenHeight / 2;
                break;

            case WindowPosition.Centered:
                width = (int)(screenWidth * 0.8);
                height = (int)(screenHeight * 0.8);
                x = (screenWidth - width) / 2;
                y = (screenHeight - height) / 2;
                break;

            case WindowPosition.Custom:
                x = customX ?? 0;
                y = customY ?? 0;
                width = customWidth ?? screenWidth;
                height = customHeight ?? screenHeight;
                break;
        }

        SetWindowPos(hWnd, HWND_TOP, x, y, width, height, 
            SWP_ASYNCWINDOWPOS | SWP_NOOWNERZORDER | SWP_SHOWWINDOW);
    }
}

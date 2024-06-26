using System;
using System.Runtime.InteropServices;

public static class HotKeyManager
{
    [DllImport("user32.dll")]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll")]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    public const uint MOD_ALT = 0x0001;
    public const int HOTKEY_ID = 1;

    // Renamed the public methods to avoid naming conflict
    public static bool RegisterSystemHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk)
    {
        return RegisterHotKey(hWnd, id, fsModifiers, vk);
    }

    public static bool UnregisterSystemHotKey(IntPtr hWnd, int id)
    {
        return UnregisterHotKey(hWnd, id);
    }
}
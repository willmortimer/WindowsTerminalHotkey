using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

public static partial class HotKeyManager
{
    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool UnregisterHotKey(IntPtr hWnd, int id);

    public const int HOTKEY_ID = 1;

    public static bool RegisterSystemHotKey(IntPtr hWnd, uint modifier, uint key)
    {
        return RegisterHotKey(hWnd, HOTKEY_ID, modifier, key);
    }

    public static bool UnregisterSystemHotKey(IntPtr hWnd)
    {
        return UnregisterHotKey(hWnd, HOTKEY_ID);
    }
}

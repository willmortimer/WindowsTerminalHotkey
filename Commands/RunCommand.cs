using Spectre.Console.Cli;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

public class RunCommand : Command<RunCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [Description("Path to the configuration file.")]
        [CommandOption("--configpath")]
        public string ConfigPath { get; set; } = string.Empty;

        [Description("Run in silent mode.")]
        [CommandOption("--silent")]
        public bool Silent { get; set; }
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        string configPath = string.IsNullOrEmpty(settings.ConfigPath) ? "config.json" : settings.ConfigPath;
        Config config = ConfigManager.Load(configPath);

        if (config.RunAtStartup)
        {
            StartupManager.AddToStartup("WindowsTerminalHotkey", System.Reflection.Assembly.GetExecutingAssembly().Location);
        }
        else
        {
            StartupManager.RemoveFromStartup("WindowsTerminalHotkey");
        }

        if (settings.Silent || config.SilentMode)
        {
            RunHeadless(config);
        }
        else
        {
            RunWithUI(config);
        }

        return 0;
    }

    private void RunHeadless(Config config)
    {
        HotKeyManager.RegisterSystemHotKey(IntPtr.Zero, HotKeyManager.HOTKEY_ID, config.Modifier, config.Key);

        while (!Console.KeyAvailable)
        {
            if (GetMessage(out MSG msg, IntPtr.Zero, 0, 0) > 0)
            {
                if (msg.message == WM_HOTKEY)
                {
                    WindowsTerminalManager.ToggleWindowsTerminal();
                }
            }
        }

        HotKeyManager.UnregisterSystemHotKey(IntPtr.Zero, HotKeyManager.HOTKEY_ID);
    }

    private void RunWithUI(Config config)
    {
        HotKeyManager.RegisterSystemHotKey(IntPtr.Zero, HotKeyManager.HOTKEY_ID, config.Modifier, config.Key);

        Console.WriteLine("Hotkey registered. Press Alt+Space to toggle Windows Terminal.");
        Console.WriteLine("Press any key to exit...");

        while (!Console.KeyAvailable)
        {
            if (GetMessage(out MSG msg, IntPtr.Zero, 0, 0) > 0)
            {
                if (msg.message == WM_HOTKEY)
                {
                    WindowsTerminalManager.ToggleWindowsTerminal();
                }
            }
        }

        HotKeyManager.UnregisterSystemHotKey(IntPtr.Zero, HotKeyManager.HOTKEY_ID);
    }

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

using Spectre.Console.Cli;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;

public class RunCommand : Command<RunCommand.Settings>
{
    private Config? _config;
    private SystemTrayManager? _trayManager;

    public class Settings : CommandSettings
    {
        [Description("Path to the configuration file.")]
        [CommandOption("--configpath")]
        public string? ConfigPath { get; set; }

        [Description("Run in silent mode.")]
        [CommandOption("--silent")]
        public bool Silent { get; set; }
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        _config = ConfigManager.Load(settings.ConfigPath);

        if (_config.RunAtStartup)
        {
            StartupManager.AddToStartup("WindowsTerminalHotkey", System.Reflection.Assembly.GetExecutingAssembly().Location);
        }
        else
        {
            StartupManager.RemoveFromStartup("WindowsTerminalHotkey");
        }

        if (settings.Silent || _config.SilentMode)
        {
            RunHeadless();
        }
        else
        {
            RunWithUI();
        }

        return 0;
    }

    private void RunHeadless()
    {
        if (_config == null) return;

        InitializeSystemTray();

        if (!HotKeyManager.RegisterSystemHotKey(IntPtr.Zero, _config.Modifier, _config.Key))
        {
            Console.WriteLine($"Failed to register hotkey. Modifier: {_config.Modifier}, Key: {_config.Key}");
            return;
        }

        Console.WriteLine($"Hotkey registered. Modifier: {_config.Modifier}, Key: {_config.Key}. Running in headless mode.");

        Application.Run();

        HotKeyManager.UnregisterSystemHotKey(IntPtr.Zero);
        _trayManager?.Dispose();
    }

    private void RunWithUI()
    {
        if (_config == null) return;

        InitializeSystemTray();

        if (!HotKeyManager.RegisterSystemHotKey(IntPtr.Zero, _config.Modifier, _config.Key))
        {
            Console.WriteLine($"Failed to register hotkey. Modifier: {_config.Modifier}, Key: {_config.Key}");
            return;
        }

        Console.WriteLine($"Hotkey registered. Modifier: {_config.Modifier}, Key: {_config.Key}");
        Console.WriteLine($"Press the configured hotkey to toggle Windows Terminal.");
        Console.WriteLine("Right-click the system tray icon for options or press Ctrl+C to exit.");

        Application.Run();

        HotKeyManager.UnregisterSystemHotKey(IntPtr.Zero);
        _trayManager?.Dispose();
    }

    private void InitializeSystemTray()
    {
        if (_config == null) return;

        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        _trayManager = new SystemTrayManager(
            onPositionSelected: position =>
            {
                _config.WindowPosition = position.ToString();
                ConfigManager.Save(_config);
                ToggleTerminal();
            },
            onSettingsRequested: () =>
            {
                // TODO: Implement settings dialog
                MessageBox.Show("Settings dialog coming soon!", "Windows Terminal Hotkey");
            }
        );

        _trayManager.Initialize();

        // Register for Windows messages
        Application.AddMessageFilter(new HotKeyMessageFilter(ToggleTerminal));
    }

    private void ToggleTerminal()
    {
        if (_config == null) return;

        WindowsTerminalManager.ToggleWindowsTerminal(
            _config.GetWindowPosition(),
            _config.OverrideX,
            _config.OverrideY,
            _config.OverrideWidth,
            _config.OverrideHeight
        );
    }

    private class HotKeyMessageFilter : IMessageFilter
    {
        private readonly Action _onHotKeyPressed;
        private const int WM_HOTKEY = 0x0312;

        public HotKeyMessageFilter(Action onHotKeyPressed)
        {
            _onHotKeyPressed = onHotKeyPressed;
        }

        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == WM_HOTKEY)
            {
                _onHotKeyPressed();
                return true;
            }
            return false;
        }
    }
}

using System;
using System.Drawing;
using System.Windows.Forms;

/// Manages the system tray icon and context menu for the application.
public class SystemTrayManager : IDisposable
{
    private NotifyIcon? _trayIcon;
    private ContextMenuStrip? _contextMenu;
    private readonly Action<WindowsTerminalManager.WindowPosition>? _onPositionSelected;
    private readonly Action? _onSettingsRequested;

    public SystemTrayManager(Action<WindowsTerminalManager.WindowPosition>? onPositionSelected = null, Action? onSettingsRequested = null)
    {
        _onPositionSelected = onPositionSelected;
        _onSettingsRequested = onSettingsRequested;
    }

    /// Initializes the system tray manager by creating the context menu and setting up the tray icon.
    public void Initialize()
    {
        _contextMenu = new ContextMenuStrip();

        // Window Position submenu
        var positionMenu = new ToolStripMenuItem("Window Position");
        positionMenu.DropDownItems.Add(new ToolStripMenuItem("Full Screen", null, (s, e) => _onPositionSelected?.Invoke(WindowsTerminalManager.WindowPosition.FullScreen)));
        positionMenu.DropDownItems.Add(new ToolStripMenuItem("Left Half", null, (s, e) => _onPositionSelected?.Invoke(WindowsTerminalManager.WindowPosition.LeftHalf)));
        positionMenu.DropDownItems.Add(new ToolStripMenuItem("Right Half", null, (s, e) => _onPositionSelected?.Invoke(WindowsTerminalManager.WindowPosition.RightHalf)));
        positionMenu.DropDownItems.Add(new ToolStripMenuItem("Top Half", null, (s, e) => _onPositionSelected?.Invoke(WindowsTerminalManager.WindowPosition.TopHalf)));
        positionMenu.DropDownItems.Add(new ToolStripMenuItem("Bottom Half", null, (s, e) => _onPositionSelected?.Invoke(WindowsTerminalManager.WindowPosition.BottomHalf)));
        positionMenu.DropDownItems.Add(new ToolStripMenuItem("Top Left", null, (s, e) => _onPositionSelected?.Invoke(WindowsTerminalManager.WindowPosition.TopLeft)));
        positionMenu.DropDownItems.Add(new ToolStripMenuItem("Top Right", null, (s, e) => _onPositionSelected?.Invoke(WindowsTerminalManager.WindowPosition.TopRight)));
        positionMenu.DropDownItems.Add(new ToolStripMenuItem("Bottom Left", null, (s, e) => _onPositionSelected?.Invoke(WindowsTerminalManager.WindowPosition.BottomLeft)));
        positionMenu.DropDownItems.Add(new ToolStripMenuItem("Bottom Right", null, (s, e) => _onPositionSelected?.Invoke(WindowsTerminalManager.WindowPosition.BottomRight)));
        positionMenu.DropDownItems.Add(new ToolStripMenuItem("Centered", null, (s, e) => _onPositionSelected?.Invoke(WindowsTerminalManager.WindowPosition.Centered)));

        _contextMenu.Items.Add(positionMenu);
        _contextMenu.Items.Add(new ToolStripSeparator());
        _contextMenu.Items.Add(new ToolStripMenuItem("Settings", null, (s, e) => _onSettingsRequested?.Invoke()));
        _contextMenu.Items.Add(new ToolStripSeparator());
        _contextMenu.Items.Add(new ToolStripMenuItem("Exit", null, OnExit));

        _trayIcon = new NotifyIcon
        {
            Icon = new Icon("HotKeyTerminal.ico"),
            Text = "Windows Terminal Hotkey",
            Visible = true,
            ContextMenuStrip = _contextMenu
        };

        _trayIcon.DoubleClick += (s, e) => _onSettingsRequested?.Invoke();
    }

    private void OnExit(object? sender, EventArgs e)
    {
        Application.Exit();
    }

    /// Disposes the system tray manager by releasing any resources used by the tray icon and context menu.
    public void Dispose()
    {
        _trayIcon?.Dispose();
        _contextMenu?.Dispose();
    }
}

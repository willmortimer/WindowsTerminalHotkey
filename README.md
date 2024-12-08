# Windows Terminal Hotkey

A Windows application that brings iTerm2-like hotkey window functionality to Windows Terminal. It allows you to show/hide Windows Terminal with a global hotkey and supports various window positions and profiles.

## Features

- Global hotkey to toggle Windows Terminal visibility
- Multiple window positions:
  - Full Screen
  - Left/Right Half
  - Top/Bottom Half
  - Top Left/Right
  - Bottom Left/Right
  - Centered
  - Custom position and size
- System tray integration with quick access to window positions
- Instant window transitions without animations
- Support for all Windows Terminal profiles
- Run at startup option
- Silent mode operation

## Requirements

- Windows 10 or later
- .NET 8.0 or later
- Windows Terminal (installed from Microsoft Store or GitHub)

## Installation

1. Clone the repository:
```bash
git clone https://github.com/willmortimer/WindowsTerminalHotkey.git
```

2. Build the project:
```bash
cd WindowsTerminalHotkey
dotnet build
```

3. Run the setup:
```bash
dotnet run -- setup
```

## Configuration

Run the setup command to configure:
- Hotkey combination (modifier + key)
- Default Windows Terminal profile
- Window position and size
- Startup and silent mode options

The configuration is stored in:
`%APPDATA%\WindowsTerminalHotkey\config.json`

### Example Configuration

```json
{
    "Modifier": 8,          // Windows key
    "Key": 192,            // Tilde key
    "Profile": "PowerShell",
    "WindowPosition": "RightHalf",
    "RunAtStartup": true,
    "SilentMode": false
}
```

## Usage

1. Start the application:
```bash
dotnet run -- run
```

2. Use the configured hotkey to toggle Windows Terminal
3. Right-click the system tray icon to:
   - Change window position
   - Access settings
   - Exit the application

## Building from Source

### Prerequisites
- .NET 8.0 SDK
- Visual Studio 2022 or VS Code with C# extension

### Build Steps
1. Clone the repository
2. Open in Visual Studio or VS Code
3. Restore NuGet packages:
```bash
dotnet restore
```
4. Build the solution:
```bash
dotnet build
```

## Project Structure

- `Commands/` - CLI commands for setup and running
- `Config/` - Configuration management
- `Hotkeys/` - Hotkey and window management
- `Startup/` - Startup registration
- `SystemTray/` - System tray icon and menu

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Acknowledgments

- Inspired by iTerm2's hotkey window feature
- Uses Windows Terminal's rich functionality
- Built with .NET 8.0 
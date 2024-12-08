public class Config
{
    public uint Modifier { get; set; }
    public uint Key { get; set; }
    public string Profile { get; set; } = "Windows PowerShell";
    public string WindowPosition { get; set; } = "Centered"; // Can be "FullScreen", "LeftHalf", "RightHalf", "TopHalf", "BottomHalf", "TopLeft", "TopRight", "BottomLeft", "BottomRight", "Centered"
    public int? OverrideX { get; set; } // Optional override for X coordinate
    public int? OverrideY { get; set; } // Optional override for Y coordinate
    public int? OverrideWidth { get; set; } // Optional override for window width
    public int? OverrideHeight { get; set; } // Optional override for window height
    public bool RunAtStartup { get; set; }
    public bool SilentMode { get; set; }

    public WindowsTerminalManager.WindowPosition GetWindowPosition()
    {
        return WindowPosition?.ToLower() switch
        {
            "fullscreen" => WindowsTerminalManager.WindowPosition.FullScreen,
            "lefthalf" => WindowsTerminalManager.WindowPosition.LeftHalf,
            "righthalf" => WindowsTerminalManager.WindowPosition.RightHalf,
            "tophalf" => WindowsTerminalManager.WindowPosition.TopHalf,
            "bottomhalf" => WindowsTerminalManager.WindowPosition.BottomHalf,
            "topleft" => WindowsTerminalManager.WindowPosition.TopLeft,
            "topright" => WindowsTerminalManager.WindowPosition.TopRight,
            "bottomleft" => WindowsTerminalManager.WindowPosition.BottomLeft,
            "bottomright" => WindowsTerminalManager.WindowPosition.BottomRight,
            "centered" => WindowsTerminalManager.WindowPosition.Centered,
            _ => WindowsTerminalManager.WindowPosition.Custom
        };
    }
}

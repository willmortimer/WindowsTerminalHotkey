public class Config
{
    public uint Modifier { get; set; }
    public uint Key { get; set; }
    public string Profile { get; set; } = string.Empty;
    public int WindowX { get; set; }
    public int WindowY { get; set; }
    public int WindowWidth { get; set; }
    public int WindowHeight { get; set; }
    public bool RunAtStartup { get; set; }
    public bool SilentMode { get; set; }
}

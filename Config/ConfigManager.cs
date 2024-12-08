using System;
using System.IO;
using Newtonsoft.Json;

public static class ConfigManager
{
    private static string DefaultConfigPath => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "WindowsTerminalHotkey",
        "config.json"
    );

    public static string GetConfigPath(string? overridePath = null)
    {
        return string.IsNullOrEmpty(overridePath) ? DefaultConfigPath : overridePath;
    }

    public static Config Load(string? overridePath = null)
    {
        string configPath = GetConfigPath(overridePath);

        if (File.Exists(configPath))
        {
            string json = File.ReadAllText(configPath);
            return JsonConvert.DeserializeObject<Config>(json) ?? new Config();
        }
        else   //debug line
        {
            Console.WriteLine("Config file not found");
        }
        return new Config(); // Return default config if file doesn't exist
    }

    public static void Save(Config config, string? overridePath = null)
    {
        string configPath = GetConfigPath(overridePath);

        // Ensure the directory exists
        Directory.CreateDirectory(Path.GetDirectoryName(configPath)!);

        string json = JsonConvert.SerializeObject(config, Formatting.Indented);
        File.WriteAllText(configPath, json);
    }
}
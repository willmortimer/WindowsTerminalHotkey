using System.IO;
using Newtonsoft.Json;

public static class ConfigManager
{
    public static Config Load(string path)
    {
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<Config>(json) ?? new Config();
        }
        return new Config(); // Return default config if file doesn't exist
    }

    public static void Save(Config config, string path)
    {
        string json = JsonConvert.SerializeObject(config, Formatting.Indented);
        File.WriteAllText(path, json);
    }
}

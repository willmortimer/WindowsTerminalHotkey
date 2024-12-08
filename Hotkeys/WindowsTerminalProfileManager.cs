using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Linq;

public static class WindowsTerminalProfileManager
{
    public static List<string> GetProfiles()
    {
        try
        {
            string settingsPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Packages",
                "Microsoft.WindowsTerminal_8wekyb3d8bbwe",
                "LocalState",
                "settings.json"
            );

            if (!File.Exists(settingsPath))
            {
                // Try the unpackaged version
                settingsPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "Microsoft",
                    "Windows Terminal",
                    "settings.json"
                );
            }

            if (!File.Exists(settingsPath))
            {
                return new List<string> { "Windows PowerShell" }; // Default fallback
            }

            string jsonContent = File.ReadAllText(settingsPath);
            using JsonDocument document = JsonDocument.Parse(jsonContent);
            
            var profiles = new List<string>();
            
            // Check if profiles are in the new format (under profiles.list)
            if (document.RootElement.TryGetProperty("profiles", out JsonElement profilesElement) &&
                profilesElement.TryGetProperty("list", out JsonElement listElement))
            {
                foreach (JsonElement profile in listElement.EnumerateArray())
                {
                    if (profile.TryGetProperty("name", out JsonElement nameElement))
                    {
                        profiles.Add(nameElement.GetString() ?? "");
                    }
                }
            }
            // Check old format (direct under profiles)
            else if (document.RootElement.TryGetProperty("profiles", out JsonElement oldProfilesElement))
            {
                foreach (JsonElement profile in oldProfilesElement.EnumerateArray())
                {
                    if (profile.TryGetProperty("name", out JsonElement nameElement))
                    {
                        profiles.Add(nameElement.GetString() ?? "");
                    }
                }
            }

            return profiles.Where(p => !string.IsNullOrEmpty(p)).ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading Windows Terminal profiles: {ex.Message}");
            return new List<string> { "Windows PowerShell" }; // Default fallback
        }
    }
} 
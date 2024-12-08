using Microsoft.Win32;
using System;

public static class StartupManager
{
    public static void AddToStartup(string appName, string appPath)
    {
        try
        {
            #pragma warning disable CA1416
            RegistryKey? rk = Registry.CurrentUser?.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (rk != null)
            {
                rk.SetValue(appName, appPath);
            }
            #pragma warning restore CA1416
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to add to startup: {ex.Message}");
        }
    }

    public static void RemoveFromStartup(string appName)
    {
        try
        {
            #pragma warning disable CA1416
            RegistryKey? rk = Registry.CurrentUser?.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (rk != null)
            {
                rk.DeleteValue(appName, false);
            }
            #pragma warning restore CA1416
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to remove from startup: {ex.Message}");
        }
    }
}

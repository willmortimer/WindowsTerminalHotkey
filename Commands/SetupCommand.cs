using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Linq;

public class SetupCommand : Command<SetupCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [Description("Path to save the configuration file.")]
        [CommandOption("--configpath")]
        public string? ConfigPath { get; set; }
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        var config = new Config();

        config.Modifier = AnsiConsole.Prompt(
            new SelectionPrompt<uint>()
                .Title("Select modifier key:")
                .AddChoices(new[] {
                    (uint)0x0001, // ALT
                    (uint)0x0002, // CTRL
                    (uint)0x0004, // SHIFT
                    (uint)0x0008  // WINDOWS
                })
                .UseConverter(m => m switch
                {
                    0x0001 => "ALT",
                    0x0002 => "CTRL",
                    0x0004 => "SHIFT",
                    0x0008 => "WINDOWS",
                    _ => m.ToString()
                }));

        config.Key = AnsiConsole.Prompt(
            new TextPrompt<uint>("Enter key code (e.g., 32 for SPACE):")
                .PromptStyle("green")
                .ValidationErrorMessage("[red]Please enter a valid key code[/]")
                .Validate(key => key > 0));

        var profiles = WindowsTerminalProfileManager.GetProfiles();
        if (profiles.Count > 0)
        {
            config.Profile = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select Windows Terminal profile:")
                    .AddChoices(profiles));
        }
        else
        {
            config.Profile = AnsiConsole.Prompt(
                new TextPrompt<string>("Enter Windows Terminal profile name:")
                    .DefaultValue("Windows PowerShell")
                    .PromptStyle("green"));
        }

        config.WindowPosition = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select default window position:")
                .AddChoices(new[] {
                    "FullScreen",
                    "LeftHalf",
                    "RightHalf",
                    "TopHalf",
                    "BottomHalf",
                    "TopLeft",
                    "TopRight",
                    "BottomLeft",
                    "BottomRight",
                    "Centered",
                    "Custom"
                }));

        if (config.WindowPosition == "Custom")
        {
            config.OverrideX = AnsiConsole.Prompt(
                new TextPrompt<int>("Enter window X position:")
                    .PromptStyle("green"));

            config.OverrideY = AnsiConsole.Prompt(
                new TextPrompt<int>("Enter window Y position:")
                    .PromptStyle("green"));

            config.OverrideWidth = AnsiConsole.Prompt(
                new TextPrompt<int>("Enter window width:")
                    .PromptStyle("green"));

            config.OverrideHeight = AnsiConsole.Prompt(
                new TextPrompt<int>("Enter window height:")
                    .PromptStyle("green"));
        }

        config.RunAtStartup = AnsiConsole.Confirm("Run at startup?", false);
        config.SilentMode = AnsiConsole.Confirm("Run in silent mode?", false);

        ConfigManager.Save(config, settings.ConfigPath);

        AnsiConsole.MarkupLine("[green]Configuration saved successfully![/]");
        AnsiConsole.MarkupLine($"[blue]Configuration file:[/] {ConfigManager.GetConfigPath(settings.ConfigPath)}");
        AnsiConsole.MarkupLine("\n[yellow]Run the application with 'run' command to start.[/]");

        return 0;
    }
}
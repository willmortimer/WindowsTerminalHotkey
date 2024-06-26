using Spectre.Console;
using Spectre.Console.Cli;

public class SetupCommand : Command
{
    public override int Execute(CommandContext context)
    {
        var config = new Config();

        config.Modifier = AnsiConsole.Prompt(
            new TextPrompt<uint>("Enter modifier key (e.g., 0x0001 for ALT):")
                .PromptStyle("green"));

        config.Key = AnsiConsole.Prompt(
            new TextPrompt<uint>("Enter key (e.g., 0x20 for SPACE):")
                .PromptStyle("green"));

        config.Profile = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter profile name:")
                .PromptStyle("green"));

        config.WindowX = AnsiConsole.Prompt(
            new TextPrompt<int>("Enter window X position:")
                .PromptStyle("green"));

        config.WindowY = AnsiConsole.Prompt(
            new TextPrompt<int>("Enter window Y position:")
                .PromptStyle("green"));

        config.WindowWidth = AnsiConsole.Prompt(
            new TextPrompt<int>("Enter window width:")
                .PromptStyle("green"));

        config.WindowHeight = AnsiConsole.Prompt(
            new TextPrompt<int>("Enter window height:")
                .PromptStyle("green"));

        config.RunAtStartup = AnsiConsole.Confirm("Run at startup?");

        config.SilentMode = AnsiConsole.Confirm("Run in silent mode?");

        string configPath = "config.json";
        ConfigManager.Save(config, configPath);

        return 0;
    }
}

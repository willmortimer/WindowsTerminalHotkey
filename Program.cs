using Spectre.Console.Cli;
using System;

class Program
{
    static int Main(string[] args)
    {
        var app = new CommandApp();
        app.Configure(config =>
        {
            config.AddCommand<RunCommand>("run");
            config.AddCommand<SetupCommand>("setup");
        });

        return app.Run(args);
    }
}

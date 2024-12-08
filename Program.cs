using Spectre.Console.Cli;
using System;
using System.Linq;
using System.Windows.Forms;

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

        if (args.Contains("--headless"))
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Run the app headlessly
            var result = app.Run(["run"]);

            return result;
        }

        return app.Run(args);
    }
}

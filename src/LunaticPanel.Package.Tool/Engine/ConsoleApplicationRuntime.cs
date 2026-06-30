using LunaticPanel.Package.Tool.Extensions;
using System.CommandLine;

namespace LunaticPanel.Package.Tool.Engine;

public sealed class ConsoleApplicationRuntime
{
    public IServiceProvider ServiceProvider { get; init; } = default!;
    public string[] Arguments { get; init; } = default!;

    public async Task RunAsync(CancellationToken ct = default)
    {
        await RunStartupCommandAsync(ct, Arguments);
    }
    internal static async Task RunStartupCommandAsync(CancellationToken ct, params string[] args)
    {
        var rootCommand = new RootCommand("Game Server Installation CLI");
        var modCommand = new Command("mod", "All commands related to mods support.")
        .AddOption<bool>("check", "c", "Check if the game server is supporting mods.")
        .AddOption<bool>("details", "dt", "Get the details about mods.")
        .AddOption<bool>("rm-current", "rmc", "Remove current as active modlist.")
        .SetModAction(services);

        var setupCommand = new Command("setup", "All commands related to setup.")
            .AddOption<bool>("install", "i", "Perform installation of the game server.")
            .AddOption<bool>("update", "u", "Perform update for game server.")
            .AddOption<bool>("version", "v", "Perform check for curren version of installed game server.")
            .AddOption<bool>("check-update", "cu", "Perform check if new update for game server is available.")
            .SetInstallationAction(services);

        var serverCommand = new Command("server", "All commands related to server control.")
            .AddOption<bool>("start", "st", "Start the server.")
            .AddOption<bool>("stop", "sp", "Stop the server.")
            .AddOption<bool>("restart", "r", "Restart the server.")
            .AddOption<bool>("status", "s", "Status the server.")
            .SetServerAction(services);
        var initCommand = new Command("initialize", "Command to install and setup all prerequisite for the installation of new game server.")
.SetInitializingAction(services);
        rootCommand.WithSubCommand(modCommand)
            .WithSubCommand(setupCommand)
            .WithSubCommand(serverCommand)
            .WithSubCommand(initCommand);


        var cmdParsed = rootCommand.Parse(args);

        if (cmdParsed.Errors.Count > 0)
        {
            await rootCommand.PrintHelp();
            Environment.Exit(1);
        }
        else
        {
            await cmdParsed.InvokeAsync(null, ct);
        }

    }
}

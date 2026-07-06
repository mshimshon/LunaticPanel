using LunaticPanel.Core.Abstraction.Exceptions;
using LunaticPanel.Package.Tool.Extensions;
using LunaticPanel.Package.Tool.Payloads;
using LunaticPanel.Package.Tool.Tools.Packing;
using LunaticPanel.Package.Tool.Tools.Validation;
using System.CommandLine;

namespace LunaticPanel.Package.Tool.Engine;

public sealed class ConsoleApplicationRuntime
{
    public IServiceProvider ServiceProvider { get; init; } = default!;
    private string[] _args { get; init; } = default!;
    public ConsoleApplicationRuntime(string[] args)
    {
        _args = args;
    }
    public async Task RunAsync(CancellationToken ct = default)
    {
        await RunStartupCommandAsync(ct, _args);
    }
    internal static async Task RunStartupCommandAsync(CancellationToken ct, params string[] args)
    {
        var rootCommand = new RootCommand("Game Server Installation CLI");
        rootCommand
            .WithPackCommands()
            .WithUnPackCommands()
            .WithValidateCommands();


        var cmdParsed = rootCommand.Parse(args);

        if (cmdParsed.Errors.Count > 0)
        {
            await rootCommand.PrintHelp();
            Environment.Exit(1);
        }
        else
        {
            try
            {
                await cmdParsed.InvokeAsync(null, ct);

            }
            catch (HostCodedException ex)
            {
                var errorPrint = new ResultResponse()
                {
                    Error = new ErrorResponse(ex.Code, ex.Message)
                };
                await errorPrint.PrintAsync();
                Environment.Exit(1);

            }
            catch (Exception ex)
            {
                var errorPrint = new ResultResponse()
                {
                    Error = new ErrorResponse(ex)
                };
                await errorPrint.PrintAsync();
                Environment.Exit(1);
            }
        }

    }
}

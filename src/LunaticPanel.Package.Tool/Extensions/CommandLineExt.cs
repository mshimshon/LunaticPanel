using LunaticPanel.Core.Abstraction.Exceptions;
using LunaticPanel.Package.Tool.Payloads;
using System.CommandLine;

namespace LunaticPanel.Package.Tool.Extensions;


internal static class CommandLineExt
{
    internal static async Task<bool> ExecuteCommandAsync(Func<Task> task)
    {
        bool success = await ExecuteCommandResultAsync(async () =>
        {
            await task();
            return "Success";
        });
        return success;
    }
    internal static async Task<bool> ExecuteCommandResultAsync(Func<Task<object?>> task)
    {
        bool success = false;
        try
        {

            object? serviceResult = await task();
            var outResult = new ResultResponse()
            {
                Data = serviceResult
            };

            await outResult.PrintAsync();
            success = true;
            return success;
        }
        catch (HostCodedException ex)
        {
            var outResult = new ResultResponse()
            {
                Error = new ErrorResponse(ex.Code, ex.Message)
            };
            await outResult.PrintAsync();
            return success;
        }
        catch (Exception ex)
        {
            var outResult = new ResultResponse()
            {
                Error = new ErrorResponse(ex)
            };
            await outResult.PrintAsync();
            return success;
        }
    }
    internal static Command AddOption<T>(this Command command, string name, string alias, string desc)
    {
        var option = new Option<T>($"--{name}", $"-{alias}")
        {
            Description = desc
        };
        command.Options.Add(option);
        return command;
    }

    internal static async Task PrintHelp(this Command command)
    {

        foreach (var option in command.Options)
        {
            Console.Error.WriteLine($"{string.Join(',', [option.Name, .. option.Aliases])} - {option.Description}");
        }
        foreach (Command subCommand in command.Subcommands)
        {
            Console.Error.WriteLine($"{string.Join(',', [subCommand.Name, .. subCommand.Aliases])} - {subCommand.Description}");
        }
        var outResult = new ResultResponse()
        {
            Error = new ErrorResponse(nameof(PrintHelp), "Command not available")

        };
        await outResult.PrintAsync();

    }
    internal static RootCommand WithSubCommand(this RootCommand rootCommand, Command command)
    {
        rootCommand.Subcommands.Add(command);
        return rootCommand;
    }
}


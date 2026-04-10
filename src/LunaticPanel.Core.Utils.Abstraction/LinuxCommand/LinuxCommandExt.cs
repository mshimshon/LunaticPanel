using LunaticPanel.Core.Utils.Abstraction.LinuxCommand.Exceptions;
using System.Text.Json;

namespace LunaticPanel.Core.Utils.Abstraction.LinuxCommand;

public static class LinuxCommandExt
{
    public static LinuxCommandBuilderContext BuildCommand(this ILinuxCommand linuxCommand, string command, bool outputIntoStdErr = false)
        => new() { CommandBuilder = new(RedirectStdOutToStdErr(command, outputIntoStdErr)), LinuxCommand = linuxCommand };
    public static LinuxCommandBuilderContext BuildBash(this ILinuxCommand linuxCommand, string file)
        => new() { CommandBuilder = new($"{Path.Combine("/", "bin", "bash")} {file}"), LinuxCommand = linuxCommand };

    /// <summary>
    /// previous command &amp;&amp; this command (Skip if previous command fails)
    /// </summary>
    public static LinuxCommandBuilderContext AndCommand(this LinuxCommandBuilderContext context, string command, bool outputIntoStdErr = false)
        => context.UpdateBuilder(p => p with { Command = p.Command + " && " + RedirectStdOutToStdErr(command, outputIntoStdErr) });

    /// <summary>
    /// previous command || this command (Execute this command only when previous fails)
    /// </summary>
    public static LinuxCommandBuilderContext OrCommand(this LinuxCommandBuilderContext context, string command, bool outputIntoStdErr = false)
        => context.UpdateBuilder(p => p with { Command = p.Command + " || " + RedirectStdOutToStdErr(command, outputIntoStdErr) });

    /// <summary>
    /// previous command | this command (Takes standard output of the previous command and relay into the input of the this command)
    /// </summary>
    public static LinuxCommandBuilderContext PatchInStandardCommand(this LinuxCommandBuilderContext context, string command, bool outputIntoStdErr = false)
        => context.UpdateBuilder(p => p with { Command = p.Command + " | " + RedirectStdOutToStdErr(command, outputIntoStdErr) });

    /// <summary>
    /// previous command |&amp; this command (Takes standard output and standard error of the previous command and relay into the input of the this command)
    /// </summary>
    public static LinuxCommandBuilderContext PatchInAllCommand(this LinuxCommandBuilderContext context, string command, bool outputIntoStdErr = false)
        => context.UpdateBuilder(p => p with { Command = p.Command + " |& " + RedirectStdOutToStdErr(command, outputIntoStdErr) });

    private static string RedirectStdOutToStdErr(string command, bool outputIntoStdErr) => outputIntoStdErr ? command + " 1>&2" : command;



    public static LinuxCommandBuilderContext AsUser(this LinuxCommandBuilderContext context, string user)
        => context.UpdateBuilder(p => p with { RunAs = user });
    public static LinuxCommandBuilderContext SetWorkingDir(this LinuxCommandBuilderContext context, string dir)
    => context.UpdateBuilder(p => p with { WorkingDirectory = dir });
    public static LinuxCommandBuilderContext Sudo(this LinuxCommandBuilderContext context)
        => context.UpdateBuilder(p => p with { Sudo = true });

    public static LinuxCommandBuilderContext PreserveEnvironmentVariable(this LinuxCommandBuilderContext context)
        => context.UpdateBuilder(p => p with { PerserveEnvironmentVariable = true });

    public static LinuxCommandBuilderContext ReadConsoleStandardOutput(this LinuxCommandBuilderContext context, Func<string, Task> onConsoleLine)
    => context.UpdateBuilder(p => p with { OnStantardOutput = onConsoleLine });

    public static LinuxCommandBuilderContext ReadConsoleErrorOutput(this LinuxCommandBuilderContext context, Func<string, Task> onConsoleLine)
    => context.UpdateBuilder(p => p with { OnErrorOutput = onConsoleLine });

    public static LinuxCommandBuilderContext ReadConsoleOutput(this LinuxCommandBuilderContext context, Func<string, Task> onConsoleLine)
    => context.ReadConsoleErrorOutput(onConsoleLine).ReadConsoleStandardOutput(onConsoleLine);

    public static async Task<LinuxCommandResult> ExecAsync(this LinuxCommandBuilderContext context, CancellationToken ct = default)
        => await context.LinuxCommand.RunCommand(context.CommandBuilder, ct);


    public static async Task<T> ExecAsync<T>(this LinuxCommandBuilderContext context, CancellationToken ct = default)
    where T : notnull
    {
        var result = await context.ExecAsync(ct);
        if (result.Failed)
            throw new CommandFailed(result);
        try
        {
            var expectResult = JsonSerializer.Deserialize<T>(result.StandardOutput);
            if (expectResult == null) throw new CommandFailed(result, "Command failed result is null.");
            return expectResult;
        }
        catch (Exception ex)
        {
            throw new CommandFailed(result, ex.Message);
        }
    }

    public static async Task<T> ExecOrDefaultAsync<T>(this LinuxCommandBuilderContext context, T? defaultValue, CancellationToken ct = default)
        where T : notnull
    {
        try
        {
            return await context.ExecAsync<T>(ct);
        }
        catch
        {
            if (defaultValue != null)
                return defaultValue;
            return default(T)!;
        }

    }



}
using LunaticPanel.Core.Utils.Abstraction.LinuxCommand.Enums;
using LunaticPanel.Core.Utils.Abstraction.LinuxCommand.Exceptions;
using LunaticPanel.Core.Utils.Abstraction.Logging;
using System.Text;

namespace LunaticPanel.Core.Utils.Abstraction.LinuxCommand;

public static class LinuxCommandExt
{
    public static LinuxCommandBuilderContext BuildCommand(this ILinuxCommand linuxCommand, string command)
        => new()
        {
            CommandBuilder = new(command),
            LinuxCommand = linuxCommand,
        };

    // [[{0}]]
    public static LinuxCommandBuilderContext BuildCommand(this ILinuxCommand linuxCommand, string command, string wrapper)
    => new()
    {
        CommandBuilder = new(string.Format(wrapper, $" {command} ")),
        LinuxCommand = linuxCommand,
    };
    public static LinuxCommandBuilderContext BuildBash(this ILinuxCommand linuxCommand, string file)
        => new()
        {
            CommandBuilder = new($"{Path.Combine("/", "bin", "bash")} {file}"),
            LinuxCommand = linuxCommand
        };

    private static IReadOnlyCollection<LinuxSubCommandDescriptor> CreateAndPreserveSubCommands(this LinuxCommandBuilderContext context, LinuxSubCommandDescriptor toAdd)
    {
        if (context.CommandBuilder.SubCommands != default)
            return new List<LinuxSubCommandDescriptor>(context.CommandBuilder.SubCommands) {
                toAdd
            }.AsReadOnly();
        return new List<LinuxSubCommandDescriptor>() {
                toAdd
            }.AsReadOnly();
    }

    private static LinuxCommandBuilderContext AddToSubCommands(this LinuxCommandBuilderContext context, string command, CommandOperand operand)
        => context.UpdateBuilder(p => p with
        {
            SubCommands = context.CreateAndPreserveSubCommands(new(command, operand))
        });
    /// <summary>
    /// previous command &amp;&amp; this command (Skip if previous command fails)
    /// </summary>
    public static LinuxCommandBuilderContext AndCommand(this LinuxCommandBuilderContext context, string command)
        => context.AddToSubCommands(command, CommandOperand.And);

    // /bin/sh -c 'payload=$(cat); printf "<<<PAYLOAD_BEGIN>>>%s<<<PAYLOAD_END>>>\n" "$payload"'
    // /bin/sh -c 'payload="$0"; printf "<<<PAYLOAD>>>%s<<<ENDPAYLOAD>>>\n" "$payload"' "$(cat)"
    // ls / && /bin/bash -c '\''payload="$0"; printf "<<<PAYLOAD_BEGIN>>>%s<<<PAYLOAD_BEGIN>>>" "$payload"'\'' '\''{"Completed":false,"failure_message":"hey"}'\'''

    private const string PAYLOAD_BEGIN_SYMBOL = @"<<<PAYLOAD_BEGIN>>>";
    private const string PAYLOAD_END_SYMBOL = @"<<<PAYLOAD_END>>>";

    private static string ToBase64(this string str)
    {
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(str));
    }
    // Create Bash
    // Its job to start then await input
    // using input we send the all commands raw and it execute just like the regular console would.
    // " \"--\" \" -- \"\"--- \"  "
    public static string WrapContentAsPayload(this ILinuxCommand linuxCommand, string payload, bool useBase64 = false)
    {
        string processedPayload = payload;
        if (useBase64)
            processedPayload = ToBase64(payload);
        return $"{PAYLOAD_BEGIN_SYMBOL}{processedPayload}{PAYLOAD_END_SYMBOL}";
    }

    public static string WrapContentAsPayload(this LinuxCommandBuilderContext context, string payload, bool useBase64 = false)
        => context.LinuxCommand.WrapContentAsPayload(payload, useBase64);

    private static string PrintPayloadCommand(string payload)
        => $"printf \"{PAYLOAD_BEGIN_SYMBOL}%s{PAYLOAD_END_SYMBOL}\\n\" \"{payload}\"";

    public static LinuxCommandBuilderContext AndPrintPayload(this LinuxCommandBuilderContext context, string payload)
        => context.AndCommand(PrintPayloadCommand(ToBase64(payload)));

    /// <summary>
    /// previous command || this command (Execute this command only when previous fails)
    /// </summary>
    public static LinuxCommandBuilderContext OrCommand(this LinuxCommandBuilderContext context, string command)
        => context.AddToSubCommands(command, CommandOperand.Or);
    public static LinuxCommandBuilderContext OrPrintPayload(this LinuxCommandBuilderContext context, string payload)
        => context.OrCommand(PrintPayloadCommand(ToBase64(payload)));
    public static LinuxCommandBuilderContext SetCrazyReport(this LinuxCommandBuilderContext context, ICrazyReport crazyReport)
    => context.UpdateBuilder(p => p with { CrazyReportOverride = crazyReport });

    /// <summary>
    /// This will automatically removed and strip each incoming line from color codes or other polluting characters.
    /// </summary>
    public static LinuxCommandBuilderContext AutoCleanConsoleStream(this LinuxCommandBuilderContext context)
        => context.UpdateBuilder(p => p with { AutoCleanConsoleStream = true });
    /// <summary>
    /// previous command | this command (Takes standard output of the previous command and relay into the input of the this command).\n
    /// </summary>
    public static LinuxCommandBuilderContext PatchInStdPipeCommand(this LinuxCommandBuilderContext context, string command)
        => context.AddToSubCommands(command, CommandOperand.PipeIn);
    /// <summary>
    /// previous command | print the result of the previous (stdout) command as a printed valid payload.\n
    /// </summary>
    public static LinuxCommandBuilderContext PatchInStdOutAsPayload(this LinuxCommandBuilderContext context)
    => context.PatchInStdPipeCommand(PrintPayloadCommand("$(cat)"));

    /// <summary>
    /// previous command |&amp; this command (Takes standard output and standard error of the previous command and relay into the input of the this command)
    /// </summary>
    public static LinuxCommandBuilderContext PatchInAllPipesCommand(this LinuxCommandBuilderContext context, string command)
        => context.AddToSubCommands(command, CommandOperand.PipeAll);


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

    public static async Task<ILinuxCommandResult> ExecAsync(this LinuxCommandBuilderContext context, CancellationToken ct = default)
        => await context.LinuxCommand.RunCommand(context.CommandBuilder, ct);

    public static T PayloadAs<T>(this ILinuxCommandResult context)
        where T : notnull
    {
        return DeserializeResult<T>(context.Payload, context);
    }
    public static T PayloadAsOrDefault<T>(this ILinuxCommandResult context, T? defaultValue)
    where T : notnull
    {
        try
        {
            return context.PayloadAs<T>();
        }
        catch
        {
            if (defaultValue != null)
                return defaultValue;
            return default(T)!;
        }
    }
    public static async Task<T> ExecPayloadAsync<T>(this LinuxCommandBuilderContext context, CancellationToken ct = default)
        where T : notnull
    {
        var result = await context.ExecAsync(ct);
        return result.PayloadAs<T>();
    }

    public static async Task<T> ExecPayloadOrDefaultAsync<T>(this LinuxCommandBuilderContext context, T? defaultValue, CancellationToken ct = default)
        where T : notnull
    {
        var result = await context.ExecAsync(ct);
        return result.PayloadAsOrDefault(defaultValue);
    }
    private static T DeserializeResult<T>(string? input, ILinuxCommandResult result)
where T : notnull
    {
        try
        {
            return result.DeserializeResult<T>(input);
        }
        catch (Exception ex)
        {
            throw new CommandFailedException(result.StandardOutput, result.StandardError, ex.Message);
        }
    }
    public static async Task<T> ExecOutputAsync<T>(this LinuxCommandBuilderContext context, CancellationToken ct = default)
        where T : notnull
    {
        var result = await context.ExecAsync(ct);
        return DeserializeResult<T>(result.StandardOutput, result);
    }

    public static async Task<T> ExecOutputOrDefaultAsync<T>(this LinuxCommandBuilderContext context, T? defaultValue, CancellationToken ct = default)
        where T : notnull
    {
        try
        {
            return await context.ExecOutputAsync<T>(ct);
        }
        catch
        {
            if (defaultValue != null)
                return defaultValue;
            return default(T)!;
        }

    }



}
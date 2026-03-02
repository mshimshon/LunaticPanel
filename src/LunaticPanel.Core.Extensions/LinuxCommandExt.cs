using LunaticPanel.Core.Abstraction.Tools.LinuxCommand;

namespace LunaticPanel.Core.Extensions;

public static class LinuxCommandExt
{
    public static LinuxCommandBuilderContext BuildCommand(this ILinuxCommand linuxCommand, string command)
        => new() { CommandBuilder = new(command), LinuxCommand = linuxCommand };
    public static LinuxCommandBuilderContext BuildBash(this ILinuxCommand linuxCommand, string file)
    => new() { CommandBuilder = new($"{Path.Combine("/", "bin", "bash")} {file}"), LinuxCommand = linuxCommand };

    public static LinuxCommandBuilderContext AsUser(this LinuxCommandBuilderContext context, string user)
        => context.UpdateBuilder(p => p with { RunAs = user });
    public static LinuxCommandBuilderContext Sudo(this LinuxCommandBuilderContext context)
        => context.UpdateBuilder(p => p with { Sudo = true });

    public static LinuxCommandBuilderContext PreserveEnvironmentVariable(this LinuxCommandBuilderContext context)
        => context.UpdateBuilder(p => p with { PerserveEnvironmentVariable = true });

    public static async Task<LinuxCommandResult> ExecAsync(this LinuxCommandBuilderContext context, CancellationToken ct = default)
        => await context.LinuxCommand.RunCommand(context.CommandBuilder, ct);



}
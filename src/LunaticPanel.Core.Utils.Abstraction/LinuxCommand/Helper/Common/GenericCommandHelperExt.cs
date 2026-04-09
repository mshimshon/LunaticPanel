using LunaticPanel.Core.Utils.Abstraction.LinuxCommand.Exceptions;
using System.Text.Json;

namespace LunaticPanel.Core.Utils.Abstraction.LinuxCommand.Helper.Common;

public static class GenericCommandHelperExt
{
    public static async Task<T> GetAsync<T>(this ILinuxCommand linuxCommand, string command, Action<LinuxCommandBuilderContext>? options = default, CancellationToken ct = default)
        where T : notnull
    {
        var builderContext = linuxCommand.BuildCommand(command);
        options?.Invoke(builderContext);
        var result = await builderContext.ExecAsync(ct);
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

    public static async Task<T> GetOrDefaultAsync<T>(this ILinuxCommand linuxCommand, string command, T? defaultValue, Action<LinuxCommandBuilderContext>? options = default, CancellationToken ct = default)
        where T : notnull
    {
        try
        {
            return await linuxCommand.GetAsync<T>(command, options, ct);
        }
        catch
        {
            if (defaultValue != null)
                return defaultValue;
            return default(T)!;
        }

    }
}

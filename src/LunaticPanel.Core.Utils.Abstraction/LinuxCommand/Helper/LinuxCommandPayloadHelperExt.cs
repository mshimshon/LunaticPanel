namespace LunaticPanel.Core.Utils.Abstraction.LinuxCommand.Helper;

public static class LinuxCommandPayloadHelperExt
{
    public static async Task<bool> BoolCommandAsync(this ILinuxCommand linuxCommand, string command, CancellationToken ct = default)
    {
        var result = await linuxCommand
               .BuildCommand(command)
               .AndPrintPayload(bool.TrueString)
               .OrPrintPayload(bool.FalseString)
               .ExecPayloadAsync<bool>(ct);
        return result;
    }
}

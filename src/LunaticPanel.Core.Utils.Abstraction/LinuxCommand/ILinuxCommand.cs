namespace LunaticPanel.Core.Utils.Abstraction.LinuxCommand;

public interface ILinuxCommand
{
    Task<LinuxCommandResult> RunCommand(LinuxCommandBuilder builder, CancellationToken ct = default);
}

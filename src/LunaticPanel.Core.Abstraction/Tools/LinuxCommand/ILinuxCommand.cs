namespace LunaticPanel.Core.Abstraction.Tools.LinuxCommand;

public interface ILinuxCommand
{
    Task<LinuxCommandResult> RunCommand(LinuxCommandBuilder builder, CancellationToken ct = default);
}

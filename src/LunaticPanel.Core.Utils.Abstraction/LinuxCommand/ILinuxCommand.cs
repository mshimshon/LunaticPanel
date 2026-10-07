namespace LunaticPanel.Core.Utils.Abstraction.LinuxCommand;

public interface ILinuxCommand
{
    Task<ILinuxCommandResult> RunCommand(LinuxCommandBuilder builder, CancellationToken ct = default);

}


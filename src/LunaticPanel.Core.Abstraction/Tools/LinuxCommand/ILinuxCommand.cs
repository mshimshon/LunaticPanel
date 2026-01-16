namespace LunaticPanel.Core.Abstraction.Tools.LinuxCommand;

public interface ILinuxCommand
{
    Task<LinuxCommandResult> RunLinuxScript(string file, bool sudo = true);
    Task<LinuxCommandResult> RunLinuxCommand(string command, bool sudo = true);
}

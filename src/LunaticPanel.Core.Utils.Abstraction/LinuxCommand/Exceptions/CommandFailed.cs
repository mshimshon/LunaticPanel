namespace LunaticPanel.Core.Utils.Abstraction.LinuxCommand.Exceptions;

public class CommandFailed : Exception
{

    public CommandFailed(LinuxCommandResult linuxCommandResult, string message) : base(message)
    {
        LinuxCommandResult = linuxCommandResult;

    }
    public CommandFailed(LinuxCommandResult linuxCommandResult) : this(linuxCommandResult, "Linux Command has failed.")
    {
    }

    public LinuxCommandResult LinuxCommandResult { get; }
}

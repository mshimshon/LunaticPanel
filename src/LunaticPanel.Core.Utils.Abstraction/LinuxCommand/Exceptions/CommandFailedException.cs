namespace LunaticPanel.Core.Utils.Abstraction.LinuxCommand.Exceptions;

public class CommandFailedException : Exception
{

    public CommandFailedException(string stdOut, string stdErr, string message) : base(message)
    {
        StdOut = stdOut;
        StdErr = stdErr;
    }
    public CommandFailedException(string stdOut, string stdErr) : this(stdOut, stdErr, "Linux Command has failed.")
    {
    }
    public string StdOut { get; }
    public string StdErr { get; }
}

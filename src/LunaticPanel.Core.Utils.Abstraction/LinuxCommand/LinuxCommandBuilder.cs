using LunaticPanel.Core.Utils.Abstraction.Logging;

namespace LunaticPanel.Core.Utils.Abstraction.LinuxCommand;

public sealed record LinuxCommandBuilder(string Command) : ILinuxCommandBuilderOptions
{


    public IReadOnlyCollection<LinuxSubCommandDescriptor>? SubCommands { get; init; }
    public bool BashFile { get; init; }
    public Func<string, Task>? OnStantardOutput { get; init; }
    public Func<string, Task>? OnErrorOutput { get; init; }
    public bool AutoCleanConsoleStream { get; init; }
    public bool Sudo { get; init; }
    public bool PerserveEnvironmentVariable { get; init; }
    public string? RunAs { get; init; }
    public string? WorkingDirectory { get; init; }
    public ICrazyReport? CrazyReportOverride { get; init; }
    public override string ToString()
    {
        bool runWithSpecificUser = RunAs != default;
        string preservedEnvironmentVariable = PerserveEnvironmentVariable ? "-E" : "";

        if (Sudo)
        {
            if (runWithSpecificUser) return $"sudo -u \"{RunAs}\" {preservedEnvironmentVariable} -- {BuildCommandsIntoLine()}";
            return $"sudo {preservedEnvironmentVariable} -- {BuildCommandsIntoLine()}";
        }
        if (runWithSpecificUser) return $"runuser -u \"{RunAs}\" {preservedEnvironmentVariable} -- bash -lc '{BuildCommandsIntoLine()}'";
        // runuser -u USERNAME -- COMMAND

        return $"{BuildCommandsIntoLine()}";
    }

    private string BuildCommandsIntoLine()
    {
        string inlineCommands = Command;
        if (SubCommands != default && SubCommands.Count > 0)
            inlineCommands += " " + string.Join(' ', SubCommands.Select(ProcessSubCommandsToString));

        return inlineCommands;
    }
    private string ProcessSubCommandsToString(LinuxSubCommandDescriptor d)
    {
        var toStr = Sudo ? d with { Command = $"sudo {d.Command}" } : d;
        return toStr.ToString();
    }
}


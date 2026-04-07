namespace LunaticPanel.Core.Utils.Abstraction.LinuxCommand;

public sealed record LinuxCommandBuilder(string Command)
{
    public bool Sudo { get; init; }
    public bool PerserveEnvironmentVariable { get; init; }
    public string? RunAs { get; init; }
    public Func<string, Task>? OnStantardOutput { get; init; }
    public Func<string, Task>? OnErrorOutput { get; init; }
    public override string ToString()
    {
        bool runWithSpecificUser = RunAs != default;
        string preservedEnvironmentVariable = PerserveEnvironmentVariable ? "-E" : "";
        if (Sudo)
        {
            if (runWithSpecificUser) return $"-c \"sudo -u \"{RunAs}\" {preservedEnvironmentVariable} -- {Command}\"";
            return $"-c \"sudo {preservedEnvironmentVariable} -- {Command}\"";
        }
        if (runWithSpecificUser) return $"-c \"runuser -u \"{RunAs}\" {preservedEnvironmentVariable} -- {Command}\"";
        // runuser -u USERNAME -- COMMAND

        return $"-c \"{Command}\"";
    }
}


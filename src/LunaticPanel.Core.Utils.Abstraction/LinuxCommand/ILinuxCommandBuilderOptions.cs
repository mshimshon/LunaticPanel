using LunaticPanel.Core.Utils.Abstraction.Logging;

namespace LunaticPanel.Core.Utils.Abstraction.LinuxCommand;

public interface ILinuxCommandBuilderOptions
{
    bool BashFile { get; }
    Func<string, Task>? OnStantardOutput { get; }
    Func<string, Task>? OnErrorOutput { get; }
    bool AutoCleanConsoleStream { get; }
    bool Sudo { get; }
    bool PerserveEnvironmentVariable { get; }
    string? RunAs { get; }
    string? WorkingDirectory { get; }
    ICrazyReport? CrazyReportOverride { get; }
}

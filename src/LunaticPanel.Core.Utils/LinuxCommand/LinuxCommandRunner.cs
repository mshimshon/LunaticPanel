using LunaticPanel.Core.Utils.Abstraction.LinuxCommand;
using LunaticPanel.Core.Utils.Abstraction.Logging;

namespace LunaticPanel.Core.Utils.LinuxCommand;


public class LinuxCommandRunner : ILinuxCommand
{
    private readonly string _tmpPath;
    private readonly string _bash;
    private ICrazyReport _crazyReport;

    public LinuxCommandRunner(ICrazyReport<LinuxCommandRunner> crazyReport)
    {
        _bash = Path.Combine("/", "bin", "bash");
        _tmpPath = Path.Combine("/", "tmp", "lunaticpanel", "linuxcommands");
        if (!Directory.Exists(_tmpPath))
            if (OperatingSystem.IsLinux())
            {
                var perm755 = UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute |
                    UnixFileMode.GroupRead | UnixFileMode.GroupExecute |
                    UnixFileMode.OtherRead | UnixFileMode.OtherExecute;
                Directory.CreateDirectory(_tmpPath, perm755);
            }

        _crazyReport = crazyReport;

    }

    public async Task<ILinuxCommandResult> RunCommand(LinuxCommandBuilder builder, CancellationToken ct = default)
    {
        var spawn = new LinuxCommandSpawn(builder, _crazyReport);
        return await spawn.RunCommandAsync(ct);
    }

}
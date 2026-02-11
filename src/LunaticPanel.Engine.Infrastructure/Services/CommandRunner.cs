using LunaticPanel.Core.Abstraction.Tools.LinuxCommand;
using System.Diagnostics;
using System.Text;

namespace LunaticPanel.Engine.Infrastructure.Services;


[System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
public class CommandRunner : ILinuxCommand
{
    private readonly string _tmpPath;
    private readonly string _bash;
    public CommandRunner()
    {

        var perm755 = UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute |
    UnixFileMode.GroupRead | UnixFileMode.GroupExecute |
    UnixFileMode.OtherRead | UnixFileMode.OtherExecute;
        _tmpPath = Path.Combine("/", "tmp", "lunaticpanel");
        if (!Directory.Exists(_tmpPath))
            Directory.CreateDirectory(_tmpPath, perm755);
        _bash = Path.Combine("/", "bin", "bash");

    }

    public async Task<LinuxCommandResult> RunLinuxScript(string file, bool sudo = true, CancellationToken ct = default)
    {
        var psi = new ProcessStartInfo
        {
            FileName = _bash,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
            StandardOutputEncoding = Encoding.UTF8,
            StandardErrorEncoding = Encoding.UTF8,
            Arguments = sudo ? $"-c \"sudo -E -- {_bash} {file}\"" : $"-c \"{_bash} {file}\""
        };
        Console.WriteLine($"{nameof(RunLinuxCommand)} Running: {psi.FileName} {psi.Arguments}");

        using var process = new Process
        {
            StartInfo = psi,
            EnableRaisingEvents = true
        };
        process.Start();
        var stdoutTask = process.StandardOutput.ReadToEndAsync(ct);
        var stderrTask = process.StandardError.ReadToEndAsync(ct);
        await process.WaitForExitAsync(ct).ConfigureAwait(false);


        string output = await stdoutTask;
        string error = await stderrTask;
        //Console.WriteLine($"{nameof(RunLinuxCommand)} | Standard Output : {output}");
        //Console.WriteLine($"{nameof(RunLinuxCommand)} | Error Output : {error}");

        var result = new LinuxCommandResult()
        {
            StandardError = error,
            StandardOutput = output
        };
        Console.WriteLine($"{nameof(RunLinuxCommand)} Result: {result.ToString()}");
        return result;
    }

    public async Task<LinuxCommandResult> RunLinuxCommand(string command, bool sudo = true, CancellationToken ct = default)
    {
        var psi = new ProcessStartInfo
        {
            FileName = _bash,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
            StandardOutputEncoding = Encoding.UTF8,
            StandardErrorEncoding = Encoding.UTF8,
            Arguments = sudo ? $"-c \"sudo -E -- {command}\"" : $"-c \"{command}\""
        };
        Console.WriteLine($"{nameof(RunLinuxCommand)} Running: {psi.FileName} {psi.Arguments}");
        using var process = new Process { StartInfo = psi, EnableRaisingEvents = true };
        process.Start();

        var stdoutTask = process.StandardOutput.ReadToEndAsync(ct);
        var stderrTask = process.StandardError.ReadToEndAsync(ct);
        await process.WaitForExitAsync(ct).ConfigureAwait(false);
        string output = await stdoutTask;
        string error = await stderrTask;
        var result = new LinuxCommandResult()
        {
            StandardError = error,
            StandardOutput = output
        };
        Console.WriteLine($"{nameof(RunLinuxCommand)} Result: {result.ToString()}");
        return result;
    }

}
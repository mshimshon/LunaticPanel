using LunaticPanel.Core.Utils.Abstraction.LinuxCommand;
using System.Diagnostics;
using System.Text;

namespace LunaticPanel.Core.Utils.LinuxCommand;


public class CommandRunner : ILinuxCommand
{
    private readonly string _tmpPath;
    private readonly string _bash;
    public CommandRunner()
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

    }

    private async Task PipeDrain(StreamReader stream, Func<string, CancellationToken, Task> onStream, Action<Exception>? onException = default, CancellationToken ct = default)
    {
        try
        {
            while (!ct.IsCancellationRequested)
            {
                var line = await stream.ReadLineAsync().ConfigureAwait(false);
                if (line is null) break;
                await onStream(line, ct);
            }
        }
        catch (Exception ex)
        {
            onException?.Invoke(ex);
        }
    }

    private Task PipeProcessor(StreamReader stream, Action<string> updateString, Func<string, Task>? informOutsider, CancellationToken ct = default)
    {
        return Task.Run(async () => await PipeDrain(stream, async (line, ct) =>
        {
            updateString.Invoke(line);
            if (informOutsider != default)
                await informOutsider.Invoke(line);
            Console.WriteLine($"CommandRunner: {line}"); // TODO: LOG
        }, default, ct), ct);
    }

    private async Task<LinuxCommandResult> Exec(ProcessStartInfo psi, Func<string, Task>? onStdOut, Func<string, Task>? onErrorOut, CancellationToken ct = default)
    {
        Console.WriteLine($"{nameof(Exec)} Running: {psi.FileName} {psi.Arguments}"); // TODO: LOG

        using var process = new Process
        {
            StartInfo = psi,
            EnableRaisingEvents = true
        };
        process.Start();
        var stdoutSb = new StringBuilder();
        var stderrSb = new StringBuilder();
        var pumpStdout = PipeProcessor(process.StandardOutput, line => stdoutSb.AppendLine(line), onStdOut, ct);
        var pumpStderr = PipeProcessor(process.StandardError, line => stderrSb.AppendLine(line), onErrorOut, ct);

        try
        {

            await process.WaitForExitAsync(ct).ConfigureAwait(false);
            await Task.WhenAll(pumpStdout, pumpStderr);
        }
        catch (OperationCanceledException)
        {
            try
            {
                if (!process.HasExited) process.Kill(entireProcessTree: true);
            }
            catch
            {

            }
            return new LinuxCommandResult() { StandardError = "OperationCanceledException", };
        }

        string output = stdoutSb.ToString();
        string error = stderrSb.ToString();
        var result = new LinuxCommandResult()
        {
            StandardError = error,
            StandardOutput = output
        };
        return result;

    }

    public async Task<LinuxCommandResult> RunCommand(LinuxCommandBuilder builder, CancellationToken ct = default)
         => await Exec(new ProcessStartInfo
         {
             FileName = _bash,
             UseShellExecute = false,
             RedirectStandardOutput = true,
             RedirectStandardError = true,
             CreateNoWindow = true,
             StandardOutputEncoding = Encoding.UTF8,
             StandardErrorEncoding = Encoding.UTF8,
             Arguments = builder.ToString(),
             WorkingDirectory = builder.WorkingDirectory
         }, builder.OnStantardOutput, builder.OnErrorOutput);
}
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

    private Task PipeProcessor(StreamReader stream, Action<StringBuilder> updateString, CancellationToken ct = default)
    {
        var localString = new StringBuilder();

        return Task.Run(async () => await PipeDrain(stream, (line, ct) =>
        {
            localString.AppendLine(line);
            updateString.Invoke(localString);
            Console.WriteLine($"CommandRunner: {line}"); // TODO: LOG
            return Task.CompletedTask;
        }, default, ct), ct);
    }

    private async Task<LinuxCommandResult> Exec(ProcessStartInfo psi, CancellationToken ct = default)
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
        var pumpStdout = PipeProcessor(process.StandardOutput, builder => stdoutSb = builder, ct);
        var pumpStderr = PipeProcessor(process.StandardError, builder => stderrSb = builder, ct);

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
        //Console.WriteLine($"{nameof(RunLinuxCommand)} | Standard Output : {output}");
        //Console.WriteLine($"{nameof(RunLinuxCommand)} | Error Output : {error}");

        var result = new LinuxCommandResult()
        {
            StandardError = error,
            StandardOutput = output
        };
        //Console.WriteLine($"{nameof(RunLinuxCommand)} Result: {result.ToString()}"); // TODO: LOG
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
             Arguments = builder.ToString()
         });
}
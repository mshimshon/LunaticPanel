using LunaticPanel.Core.Utils.Abstraction.LinuxCommand;
using LunaticPanel.Core.Utils.Abstraction.LinuxCommand.Helper;
using LunaticPanel.Core.Utils.Abstraction.Logging;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace LunaticPanel.Core.Utils.LinuxCommand;

internal sealed class LinuxCommandSpawn
{
    private readonly string _tmpPath;
    private readonly string _bash;
    private ICrazyReport _crazyReport;
    private ILinuxCommandBuilderOptions _options;
    private readonly string _command;
    private readonly CancellationTokenSource _drainSource = new CancellationTokenSource();
    private const string END_COMMAND_MARKER = "+-+-+-+END_COMMAND+-+-+-+";
    private const string END_COMMAND_SEQUENCE = $"+-+-+-+{{0}}{END_COMMAND_MARKER}{{1}}+-+-+-+";
    private const string END_COMMAND_REGEX = @"\+\-\+\-\+\-\+([0-9a-fA-F\-]{36})\+\-\+\-\+\-\+END_COMMAND\+\-\+\-\+\-\+(\d+)\+\-\+\-\+\-\+";

    public LinuxCommandSpawn(LinuxCommandBuilder builder, ICrazyReport crazyReport)
    {
        _bash = Path.Combine("/", "bin", "bash");
        _tmpPath = CreateAndDefineTmpPath();
        _options = builder;
        _command = builder.ToString();
        _crazyReport = crazyReport;
        if (_options.CrazyReportOverride != default)
            _crazyReport = _options.CrazyReportOverride;

    }
    private string CreateAndDefineTmpPath()
    {
        var path = Path.Combine("/", "tmp", "lunaticpanel", "linuxcommands");
        if (!Directory.Exists(path))
            if (OperatingSystem.IsLinux())
            {
                var perm755 = UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute |
                    UnixFileMode.GroupRead | UnixFileMode.GroupExecute |
                    UnixFileMode.OtherRead | UnixFileMode.OtherExecute;
                Directory.CreateDirectory(path, perm755);
            }
        return path;
    }

    private async Task PipeDrain(StreamReader stream, Func<string, CancellationToken, Task> onStream, Action<Exception>? onException = default, CancellationToken ct = default)
    {

        while (!ct.IsCancellationRequested)
        {
            try
            {
                var line = await stream.ReadLineAsync().ConfigureAwait(false);
                if (string.IsNullOrWhiteSpace(line)) continue;
                if (_options.AutoCleanConsoleStream)
                    line = line.CleanFromColorCodes();
                await onStream(line, ct);
            }
            catch (Exception ex)
            {
                _crazyReport.ReportErrorException(ex.Message, ex);
                onException?.Invoke(ex);
            }

        }

    }

    private Task PipeProcessor(StreamReader stream, Action<string> updateString, Func<string, Task>? informOutsider, CancellationToken ct = default)
    {
        return Task.Run(async () => await PipeDrain(stream, async (line, token) =>
        {
            updateString.Invoke(line);
            if (informOutsider != default)
                await informOutsider.Invoke(line);
        }, default, ct), ct);
    }

    // +-+-+-+END_COMMAND+-+-+-+ID+-+-+-+EXITCODE+-+-+-+  
    private bool CheckCommandEnd(string line, out int exitCode, out Guid id)
    {
        exitCode = int.MinValue;
        id = Guid.Empty;
        if (string.IsNullOrWhiteSpace(line)) return false;
        bool endMarkerFound = line.Contains(END_COMMAND_MARKER);
        bool isFalsePositive = line.Contains("$rc\"");
        if (isFalsePositive) return false;
        var regex = new Regex(END_COMMAND_REGEX, RegexOptions.Compiled);
        var match = regex.Match(line);
        if (endMarkerFound && match.Success)
        {

            string guid = match.Groups[1].Value;
            id = Guid.Parse(guid);
            if (int.TryParse(match.Groups[2].Value, out int result))
                exitCode = result;
            return true;
        }


        return false;
    }
    private bool KillOnCommandEnd(Process process, string line, out int exitCode, out Guid id)
    {
        exitCode = int.MinValue;
        id = Guid.Empty;
        if (CheckCommandEnd(line, out int result, out Guid resultId))
        {
            exitCode = result;
            id = resultId;
            _crazyReport.Report("END COMMAND DETECTED, KILL BASH.");
            return true;
        }
        return false;
    }


    private async Task<LinuxCommandResult> Exec(ProcessStartInfo psi, CancellationToken ct = default)
    {
        var drainCts = _drainSource;
        string cmd = _command;
        _crazyReport.ReportInfo(cmd);
        Guid sequenceId = Guid.NewGuid();
        string endSequence = string.Format(END_COMMAND_SEQUENCE, sequenceId, "$rc");
        string readyCmd = $"({cmd} ); rc=$?; echo \"{endSequence}\"";
        _crazyReport.Report(readyCmd);
        using var process = new Process
        {
            StartInfo = psi,
            EnableRaisingEvents = true,
        };


        process.Start();
        process.StandardInput.NewLine = "\n";
        var stdoutList = new List<string>();
        var stderrList = new List<string>();
        var stdoutSb = new StringBuilder();
        var stderrSb = new StringBuilder();
        int exitCode = int.MinValue;
        var pumpStdout = PipeProcessor(process.StandardOutput, line =>
        {
            Console.Out.WriteLine(line);
            if (ct.IsCancellationRequested)
                drainCts.Cancel();
            bool endDetected = KillOnCommandEnd(process, line, out int code, out Guid id);
            if (endDetected && id == sequenceId)
            {

                exitCode = code;
                drainCts.Cancel();
                return;
            }

            stdoutList.Add(line);
            stdoutSb.AppendLine(line);
        }, _options.OnStantardOutput, drainCts.Token);

        var pumpStderr = PipeProcessor(process.StandardError, line =>
        {
            Console.Error.WriteLine(line);
            if (ct.IsCancellationRequested)
                drainCts.Cancel();
            bool endDetected = KillOnCommandEnd(process, line, out int code, out Guid id);
            if (endDetected && id == sequenceId)
            {
                exitCode = code;
                drainCts.Cancel();
                return;
            }
            stderrList.Add(line);
            stderrSb.AppendLine(line);
        }, _options.OnErrorOutput, drainCts.Token);



        try
        {
            _crazyReport.Report("Patching Through Command.");
            await process.StandardInput.WriteLineAsync(readyCmd);
            _crazyReport.Report("Flushing.");
            await process.StandardInput.FlushAsync();
            process.StandardInput.Close();

            _crazyReport.Report("Waiting the Command Execution.");
            await process.WaitForExitAsync(ct).ConfigureAwait(false);
            _crazyReport.Report("Awaiting Pipe Drain from Executed Command.");
            await Task.WhenAll(pumpStdout, pumpStderr);
            _crazyReport.Report("Pipe was drained.");
        }
        catch (OperationCanceledException)
        {
            _crazyReport.ReportInfo("{0} {1} (Cancelled)", psi.FileName, psi.Arguments);
            try
            {
                if (!process.HasExited) process.Kill(entireProcessTree: true);
            }
            catch
            {

            }
            return new LinuxCommandResult()
            {
                ExitCode = exitCode,
                RawStandardError = new List<string>() { nameof(OperationCanceledException) },
            };
        }
        catch (Exception ex)
        {
            _crazyReport.ReportError("{0} {1} = {2}", ex, psi.FileName, psi.Arguments, ex.Message);
            return new LinuxCommandResult()
            {
                ExitCode = exitCode,
                RawStandardError = new List<string>() { "Unknown Error with Linux Command (See Log)" },
            };

        }

        string output = stdoutSb.ToString();
        string error = stderrSb.ToString();
        var result = new LinuxCommandResult(output)
        {
            ExitCode = exitCode,
            RawStandardError = stderrList,
            RawStandardOutput = stdoutList,
            StandardError = error
        };
        return result;

    }

    public async Task<ILinuxCommandResult> RunCommandAsync(CancellationToken ct = default)
    {
        var looseUtf8 = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: false);
        var _psi = new ProcessStartInfo
        {
            //FileName = "socat",
            FileName = _bash,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardInput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
            StandardInputEncoding = new UTF8Encoding(false),
            StandardOutputEncoding = looseUtf8,
            StandardErrorEncoding = looseUtf8,
            Arguments = "-s",
            WorkingDirectory = _options.WorkingDirectory
        };
        // script -qec "/bin/bash -s" /dev/null
        return await Exec(_psi);
    }
}

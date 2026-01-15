using LunaticPanel.Core.Abstraction.Tools;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

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
    public async Task<TResponse> RunLinuxScriptWithReplyAs<TResponse>(string file, bool sudo = true)
    {
        string response = await RunLinuxScript(file, sudo);
        return JsonSerializer.Deserialize<TResponse>(response)!;
    }

    public async Task<string> RunLinuxScript(string file, bool sudo = true)
    {
        var psi = new ProcessStartInfo
        {

            FileName = sudo ? "sudo" : _bash,

            Arguments = $"{(sudo ? _bash : "")} -c \"{file}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        using var process = new Process { StartInfo = psi };

        var stdout = new StringBuilder();
        var stderr = new StringBuilder();

        process.OutputDataReceived += (_, e) => { if (e.Data != null) stdout.AppendLine(e.Data); };
        process.ErrorDataReceived += (_, e) => { if (e.Data != null) stderr.AppendLine(e.Data); };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        process.WaitForExit();

        string output = stdout.ToString();
        string error = stderr.ToString();

        return output + error;
    }

    public async Task<string> RunLinuxCommand(string command, bool sudo = true)
    {
        var commandTorite = command;
        if (sudo) commandTorite = "sudo " + command;
        var script = Path.Combine(_tmpPath, Path.GetRandomFileName()) + ".sh";
        await File.WriteAllTextAsync(script, commandTorite);

        var psi = new ProcessStartInfo
        {
            FileName = _bash,
            Arguments = script,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        using var process = new Process { StartInfo = psi };

        var stdout = new StringBuilder();
        var stderr = new StringBuilder();

        process.OutputDataReceived += (_, e) => { if (e.Data != null) stdout.AppendLine(e.Data); };
        process.ErrorDataReceived += (_, e) => { if (e.Data != null) stderr.AppendLine(e.Data); };

        process.Start();

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        process.WaitForExit();

        string output = stdout.ToString();
        string error = stderr.ToString();
        File.Delete(script);
        return output + error;
    }

}
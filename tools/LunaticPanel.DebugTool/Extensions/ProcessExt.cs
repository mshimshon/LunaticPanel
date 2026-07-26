using System.Diagnostics;
using System.Text;

namespace LunaticPanel.DebugTool.Extensions;

internal static class ProcessExt
{
    public static async Task<string> RunProcessAsync(string fileName, string arguments)
    {
        var psi = new ProcessStartInfo
        {
            FileName = fileName,
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = new Process { StartInfo = psi };
        process.Start();

        var stdoutBuilder = new StringBuilder();
        var stderrBuilder = new StringBuilder();

        var stdoutTask = Task.Run(async () =>
        {
            string? line;
            while ((line = await process.StandardOutput.ReadLineAsync()) != null)
            {
                stdoutBuilder.AppendLine(line);
                Console.WriteLine(line);
            }
        });

        var stderrTask = Task.Run(async () =>
        {
            string? line;
            while ((line = await process.StandardError.ReadLineAsync()) != null)
            {
                stderrBuilder.AppendLine(line);
                Console.Error.WriteLine(line);
            }
        });

        await Task.WhenAll(stdoutTask, stderrTask);
        process.WaitForExit();

        if (process.ExitCode != 0)
            throw new Exception($"Process '{fileName} {arguments}' failed with exit code {process.ExitCode}.");

        return stdoutBuilder.ToString();
    }
}

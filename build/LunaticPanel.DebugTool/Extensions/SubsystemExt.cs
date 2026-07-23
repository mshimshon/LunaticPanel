using System.Diagnostics;
using System.Text.RegularExpressions;

namespace LunaticPanel.DebugTool.Extensions;

internal static class SubsystemExt
{
    private static string CleanOutput(string s)
    {
        // Remove ANSI escape sequences like ESC[0m, ESC[32m, etc.
        s = Regex.Replace(s, @"\x1B

\[[0-9;]*[A-Za-z]", "");

        // Remove stray ESC
        s = s.Replace("\u001B", "");

        // Remove BOM
        s = s.Replace("\uFEFF", "");

        // Remove NULL
        s = s.Replace("\u0000", "");

        // Remove any leftover control chars
        s = Regex.Replace(s, @"[\x00-\x1F\x7F]", "");

        return s.Trim();
    }
    public static async Task<bool> WslDistroExists(string distroName)
    {
        string output = await PrintDistros();
        output = output.Replace("\r", "");
        var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries).Select(p => CleanOutput(p).Trim())
            .Where(p => !string.IsNullOrWhiteSpace(p)).Where(x => x.Length > 0);
        return lines.Any(d => d.Equals(distroName, StringComparison.OrdinalIgnoreCase));
    }

    public static async Task<string> PrintDistros()
    {
        Console.Out.WriteLine("Available WSL");
        return await ProcessExt.RunProcessAsync("wsl.exe", "-l -q");
    }

    public static async Task InstallOfficialDebian()
    {
        Console.Out.WriteLine($"Installing Fresh Debian.");
        await ProcessExt.RunProcessAsync("wsl.exe", "--install -d Debian --no-launch");
    }

    public static async Task ExportAsync(string name, string tarPath)
    {
        Console.Out.WriteLine($"Exporting {name} to {tarPath}.");
        await ProcessExt.RunProcessAsync("wsl.exe", $"--export {name} \"{tarPath}\"");
    }

    public static async Task ShudownAsync(string name)
    {
        Console.Out.WriteLine($"Shutdown of {name}.");

        await ProcessExt.RunProcessAsync("wsl.exe", $"--shutdown");
    }

    public static async Task StartAsync(string name)
    {
        Console.Out.WriteLine($"Starting {name}.");
        await ProcessExt.RunProcessAsync("wsl.exe", $"-d  {name} --user root -- echo {name} has started");
    }

    public static async Task DestroyAsync(string name)
    {
        Console.Out.WriteLine($"Destroying {name}.");
        await ProcessExt.RunProcessAsync("wsl.exe", $"--unregister {name}");
    }

    public static async Task ImportAsync(string name, string installDir, string tarPath)
    {
        Console.Out.WriteLine($"Importing {name} -> '{tarPath}' -> '{installDir}'.");
        if (Directory.Exists(installDir))
            Directory.Delete(installDir, true);
        Directory.CreateDirectory(installDir);
        await ProcessExt.RunProcessAsync("wsl.exe", $"--import {name} \"{installDir}\" \"{tarPath}\"");
    }
    public static async Task<string> RunAsync(string distro, string command)
    {
        Console.Out.WriteLine($"Send command {distro} -> {command}");
        return await ProcessExt.RunProcessAsync("wsl.exe", $"-d {distro} --user root -- bash -c  \"{command}\"");
    }

    public static Process ShowServiceAsync(string distro, string serviceName)
    {
        string cmd = $"wsl -d {distro} --user root -- bash -c 'tail -f -n 100 /var/log/{serviceName}.stdout.log /var/log/{serviceName}.stderr.log'";
        return Process.Start(new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = $"/k \"{cmd}\"",
            UseShellExecute = true,
            CreateNoWindow = false,
            WindowStyle = ProcessWindowStyle.Normal
        })!;
    }



    public static async Task CopyFileAsync(string distro, string winPath, string wslPath)
    {
        var wslSource = winPath
            .Replace("C:\\", "/mnt/c/")
            .Replace("\\", "/");

        Console.Out.WriteLine($"Copying File {wslSource} -> {wslPath}");
        await RunAsync(distro, $"install -d \"$(dirname '{wslPath}')\" && cp '{wslSource}' '{wslPath}'");
    }
    public static async Task CopyDirAsync(string distro, string winPath, string wslPath)
    {
        var wslSource = winPath
            .Replace("C:\\", "/mnt/c/")
            .Replace("\\", "/");
        Console.Out.WriteLine($"Copying Folder {wslSource} -> {wslPath}");

        await RunAsync(distro, $"install -d '{wslPath}' && cp -a '{wslSource}/.' '{wslPath}'");
        await RunAsync(distro, $"ls '{wslPath}'");
    }
}

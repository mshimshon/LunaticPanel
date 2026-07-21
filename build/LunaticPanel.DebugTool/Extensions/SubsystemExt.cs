namespace LunaticPanel.DebugTool.Extensions;

internal static class SubsystemExt
{
    public static async Task<bool> WslDistroExists(string distroName)
    {
        string output = await ProcessExt.RunProcessAsync("wsl.exe", "-l -q");
        return output
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Any(d => d.Trim().Equals(distroName, StringComparison.OrdinalIgnoreCase));
    }

    public static async Task InstallOfficialDebian()
    {
        await ProcessExt.RunProcessAsync("wsl.exe", "--install -d Debian --no-launch");
    }

    public static async Task ExportAsync(string name, string tarPath)
    {
        await ProcessExt.RunProcessAsync("wsl.exe", $"--export {name} \"{tarPath}\"");
    }

    public static async Task ImportAsync(string name, string installDir, string tarPath)
    {
        Directory.CreateDirectory(installDir);
        await ProcessExt.RunProcessAsync("wsl.exe", $"--import {name} \"{installDir}\" \"{tarPath}\"");
    }
    public static async Task<string> RunAsync(string distro, string command)
    {
        return await ProcessExt.RunProcessAsync("wsl.exe", $"-d {distro} {command}");
    }

    public static async Task CopyFileAsync(string distro, string winPath, string wslPath)
    {
        var wslSource = winPath
            .Replace("C:\\", "/mnt/c/")
            .Replace("\\", "/");

        await RunAsync(distro, $"cp \"{wslSource}\" \"{wslPath}\"");
    }
}

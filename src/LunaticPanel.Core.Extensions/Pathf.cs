using System;
using System.Formats.Tar;
using System.IO.Compression;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LunaticPanel.Core.Extensions;

/// <summary>
/// Built-in OS simulator, will place everything executing on non linux into TMP/LunaticPanel/Pathf/XXXXX/XXX/XXX.
/// Design for allowing Windows Execution to do unit testing.
/// </summary>
public static class Pathf
{
    public static string SimulatorBasePath { get; } = Path.Combine(Path.GetTempPath(), "lunaticpanel", "pathf");
    public static string DirectorySeparatorChar { get; } = Path.DirectorySeparatorChar.ToString();
    public static string AltDirectorySeparatorChar { get; } = Path.AltDirectorySeparatorChar.ToString();
    public static string VolumeSeparatorChar { get; } = Path.VolumeSeparatorChar.ToString();
    public static string PathSeparator { get; } = Path.PathSeparator.ToString();
    public static string EnsureCreated(string path)
    {
        bool isFile = Path.HasExtension(path);
        if (File.Exists(path)) return path;
        string dir = isFile ? Path.GetDirectoryName(path)! : path;
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);
        return path;
    }

    public static string EnsureCreated(string path, UnixFileMode unixCreateMode)
    {
        bool isFile = Path.HasExtension(path);
        if (File.Exists(path)) return path;
        string dir = isFile ? Path.GetDirectoryName(path)! : path;
        if (!Directory.Exists(dir))
            if (OperatingSystem.IsLinux())
                Directory.CreateDirectory(dir, unixCreateMode);
            else
                return EnsureCreated(path);
        return path;
    }

    public static string EnsureCreated(params string[] parts) => CombineEnsureCreated(parts);
    public static string Combine(params string[] parts)
    {
        string combined = Path.Combine(parts);
        // if path absolute start with / and not starting with simulation path and not on linux then simulate.
        if (!OperatingSystem.IsLinux() && combined.StartsWith(DirectorySeparatorChar) && !combined.StartsWith(SimulatorBasePath))
            combined = Path.Combine(SimulatorBasePath, combined.Substring(1, combined.Length - 1)); // Remove leading / to avoid breaking pathing

        return combined;
    }

    public static string CombineEnsureCreated(params string[] parts)
    {
        var combined = Combine(parts);
        return EnsureCreated(combined);
    }

    public static string CombineEnsureCreated(UnixFileMode unixCreateMode, params string[] parts)
    {
        var combined = Combine(parts);
        return EnsureCreated(combined, unixCreateMode);
    }
    /// <summary>
    /// Delete Directory and Recreate (Always Recursive)
    /// </summary>
    public static string ResetDirectory(UnixFileMode unixCreateMode, string path)
    {
        if (Directory.Exists(path))
            Directory.Delete(path, true);
        return EnsureCreated(path, unixCreateMode);
    }

    public static string ResetDirectory(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, true);
            if (!path.EndsWith(DirectorySeparatorChar))
                path += DirectorySeparatorChar;
        }

        return EnsureCreated(path);
    }


}

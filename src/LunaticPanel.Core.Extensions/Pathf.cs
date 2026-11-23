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

    /// <summary>
    /// Ensures the specified directory exists. When creating the directory on Linux,
    /// the optional Unix file mode is applied. Returns the resulting directory path.
    /// </summary>
    public static string EnsureCreated(string path, UnixFileMode? unixCreateMode = default)
    {

        if (!Directory.Exists(path))
            if (unixCreateMode != default && OperatingSystem.IsLinux())
                Directory.CreateDirectory(path, (UnixFileMode)unixCreateMode!);
            else
                Directory.CreateDirectory(path);

        return path;
    }

    /// <summary>
    /// Ensures the specified directory exists and returns a file path within it.
    /// A trailing directory separator is added when missing. The file itself is not created.
    /// </summary>
    public static string EnsureCreated(string path, string filename, UnixFileMode? unixCreateMode = default)
    {


        if (!Directory.Exists(path))
            if (unixCreateMode != default && OperatingSystem.IsLinux())
                Directory.CreateDirectory(path, (UnixFileMode)unixCreateMode!);
            else
                Directory.CreateDirectory(path);

        return Path.Combine(path, filename);
    }


    /// <summary>
    /// Combines path segments into a single path. Relative paths are returned
    /// unchanged. On non-Linux platforms, Unix-style absolute paths are mapped
    /// into the simulator directory.
    /// </summary>
    public static string Combine(params string[] parts)
    {
        string combined = Path.Combine(parts);
        // if path absolute start with / and not starting with simulation path and not on linux then simulate.
        if (!OperatingSystem.IsLinux() && combined.StartsWith(DirectorySeparatorChar) && !combined.StartsWith(SimulatorBasePath))
            combined = Path.Combine(SimulatorBasePath, combined.Substring(1, combined.Length - 1)); // Remove leading / to avoid breaking pathing

        return combined;
    }


    /// <summary>
    /// Combines path segments, ensures the resulting directory exists,
    /// and returns a ready-to-use path.
    /// </summary>
    public static string CombineReady(params string[] parts)
        => EnsureCreated(Combine(parts));

    /// <summary>
    /// Combines path segments, ensures the resulting directory exists using
    /// the specified Unix file mode, and returns a ready-to-use path.
    /// </summary>
    public static string CombineReady(UnixFileMode unixCreateMode, params string[] parts)
        => EnsureCreated(Combine(parts), unixCreateMode);

    /// <summary>
    /// Combines path segments, ensures the resulting directory exists,
    /// and returns a ready-to-use file path. The file itself is not created.
    /// </summary>
    public static string CombineReadyFile(string[] parts, string filename, UnixFileMode? unixCreateMode = null)
        => CombineReadyFile(Combine(parts), filename);

    /// <summary>
    /// Combines path segments, ensures the resulting directory exists,
    /// and returns a ready-to-use file path. The file itself is not created.
    /// </summary>
    public static string CombineReadyFile(string relatedPath, string filename, UnixFileMode? unixCreateMode = null)
        => EnsureCreated(relatedPath, filename);

    /// <summary>
    /// Combines path segments, ensures the resulting directory exists using
    /// the specified Unix file mode, and returns a ready-to-use file path.
    /// The file itself is not created.
    /// </summary>
    public static string CombineReadyFile(string[] parts, string filename, UnixFileMode unixCreateMode)
        => CombineReadyFile(Combine(parts), filename, unixCreateMode);

    /// <summary>
    /// Combines path segments, ensures the resulting directory exists using
    /// the specified Unix file mode, and returns a ready-to-use file path.
    /// The file itself is not created.
    /// </summary>
    public static string CombineReadyFile(string relatedPath, string filename, UnixFileMode unixCreateMode)
        => EnsureCreated(relatedPath, filename, unixCreateMode);




    /// <summary>
    /// Deletes the specified directory and all contents if it exists.
    /// </summary>
    public static void DeleteDir(string path)
    {
        if (Directory.Exists(path))
            Directory.Delete(path, true);
    }

    /// <summary>
    /// Deletes the specified file if it exists.
    /// </summary>
    public static void DeleteFile(string path)
    {
        if (File.Exists(path))
            File.Delete(path);
    }

    /// <summary>
    /// Deletes and recreates the specified directory using the provided
    /// Unix file mode. Returns the recreated directory path.
    /// </summary>
    public static string ResetDir(UnixFileMode unixCreateMode, string path)
    {
        DeleteDir(path);
        return EnsureCreated(path, unixCreateMode);
    }

    /// <summary>
    /// Deletes and recreates the specified directory. Returns the recreated
    /// directory path.
    /// </summary>
    public static string ResetDir(string path)
    {
        DeleteDir(path);

        return EnsureCreated(path);
    }

}

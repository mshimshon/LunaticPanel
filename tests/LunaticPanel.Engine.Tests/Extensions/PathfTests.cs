using LunaticPanel.Core.Extensions;
using System;

namespace LunaticPanel.Engine.Tests.Extensions;

[Collection("NoParallel")]

public class PathfTests : IDisposable
{
    private bool _disposedValue;


    [Fact]
    public void SimulationBase_ShouldPass()
    {
        var expected = Path.Combine([Pathf.DirectorySeparatorChar, "tmp", "lunaticpanel", "pathf"]);
        if (!OperatingSystem.IsLinux())
            expected = Path.Combine(Path.GetTempPath(), "lunaticpanel", "pathf");
        Assert.Equal(expected, Pathf.SimulatorBasePath);
        Assert.Equal(Path.AltDirectorySeparatorChar.ToString(), Pathf.AltDirectorySeparatorChar);
        Assert.Equal(Path.DirectorySeparatorChar.ToString(), Pathf.DirectorySeparatorChar);
        Assert.Equal(Path.PathSeparator.ToString(), Pathf.PathSeparator);
    }


    [Fact]
    public void Combine_ShouldPass()
    {
        var expected = Path.Combine([Pathf.DirectorySeparatorChar, "tmp", "my", "path"]);
        if (!OperatingSystem.IsLinux())
            expected = Path.Combine(Path.GetTempPath(), "lunaticpanel", "pathf", "tmp", "my", "path");
        var actual = Pathf.Combine(Pathf.DirectorySeparatorChar, "tmp", "my", "path");
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void CombineRelative_ShouldPass()
    {
        var expected = Path.Combine("my", "path");
        var actual = Pathf.Combine("my", "path");
        Assert.Equal(expected, actual);
    }


    [Fact]
    public void CombineReady_ShouldPass()
    {
        string[] targetPath = ["tmp", "my", "path"];
        var expected = Path.Combine([Pathf.DirectorySeparatorChar, .. targetPath]);
        if (!OperatingSystem.IsLinux())
            expected = Path.Combine([Path.GetTempPath(), "lunaticpanel", "pathf", .. targetPath]);
        var actual = Pathf.CombineReady([Pathf.DirectorySeparatorChar, .. targetPath]);
        Assert.Equal(expected, actual);
        Assert.True(Directory.Exists(expected));
    }


    [Fact]
    public void CombineReady_WithUnixPermission_ShouldPass()
    {
        // Pass Permission, Only Apply on Linux without Crashing on Windows
        var expected = Path.Combine(Pathf.DirectorySeparatorChar, "tmp", "my", "path");
        if (!OperatingSystem.IsLinux())
            expected = Path.Combine(Path.GetTempPath(), "lunaticpanel", "pathf", "tmp", "my", "path");
        var actual = Pathf.CombineReady(UnixFileMode.UserWrite | UnixFileMode.UserRead, Pathf.DirectorySeparatorChar, "tmp", "my", "path");
        Assert.Equal(expected, actual);
        Assert.True(Directory.Exists(expected));
    }


    [Fact]
    public void CombineReadyFile_WithUnixPermission_ShouldPass()
    {
        // Pass Permission, Only Apply on Linux without Crashing on Windows
        string[] targetPath = ["tmp", "my", "path"];
        var expected = Path.Combine([Pathf.DirectorySeparatorChar, .. targetPath, "file.me"]);
        if (!OperatingSystem.IsLinux())
            expected = Path.Combine([Path.GetTempPath(), "lunaticpanel", "pathf", .. targetPath, "file.me"]);
        var actual = Pathf.CombineReadyFile([Pathf.DirectorySeparatorChar, "tmp", "my", "path"], "file.me", UnixFileMode.UserWrite | UnixFileMode.UserRead);
        Assert.Equal(expected, actual);
        Assert.True(Directory.Exists(Pathf.Combine(targetPath)));
    }




    [Fact]
    public void EnsureCreatedFile_ShouldPass()
    {
        string[] args = [Pathf.DirectorySeparatorChar, "tmp", "my", "path", "filename.net"];
        if (!OperatingSystem.IsLinux())
            args = [Path.GetTempPath(), "lunaticpanel", "pathf", "tmp", "my", "path", "filename.net"];

        var expected = Pathf.Combine(args);
        var actual = Pathf.EnsureCreated(Path.GetDirectoryName(expected)!, Path.GetFileName(expected));
        var dir = Path.GetDirectoryName(expected);
        Assert.Equal(expected, actual);
        Assert.True(Directory.Exists(dir));
        Assert.False(File.Exists(expected));
    }


    [Fact]
    public void EnsureCreatedFileLikePath_ShouldPass()
    {
        string[] targetPath = ["tmp", "my", ".path"];
        var expected = Path.Combine([Pathf.DirectorySeparatorChar, .. targetPath]);
        if (!OperatingSystem.IsLinux())
            expected = Path.Combine([Path.GetTempPath(), "lunaticpanel", "pathf", .. targetPath]);

        var actual = Pathf.CombineReady([Pathf.DirectorySeparatorChar, .. targetPath]);
        Assert.Equal(expected, actual);
    }



    [Fact]
    public void RemoveExistingFile_ShouldPass()
    {


        string[] targetPath = ["tmp", "my", "path"];
        var filename = "filename.net";
        var expected = Path.Combine([Pathf.DirectorySeparatorChar, .. targetPath, filename]);
        if (!OperatingSystem.IsLinux())
            expected = Path.Combine([Path.GetTempPath(), "lunaticpanel", "pathf", .. targetPath, filename]);
        var actual = Pathf.CombineReadyFile([Pathf.DirectorySeparatorChar, .. targetPath], filename);
        Assert.True(Directory.Exists(Path.GetDirectoryName(actual)!));
        File.WriteAllText(expected, "whatever");
        Assert.True(File.Exists(actual));
        Pathf.DeleteFile(expected);
        Assert.False(File.Exists(expected));
    }


    [Fact]
    public void RemoveExistingFile_NonExistant_ShouldPass()
    {
        string[] targetPath = ["tmp", "my", "path"];
        var filename = "filename.net";
        var expected = Path.Combine([Pathf.DirectorySeparatorChar, .. targetPath, filename]);
        if (!OperatingSystem.IsLinux())
            expected = Path.Combine([Path.GetTempPath(), "lunaticpanel", "pathf", .. targetPath, filename]);
        var actual = Pathf.CombineReadyFile([Pathf.DirectorySeparatorChar, .. targetPath], filename);
        Assert.True(Directory.Exists(Path.GetDirectoryName(actual)!));
        Assert.False(File.Exists(actual));
        Pathf.DeleteFile(expected);
        Assert.False(File.Exists(expected));
    }

    [Fact]
    public void ResetDirectory_ShouldPass()
    {
        string[] targetPath = ["tmp", "my", ".path"];
        var expected = Path.Combine([Pathf.DirectorySeparatorChar, .. targetPath]);
        if (!OperatingSystem.IsLinux())
            expected = Path.Combine([Path.GetTempPath(), "lunaticpanel", "pathf", .. targetPath]);
        var file = Pathf.CombineReadyFile(expected, "file.net");
        File.WriteAllText(file, "whatever");
        Assert.True(Directory.Exists(expected));
        Assert.True(File.Exists(file));
        Pathf.ResetDir(expected);
        Assert.False(File.Exists(file));
        Assert.True(Directory.Exists(expected));
    }

    [Fact]
    public void ResetDirectory_WithUnixPermission_ShouldPass()
    {
        string[] targetPath = ["tmp", "my", ".path"];
        var expected = Path.Combine([Pathf.DirectorySeparatorChar, .. targetPath]);
        if (!OperatingSystem.IsLinux())
            expected = Path.Combine([Path.GetTempPath(), "lunaticpanel", "pathf", .. targetPath]);
        var file = Pathf.CombineReadyFile(expected, "file.net");
        File.WriteAllText(file, "whatever");
        Assert.True(Directory.Exists(expected));
        Assert.True(File.Exists(file));
        Pathf.ResetDir(UnixFileMode.UserWrite | UnixFileMode.UserRead, expected);
        Assert.False(File.Exists(file));
        Assert.True(Directory.Exists(expected));
    }

    [Fact]
    public void DeleteFile_ShouldPass()
    {
        string[] targetPath = ["tmp", "my", ".path"];
        var filename = "filename.net";
        var expected = Path.Combine([Pathf.DirectorySeparatorChar, .. targetPath, filename]);
        if (!OperatingSystem.IsLinux())
            expected = Path.Combine([Path.GetTempPath(), "lunaticpanel", "pathf", .. targetPath, filename]);
        var file = Pathf.CombineReadyFile([Pathf.DirectorySeparatorChar, .. targetPath], filename);
        Assert.Equal(expected, file);
        File.WriteAllText(file, "whatever");
        Assert.True(File.Exists(file));
        Pathf.DeleteFile(file);
        Assert.False(File.Exists(file));
    }


    [Fact]
    public void DeleteFile_NoFileNoCrash_ShouldPass()
    {
        string[] targetPath = ["tmp", "my", ".path"];
        var filename = "filename.net";
        var expected = Path.Combine([Pathf.DirectorySeparatorChar, .. targetPath, filename]);
        if (!OperatingSystem.IsLinux())
            expected = Path.Combine([Path.GetTempPath(), "lunaticpanel", "pathf", .. targetPath, filename]);
        var file = Pathf.CombineReadyFile([Pathf.DirectorySeparatorChar, .. targetPath], filename);
        Assert.Equal(expected, file);
        Assert.False(File.Exists(file));
        Pathf.DeleteFile(file);
        Assert.False(File.Exists(file));
    }



    private void CleanUpJob()
    {
        if (!OperatingSystem.IsLinux() && Directory.Exists(Pathf.SimulatorBasePath))
            Directory.Delete(Pathf.SimulatorBasePath, true);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                CleanUpJob();
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            _disposedValue = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~PluginDiscoveryTests()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}

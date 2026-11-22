using LunaticPanel.Core.Extensions;
using System;

namespace LunaticPanel.Engine.Tests.Extensions;

public class PathfTests
{
    [Fact]
    public void SimulationBase_ShouldPass()
    {
        var expected = "/tmp/lunaticpanel/pathf";
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
        var expected = "/tmp/my/path";
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
    public void CombineEnsureCreated_ShouldPass()
    {
        var expected = "/tmp/my/path";
        if (!OperatingSystem.IsLinux())
            expected = Path.Combine(Path.GetTempPath(), "lunaticpanel", "pathf", "tmp", "my", "path");
        var actual = Pathf.CombineEnsureCreated(Pathf.DirectorySeparatorChar, "tmp", "my", "path");
        Assert.Equal(expected, actual);
        Assert.True(Directory.Exists(expected));
    }


    [Fact]
    public void CombineEnsureCreated_WithUnixPermission_ShouldPass()
    {
        // Pass Permission, Only Apply on Linux without Crashing on Windows
        var expected = "/tmp/my/path";
        if (!OperatingSystem.IsLinux())
            expected = Path.Combine(Path.GetTempPath(), "lunaticpanel", "pathf", "tmp", "my", "path");
        var actual = Pathf.CombineEnsureCreated(UnixFileMode.UserWrite | UnixFileMode.UserRead, Pathf.DirectorySeparatorChar, "tmp", "my", "path");
        Assert.Equal(expected, actual);
        Assert.True(Directory.Exists(expected));
    }


    [Fact]
    public void EnsureCreated_ShouldPass()
    {
        string[] args = ["/", "tmp", "my", "path"];
        if (!OperatingSystem.IsLinux())
            args = [Path.GetTempPath(), "lunaticpanel", "pathf", "tmp", "my", "path"];

        var actual = Pathf.EnsureCreated(args);
        var expected = Path.Combine(args);
        Assert.Equal(expected, actual);
        Assert.True(Directory.Exists(expected));
    }

    [Fact]
    public void EnsureCreatedFile_ShouldPass()
    {
        string[] args = ["/", "tmp", "my", "path", "filename.net"];
        if (!OperatingSystem.IsLinux())
            args = [Path.GetTempPath(), "lunaticpanel", "pathf", "tmp", "my", "path", "filename.net"];

        var expected = Pathf.Combine(args);
        var actual = Pathf.EnsureCreated(expected);
        var dir = Path.GetDirectoryName(expected);
        Assert.Equal(expected, actual);
        Assert.True(Directory.Exists(dir));
        Assert.False(File.Exists(expected));
    }

    [Fact]
    public void ResetDirectory_ShouldPass()
    {
        string[] args = ["/", "tmp", "my", "path", "filename.net"];
        if (!OperatingSystem.IsLinux())
            args = [Path.GetTempPath(), "lunaticpanel", "pathf", "tmp", "my", "path", "filename.net"];

        var path = Pathf.CombineEnsureCreated(args);
        File.WriteAllText(path, "whatever");
        Assert.True(File.Exists(path));
        var dir = Path.GetDirectoryName(path);
        Pathf.ResetDirectory(dir!);
        Assert.False(File.Exists(path));

    }

    [Fact]
    public void ResetDirectory_WithUnixPermission_ShouldPass()
    {
        string[] args = ["/", "tmp", "my", "path", "filename.net"];
        if (!OperatingSystem.IsLinux())
            args = [Path.GetTempPath(), "lunaticpanel", "pathf", "tmp", "my", "path", "filename.net"];

        var path = Pathf.CombineEnsureCreated(args);
        File.WriteAllText(path, "whatever");
        Assert.True(File.Exists(path));
        var dir = Path.GetDirectoryName(path);
        Pathf.ResetDirectory(UnixFileMode.UserWrite | UnixFileMode.UserRead, dir!);
        Assert.False(File.Exists(path));

    }

    [Fact]
    public void ResetDirectory_DoNotWhenFilePath_ShouldPass()
    {
        string[] args = ["/", "tmp", "my", "path", "filename.net"];
        if (!OperatingSystem.IsLinux())
            args = [Path.GetTempPath(), "lunaticpanel", "pathf", "tmp", "my", "path", "filename.net"];

        var path = Pathf.CombineEnsureCreated(args);
        File.WriteAllText(path, "whatever");
        Assert.True(File.Exists(path));
        Pathf.ResetDirectory(path);
        Assert.True(File.Exists(path));
    }


    [Fact]
    public void ResetDirectory_DoNotWhenFilePath_WithUnixPermission_ShouldPass()
    {
        string[] args = ["/", "tmp", "my", "path", "filename.net"];
        if (!OperatingSystem.IsLinux())
            args = [Path.GetTempPath(), "lunaticpanel", "pathf", "tmp", "my", "path", "filename.net"];

        var path = Pathf.CombineEnsureCreated(args);
        File.WriteAllText(path, "whatever");
        Assert.True(File.Exists(path));
        Pathf.ResetDirectory(UnixFileMode.UserWrite | UnixFileMode.UserRead, path);
        Assert.True(File.Exists(path));
    }

}

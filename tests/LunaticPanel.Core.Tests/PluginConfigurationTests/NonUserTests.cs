using LunaticPanel.Core.Abstraction;

namespace LunaticPanel.Core.Tests.PluginConfigurationTests;

public class NonUserTests
{
    private IPluginConfiguration _pluginConfiguration = default!;
    public NonUserTests()
    {
        _pluginConfiguration = new PluginConfiguration("Test.Assembly"); // should become test_assembly for linux folder
    }

    [Fact]
    public void MakeSureArgumentsAreWrappedIntoDoubleQuotes()
    {
        var arg = _pluginConfiguration.ArgumentsToString("-o", "name=\"eee\"");
        Assert.True(arg == "\\\"-o\\\" \\\"name=\"eee\"\\\"");
    }


    [Fact]
    public void UppercaseShouldFailComparison()
    {
        var arg = _pluginConfiguration.ArgumentsToString("-o", "name=\"eee\"");
        Assert.True(arg != "\\\"-o\\\" \\\"Name=\"eee\"\\\"");
    }

    [Fact]
    public void BashBaseShouldPass()
    {
        //etc/
        var path = _pluginConfiguration.GetBashBase("MyModule");
        var correctPath = Path.Combine($"{Path.DirectorySeparatorChar}", "usr", "lib", "lunaticpanel", "plugins", "test_assembly", "bash", "mymodule");
        Assert.Equal(path, correctPath);
    }

    [Fact]
    public void BashBaseShouldPass_WithSubFolders()
    {
        //etc/
        var path = _pluginConfiguration.GetBashBase("MyModule", "my", "sub", "folder");
        var correctPath = Path.Combine($"{Path.DirectorySeparatorChar}", "usr",
            "lib", "lunaticpanel", "plugins", "test_assembly",
            "bash", "mymodule", "my", "sub", "folder"
            );
        Assert.Equal(path, correctPath);
    }

    [Fact]
    public void BashForShouldPass()
    {
        //etc/
        var path = _pluginConfiguration.GetBashFor("MyModule", "myfile.sh");
        var correctPath = Path.Combine($"{Path.DirectorySeparatorChar}", "usr", "lib",
            "lunaticpanel", "plugins", "test_assembly",
            "bash", "mymodule", "myfile.sh");
        Assert.Equal(path, correctPath);
    }


    [Fact]
    public void BashForShouldFail()
    {
        //etc/
        var path = _pluginConfiguration.GetBashFor("MyModule", "myfile.sh");
        var correctPath = Path.Combine($"{Path.DirectorySeparatorChar}", "usr", "lib",
            "lunaticpanel", "plugins", "test_assembly",
            "bash", "mymodule", "myfile2.sh");
        Assert.NotEqual(path, correctPath);
    }


    [Fact]
    public void BashForShouldPass_WithSubFolder()
    {
        //etc/
        var path = _pluginConfiguration.GetBashFor("MyModule", ["sub", "folder"], "myfile.sh");
        var correctPath = Path.Combine($"{Path.DirectorySeparatorChar}", "usr", "lib",
            "lunaticpanel", "plugins", "test_assembly",
            "bash", "mymodule", "sub", "folder", "myfile.sh");
        Assert.Equal(path, correctPath);
    }
    [Fact]
    public void BashForShouldPass_WithSubFolderNArgs()
    {
        //etc/
        var path = _pluginConfiguration.GetBashFor("MyModule", ["sub", "folder"], "myfile.sh", "-o", "-f \"/etc/fdfd/ere.sh\"");
        var correctPath = Path.Combine($"{Path.DirectorySeparatorChar}", "usr", "lib",
            "lunaticpanel", "plugins", "test_assembly",
            "bash", "mymodule", "sub", "folder", "myfile.sh") + " \\\"-o\\\" \\\"-f \"/etc/fdfd/ere.sh\"\\\"";
        Assert.Equal(path, correctPath);
    }

    [Fact]
    public void BashForShouldPass_WithArgs()
    {
        //etc/
        var path = _pluginConfiguration.GetBashFor("MyModule", "myfile.sh", "-o", "-f \"/etc/fdfd/ere.sh\"");
        var correctPath = Path.Combine($"{Path.DirectorySeparatorChar}", "usr", "lib",
            "lunaticpanel", "plugins", "test_assembly",
            "bash", "mymodule", "myfile.sh") + " \\\"-o\\\" \\\"-f \"/etc/fdfd/ere.sh\"\\\"";
        Assert.Equal(path, correctPath);
    }


    [Fact]
    public void ConfigBaseShouldPass()
    {
        //etc/
        var path = _pluginConfiguration.GetConfigBase("MyModule");
        var correctPath = Path.Combine($"{Path.DirectorySeparatorChar}", "etc", "lunaticpanel", "plugins", "test_assembly", "config", "mymodule");
        Assert.Equal(path, correctPath);
    }

    [Fact]
    public void ConfigBaseShouldPass_WithSubFolders()
    {
        //etc/
        var path = _pluginConfiguration.GetConfigBase("MyModule", "my", "sub", "folder");
        var correctPath = Path.Combine($"{Path.DirectorySeparatorChar}", "etc",
            "lunaticpanel", "plugins", "test_assembly",
            "config", "mymodule", "my", "sub", "folder"
            );
        Assert.Equal(path, correctPath);
    }

    [Fact]
    public void ConfigForShouldPass()
    {
        //etc/
        var path = _pluginConfiguration.GetConfigFor("MyModule", "myfile.json");
        var correctPath = Path.Combine($"{Path.DirectorySeparatorChar}", "etc",
            "lunaticpanel", "plugins", "test_assembly",
            "config", "mymodule", "myfile.json");
        Assert.Equal(path, correctPath);
    }

    [Fact]
    public void ConfigForShouldPass_WithSubFolder()
    {
        //etc/
        var path = _pluginConfiguration.GetConfigFor("MyModule", ["sub", "Folder"], "myfile.json");
        var correctPath = Path.Combine($"{Path.DirectorySeparatorChar}", "etc",
            "lunaticpanel", "plugins", "test_assembly",
            "config", "mymodule", "sub", "Folder", "myfile.json");
        Assert.Equal(path, correctPath);
    }
}

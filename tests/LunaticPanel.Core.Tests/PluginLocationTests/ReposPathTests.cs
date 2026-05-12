using LunaticPanel.Core.Utils.Plugin;

namespace LunaticPanel.Core.Tests.PluginConfigurationTests;

public class ReposPathTests
{
    private PluginLocation _pluginConfiguration = default!;
    public ReposPathTests()
    {
        _pluginConfiguration = new PluginLocation("Test.Assembly"); // should become test_assembly for linux folder
    }



    [Fact]
    public void ReposBaseShouldPass()
    {
        //etc/
        var path = _pluginConfiguration.GetReposBase("MyModule");
        var correctPath = Path.Combine($"{Path.DirectorySeparatorChar}", "etc", "lunaticpanel", "plugins", "test_assembly", "repos", "mymodule");
        Assert.Equal(path, correctPath);
    }

    [Fact]
    public void ReposBaseShouldPass_WithSubFolders()
    {
        //etc/
        var path = _pluginConfiguration.GetReposBase("MyModule", "my", "sub", "folder");
        var correctPath = Path.Combine($"{Path.DirectorySeparatorChar}", "etc",
            "lunaticpanel", "plugins", "test_assembly",
            "repos", "mymodule", "my", "sub", "folder"
            );
        Assert.Equal(path, correctPath);
    }
    [Fact]
    public void ReposForShouldPass()
    {
        //etc/
        var path = _pluginConfiguration.GetReposFor("MyModule", "MyRepos");
        var correctPath = Path.Combine($"{Path.DirectorySeparatorChar}", "etc",
            "lunaticpanel", "plugins", "test_assembly",
            "repos", "mymodule", "MyRepos");
        Assert.Equal(path, correctPath);
    }

    [Fact]
    public void ReposForShouldPass_WithSubFolder()
    {
        //etc/
        var path = _pluginConfiguration.GetReposFor("MyModule", ["sub", "Folder"], "MyRepos");
        var correctPath = Path.Combine($"{Path.DirectorySeparatorChar}", "etc",
            "lunaticpanel", "plugins", "test_assembly",
            "repos", "mymodule", "sub", "Folder", "MyRepos");
        Assert.Equal(path, correctPath);
    }
}

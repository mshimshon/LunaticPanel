using LunaticPanel.Core.Abstraction.Exceptions;

namespace LunaticPanel.Core.Tests.PluginConfigurationTests;

public class DownloadPathTests
{
    private PluginConfiguration _pluginConfiguration = default!;
    public DownloadPathTests()
    {
        _pluginConfiguration = new PluginConfiguration("Test.Assembly"); // should become test_assembly for linux folder
    }



    [Fact]
    public void UserDownloadBaseShouldThrow_GlobalUserException()
    {
        Assert.Throws<GlobalUserRequiredException>(() => _pluginConfiguration.GetUserDownloadBase("MyModule"));
    }

    [Fact]
    public void UserDownloadBaseShouldThrow_GlobalUserException_WithSubFolders()
    {
        Assert.Throws<GlobalUserRequiredException>(() => _pluginConfiguration.GetUserDownloadBase("MyModule", ["", ""]));
    }

    [Fact]
    public void UserDownloadBaseShouldPass_GlobalUser()
    {
        _pluginConfiguration.SetUsername("theglobaluser");
        var path = _pluginConfiguration.GetUserDownloadBase("MyModule");
        var correctPath = Path.Combine($"{Path.DirectorySeparatorChar}", "home", "theglobaluser", "lunaticpanel", "plugins", "test_assembly", "download", "mymodule");
        Assert.Equal(path, correctPath);
    }

    [Fact]
    public void UserDownloadBaseShouldPass_GlobalUser_WithSubFolders()
    {
        _pluginConfiguration.SetUsername("theglobaluser");
        var path = _pluginConfiguration.GetUserDownloadBase("MyModule", "my", "sub", "folder");
        var correctPath = Path.Combine($"{Path.DirectorySeparatorChar}", "home", "theglobaluser",
            "lunaticpanel", "plugins", "test_assembly",
            "download", "mymodule", "my", "sub", "folder"
            );
        Assert.Equal(path, correctPath);
    }

    [Fact]
    public void UserDownloadBaseShouldPass_ExplicitUser()
    {
        var path = _pluginConfiguration.GetUserDownloadBase("MyModule", "myusername");
        var correctPath = Path.Combine($"{Path.DirectorySeparatorChar}", "home", "myusername", "lunaticpanel", "plugins", "test_assembly", "download", "mymodule");
        Assert.Equal(path, correctPath);
    }

    [Fact]
    public void UserDownloadBaseShouldPass_ExplicitUser_WithSubFolders()
    {
        var path = _pluginConfiguration.GetUserDownloadBase("MyModule", ["my", "sub", "folder"], "myusername");
        var correctPath = Path.Combine($"{Path.DirectorySeparatorChar}", "home", "myusername",
            "lunaticpanel", "plugins", "test_assembly",
            "download", "mymodule", "my", "sub", "folder"
            );
        Assert.Equal(path, correctPath);
    }

    [Fact]
    public void UserDownloadForShouldPass_ExplicitUser()
    {
        var path = _pluginConfiguration.GetUserDownloadFor("MyModule", "file.sh", "myusername");
        var correctPath = Path.Combine($"{Path.DirectorySeparatorChar}", "home",
            "myusername", "lunaticpanel", "plugins",
            "test_assembly", "download", "mymodule", "file.sh");
        Assert.Equal(path, correctPath);
    }

    [Fact]
    public void UserDownloadForShouldPass_ExplicitUser_WithSubFolders()
    {
        var path = _pluginConfiguration.GetUserDownloadFor("MyModule", ["my", "sub", "folder"], "file.sh", "myusername");
        var correctPath = Path.Combine($"{Path.DirectorySeparatorChar}", "home", "myusername",
            "lunaticpanel", "plugins", "test_assembly",
            "download", "mymodule", "my", "sub", "folder", "file.sh"
            );
        Assert.Equal(path, correctPath);
    }

}

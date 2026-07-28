using LunaticPanel.Core.Utils.Abstraction.Plugin.Location.Exceptions;
using LunaticPanel.Core.Utils.Plugin;

namespace LunaticPanel.Core.Tests.PluginLocationTests;

public class ConfigPathTests
{
    private PluginLocation _pluginConfiguration = default!;
    public ConfigPathTests()
    {
        _pluginConfiguration = new PluginLocation("Test.Assembly"); // should become test_assembly for linux folder
    }

    [Fact]
    public void AppBinBaseShouldPass()
    {
        //etc/
        var path = _pluginConfiguration.GetAppBinBase();
        var correctPath = Path.Combine($"{Path.DirectorySeparatorChar}", "srv", "lunaticpanel", "plugins", "test_assembly");
        Assert.Equal(path, correctPath);
    }

    [Fact]
    public void AppBinBaseShouldPass_WithSubFolders()
    {
        //etc/
        var path = _pluginConfiguration.GetAppBinBase("MyModule", "my", "sub", "folder");
        var correctPath = Path.Combine($"{Path.DirectorySeparatorChar}", "srv", "lunaticpanel", "plugins",
            "test_assembly", "MyModule", "my", "sub", "folder");
        Assert.Equal(path, correctPath);
    }



    [Fact]
    public void AppDataBaseShouldPass()
    {
        //etc/
        var path = _pluginConfiguration.GetAppDataBase();
        var correctPath = Path.Combine($"{Path.DirectorySeparatorChar}", "var", "lib", "lunaticpanel", "plugins", "test_assembly");
        Assert.Equal(path, correctPath);
    }

    [Fact]
    public void AppDataBaseShouldPass_WithSubFolders()
    {
        //etc/
        var path = _pluginConfiguration.GetAppDataBase("MyModule", "my", "sub", "folder");
        var correctPath = Path.Combine($"{Path.DirectorySeparatorChar}", "var", "lib",
            "lunaticpanel", "plugins", "test_assembly", "MyModule", "my", "sub", "folder"
            );
        Assert.Equal(path, correctPath);
    }


    [Fact]
    public void AppConfigBaseShouldPass()
    {
        //etc/
        var path = _pluginConfiguration.GetAppConfigBase();
        var correctPath = Path.Combine($"{Path.DirectorySeparatorChar}", "etc", "lunaticpanel", "plugins", "test_assembly");
        Assert.Equal(path, correctPath);
    }

    [Fact]
    public void AppConfigBaseShouldPass_WithSubFolders()
    {
        //etc/
        var path = _pluginConfiguration.GetAppConfigBase("MyModule", "my", "sub", "folder");
        var correctPath = Path.Combine($"{Path.DirectorySeparatorChar}", "etc", "lunaticpanel", "plugins",
            "test_assembly", "MyModule", "my", "sub", "folder");
        Assert.Equal(path, correctPath);
    }



    [Fact]
    public void DownloadBaseShouldPass()
    {
        //etc/
        var path = _pluginConfiguration.GetDownloadBase("MyModule");
        var correctPath = Path.Combine($"{Path.DirectorySeparatorChar}", "var", "lib", "lunaticpanel", "plugins", "test_assembly", "download", "mymodule");
        Assert.Equal(path, correctPath);
    }

    [Fact]
    public void DownloadBaseShouldPass_WithSubFolders()
    {
        //etc/
        var path = _pluginConfiguration.GetDownloadBase("MyModule", "my", "sub", "folder");
        var correctPath = Path.Combine($"{Path.DirectorySeparatorChar}", "var", "lib",
            "lunaticpanel", "plugins", "test_assembly",
            "download", "mymodule", "my", "sub", "folder"
            );
        Assert.Equal(path, correctPath);
    }

    [Fact]
    public void DownloadForShouldPass()
    {
        //etc/
        var path = _pluginConfiguration.GetDownloadFor("MyModule", "myfile.json");
        var correctPath = Path.Combine($"{Path.DirectorySeparatorChar}", "var", "lib",
            "lunaticpanel", "plugins", "test_assembly",
            "download", "mymodule", "myfile.json");
        Assert.Equal(path, correctPath);
    }

    [Fact]
    public void DownloadForShouldPass_WithSubFolder()
    {
        //etc/
        var path = _pluginConfiguration.GetDownloadFor("MyModule", ["sub", "Folder"], "myfile.json");
        var correctPath = Path.Combine($"{Path.DirectorySeparatorChar}", "var", "lib",
            "lunaticpanel", "plugins", "test_assembly",
            "download", "mymodule", "sub", "Folder", "myfile.json");
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


    [Fact]
    public async Task UserConfigBaseShouldThrow_GlobalUserException()
    {
        await Assert.ThrowsAsync<GlobalUserRequiredException>(async () => _pluginConfiguration.GetUserConfigBase("MyModule"));
    }

    [Fact]
    public async Task UserConfigBaseShouldThrow_GlobalUserException_WithSubFolders()
    {
        await Assert.ThrowsAsync<GlobalUserRequiredException>(async () => _pluginConfiguration.GetUserConfigBase("MyModule", ["", ""]));
    }

    [Fact]
    public void UserConfigBaseShouldPass_GlobalUser()
    {
        _pluginConfiguration.SetUsername("theglobaluser");
        var path = _pluginConfiguration.GetUserConfigBase("MyModule");
        var correctPath = Path.Combine($"{Path.DirectorySeparatorChar}", "home", "theglobaluser", "lunaticpanel", "plugins", "test_assembly", "config", "mymodule");
        Assert.Equal(path, correctPath);
    }

    [Fact]
    public void UserConfigBaseShouldPass_GlobalUser_WithSubFolders()
    {
        _pluginConfiguration.SetUsername("theglobaluser");
        var path = _pluginConfiguration.GetUserConfigBase("MyModule", "my", "sub", "folder");
        var correctPath = Path.Combine($"{Path.DirectorySeparatorChar}", "home", "theglobaluser",
            "lunaticpanel", "plugins", "test_assembly",
            "config", "mymodule", "my", "sub", "folder"
            );
        Assert.Equal(path, correctPath);
    }
    [Fact]
    public void UserConfigForShouldPass_GlobalUser()
    {
        _pluginConfiguration.SetUsername("theglobaluser");
        var path = _pluginConfiguration.GetUserConfigFor("MyModule", "file.sh");
        var correctPath = Path.Combine($"{Path.DirectorySeparatorChar}", "home",
            "theglobaluser", "lunaticpanel", "plugins",
            "test_assembly", "config", "mymodule", "file.sh");
        Assert.Equal(path, correctPath);
    }

    [Fact]
    public void UserConfigForShouldPass_GlobalUser_WithSubFolders()
    {
        _pluginConfiguration.SetUsername("theglobaluser");
        var path = _pluginConfiguration.GetUserConfigFor("MyModule", ["my", "sub", "folder"], "file.sh");
        var correctPath = Path.Combine($"{Path.DirectorySeparatorChar}", "home", "theglobaluser",
            "lunaticpanel", "plugins", "test_assembly",
            "config", "mymodule", "my", "sub", "folder", "file.sh"
            );
        Assert.Equal(path, correctPath);
    }
    [Fact]
    public void UserConfigBaseShouldPass_ExplicitUser()
    {
        var path = _pluginConfiguration.GetUserConfigBase("MyModule", "myusername");
        var correctPath = Path.Combine($"{Path.DirectorySeparatorChar}", "home", "myusername", "lunaticpanel", "plugins", "test_assembly", "config", "mymodule");
        Assert.Equal(path, correctPath);
    }

    [Fact]
    public void UserConfigBaseShouldPass_ExplicitUser_WithSubFolders()
    {
        var path = _pluginConfiguration.GetUserConfigBase("MyModule", ["my", "sub", "folder"], "myusername");
        var correctPath = Path.Combine($"{Path.DirectorySeparatorChar}", "home", "myusername",
            "lunaticpanel", "plugins", "test_assembly",
            "config", "mymodule", "my", "sub", "folder"
            );
        Assert.Equal(path, correctPath);
    }

    [Fact]
    public void UserConfigForShouldPass_ExplicitUser()
    {
        var path = _pluginConfiguration.GetUserConfigFor("MyModule", "file.sh", "myusername");
        var correctPath = Path.Combine($"{Path.DirectorySeparatorChar}", "home",
            "myusername", "lunaticpanel", "plugins",
            "test_assembly", "config", "mymodule", "file.sh");
        Assert.Equal(path, correctPath);
    }

    [Fact]
    public void UserConfigForShouldPass_ExplicitUser_WithSubFolders()
    {
        var path = _pluginConfiguration.GetUserConfigFor("MyModule", ["my", "sub", "folder"], "file.sh", "myusername");
        var correctPath = Path.Combine($"{Path.DirectorySeparatorChar}", "home", "myusername",
            "lunaticpanel", "plugins", "test_assembly",
            "config", "mymodule", "my", "sub", "folder", "file.sh"
            );
        Assert.Equal(path, correctPath);
    }


}

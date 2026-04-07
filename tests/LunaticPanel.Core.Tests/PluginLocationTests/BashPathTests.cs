using LunaticPanel.Core.Utils.Abstraction.Plugin.Location.Exceptions;
using LunaticPanel.Core.Utils.Plugin;

namespace LunaticPanel.Core.Tests.PluginConfigurationTests;

public class BashPathTests
{
    private PluginLocation _pluginConfiguration = default!;
    public BashPathTests()
    {
        _pluginConfiguration = new PluginLocation("Test.Assembly"); // should become test_assembly for linux folder
    }
    [Fact]
    public void MakeSureArgumentsAreWrappedIntoDoubleQuotes()
    {
        var arg = _pluginConfiguration.ArgumentsToString("-o", "name=\"eee\"");
        Assert.True(arg == "\\\"-o\\\" \\\"name=\"eee\"\\\"");
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
    public async Task UserBashBaseShouldThrow_GlobalUserException()
    {
        await Assert.ThrowsAsync<GlobalUserRequiredException>(async () => _pluginConfiguration.GetUserConfigBase("MyModule"));
    }

    [Fact]
    public async Task UserBashBaseShouldThrow_GlobalUserException_WithSubFolders()
    {
        await Assert.ThrowsAsync<GlobalUserRequiredException>(async () => _pluginConfiguration.GetUserBashBase("MyModule", ["", ""]));
    }

    [Fact]
    public void UserBashBaseShouldPass_GlobalUser()
    {
        _pluginConfiguration.SetUsername("theglobaluser");
        var path = _pluginConfiguration.GetUserBashBase("MyModule");
        var correctPath = Path.Combine($"{Path.DirectorySeparatorChar}", "home", "theglobaluser", "lunaticpanel", "plugins", "test_assembly", "bash", "mymodule");
        Assert.Equal(path, correctPath);
    }

    [Fact]
    public void UserBashBaseShouldPass_GlobalUser_WithSubFolders()
    {
        _pluginConfiguration.SetUsername("theglobaluser");
        var path = _pluginConfiguration.GetUserBashBase("MyModule", "my", "sub", "folder");
        var correctPath = Path.Combine($"{Path.DirectorySeparatorChar}", "home", "theglobaluser",
            "lunaticpanel", "plugins", "test_assembly",
            "bash", "mymodule", "my", "sub", "folder"
            );
        Assert.Equal(path, correctPath);
    }

    [Fact]
    public void UserBashBaseShouldPass_ExplicitUser()
    {
        var path = _pluginConfiguration.GetUserBashBase("MyModule", "myusername");
        var correctPath = Path.Combine($"{Path.DirectorySeparatorChar}", "home", "myusername", "lunaticpanel", "plugins", "test_assembly", "bash", "mymodule");
        Assert.Equal(path, correctPath);
    }

    [Fact]
    public void UserBashBaseShouldPass_ExplicitUser_WithSubFolders()
    {
        var path = _pluginConfiguration.GetUserBashBase("MyModule", ["my", "sub", "folder"], "myusername");
        var correctPath = Path.Combine($"{Path.DirectorySeparatorChar}", "home", "myusername",
            "lunaticpanel", "plugins", "test_assembly",
            "bash", "mymodule", "my", "sub", "folder"
            );
        Assert.Equal(path, correctPath);
    }

    [Fact]
    public void UserBashForShouldPass_ExplicitUser()
    {
        var path = _pluginConfiguration.GetUserBashFor("MyModule", "file.sh", "myusername");
        var correctPath = Path.Combine($"{Path.DirectorySeparatorChar}", "home",
            "myusername", "lunaticpanel", "plugins",
            "test_assembly", "bash", "mymodule", "file.sh");
        Assert.Equal(path, correctPath);
    }

    [Fact]
    public void UserBashForShouldPass_ExplicitUser_WithSubFolders()
    {
        var path = _pluginConfiguration.GetUserBashFor("MyModule", ["my", "sub", "folder"], "file.sh", "myusername");
        var correctPath = Path.Combine($"{Path.DirectorySeparatorChar}", "home", "myusername",
            "lunaticpanel", "plugins", "test_assembly",
            "bash", "mymodule", "my", "sub", "folder", "file.sh"
            );
        Assert.Equal(path, correctPath);
    }
    [Fact]
    public void UserBashForShouldPass_ExplicitUser_WithSubFolderNArgs()
    {
        //etc/
        var path = _pluginConfiguration.GetUserBashFor("MyModule", ["sub", "folder"], "myfile.sh", "myusername", "-o", "-f \"/etc/fdfd/ere.sh\"");
        var correctPath = Path.Combine($"{Path.DirectorySeparatorChar}", "home", "myusername",
            "lunaticpanel", "plugins", "test_assembly",
            "bash", "mymodule", "sub", "folder", "myfile.sh") + " \\\"-o\\\" \\\"-f \"/etc/fdfd/ere.sh\"\\\"";
        Assert.Equal(path, correctPath);
    }

    [Fact]
    public void UserBashForShouldPass_ExplicitUser_WithArgs()
    {
        //etc/
        var path = _pluginConfiguration.GetUserBashFor("MyModule", "myfile.sh", "myusername", "-o", "-f \"/etc/fdfd/ere.sh\"");
        var correctPath = Path.Combine($"{Path.DirectorySeparatorChar}", "home", "myusername",
            "lunaticpanel", "plugins", "test_assembly",
            "bash", "mymodule", "myfile.sh") + " \\\"-o\\\" \\\"-f \"/etc/fdfd/ere.sh\"\\\"";
        Assert.Equal(path, correctPath);
    }
}

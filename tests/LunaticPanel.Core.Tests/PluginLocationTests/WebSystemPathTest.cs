using LunaticPanel.Core.Utils.Plugin;

namespace LunaticPanel.Core.Tests.PluginLocationTests;

public class WebSystemPathTest
{
    private PluginLocation _pluginConfiguration = default!;
    public WebSystemPathTest()
    {
        _pluginConfiguration = new PluginLocation("Test.Assembly"); // should become test_assembly for linux folder
    }



    [Fact]
    public void GetStaticWebContentBaseShouldPass()
    {
        ///usr/lib/lunaticpanel/plugins/gamehost
        var path = _pluginConfiguration.GetStaticWebContentBase();
        var correctPath = Path.Combine($"{Path.DirectorySeparatorChar}", "srv", "lunaticpanel", "plugins", "test_assembly", "wwwroot");
        Assert.Equal(path, correctPath);
    }


    [Fact]
    public void GetStaticWebContentBase_WithSubFolder_ShouldPass()
    {
        ///usr/lib/lunaticpanel/plugins/gamehost
        var path = _pluginConfiguration.GetStaticWebContentBase(["mypath"]);
        var correctPath = Path.Combine($"{Path.DirectorySeparatorChar}", "srv", "lunaticpanel", "plugins", "test_assembly", "wwwroot", "mypath");
        Assert.Equal(path, correctPath);
    }
    [Fact]
    public void GetStaticWebContentBase_Module_ShouldPass()
    {
        ///usr/lib/lunaticpanel/plugins/gamehost
        var path = _pluginConfiguration.GetStaticWebContentBase("Module");
        var correctPath = Path.Combine($"{Path.DirectorySeparatorChar}", "srv", "lunaticpanel", "plugins", "test_assembly", "wwwroot", "module");
        Assert.Equal(path, correctPath);
    }

    [Fact]
    public void GetStaticWebContentBase_WithSubFolderAndModule_ShouldPass()
    {
        ///usr/lib/lunaticpanel/plugins/gamehost
        var path = _pluginConfiguration.GetStaticWebContentBase("Module", ["mypath"]);
        var correctPath = Path.Combine($"{Path.DirectorySeparatorChar}", "srv", "lunaticpanel", "plugins", "test_assembly", "wwwroot", "module", "mypath");
        Assert.Equal(path, correctPath);
    }

    [Fact]
    public void GetStaticWebContentFor_WithSubFolderAndModule_ShouldPass()
    {
        ///usr/lib/lunaticpanel/plugins/gamehost
        var path = _pluginConfiguration.GetStaticWebContentFor("Module", ["mypath"], "myfile.ss");
        var correctPath = Path.Combine($"{Path.DirectorySeparatorChar}", "srv", "lunaticpanel", "plugins", "test_assembly", "wwwroot", "module", "mypath", "myfile.ss");
        Assert.Equal(path, correctPath);
    }


    [Fact]
    public void GetStaticWebContentFor_WithSubFolder_ShouldPass()
    {
        ///usr/lib/lunaticpanel/plugins/gamehost
        var path = _pluginConfiguration.GetStaticWebContentFor(["mypath"], "myfile.ss");
        var correctPath = Path.Combine($"{Path.DirectorySeparatorChar}", "srv", "lunaticpanel", "plugins", "test_assembly", "wwwroot", "mypath", "myfile.ss");
        Assert.Equal(path, correctPath);
    }

    [Fact]
    public void GetStaticWebContentFor_WithModule_ShouldPass()
    {
        ///usr/lib/lunaticpanel/plugins/gamehost
        var path = _pluginConfiguration.GetStaticWebContentFor("Module", "myfile.ss");
        var correctPath = Path.Combine($"{Path.DirectorySeparatorChar}", "srv", "lunaticpanel", "plugins", "test_assembly", "wwwroot", "module", "myfile.ss");
        Assert.Equal(path, correctPath);
    }


    [Fact]
    public void GetDynamicWebContentBase_ShouldPass()
    {
        ///usr/lib/lunaticpanel/plugins/gamehost
        var path = _pluginConfiguration.GetDynamicWebContentBase();
        var correctPath = Path.Combine($"{Path.DirectorySeparatorChar}", "var", "lib", "lunaticpanel", "plugins", "test_assembly", "wwwroot");
        Assert.Equal(path, correctPath);
    }

    [Fact]
    public void GetDynamicWebContentBase_WithModule_ShouldPass()
    {
        ///usr/lib/lunaticpanel/plugins/gamehost
        var path = _pluginConfiguration.GetDynamicWebContentBase("Path");
        var correctPath = Path.Combine($"{Path.DirectorySeparatorChar}", "var", "lib", "lunaticpanel", "plugins", "test_assembly", "wwwroot", "path");
        Assert.Equal(path, correctPath);
    }
    [Fact]
    public void GetDynamicWebContentBase_WithSubFolder_ShouldPass()
    {
        ///usr/lib/lunaticpanel/plugins/gamehost
        var path = _pluginConfiguration.GetDynamicWebContentBase(["path"]);
        var correctPath = Path.Combine($"{Path.DirectorySeparatorChar}", "var", "lib", "lunaticpanel", "plugins", "test_assembly", "wwwroot", "path");
        Assert.Equal(path, correctPath);
    }


    [Fact]
    public void GetDynamicWebContentBase_WithSubFolderAndModule_ShouldPass()
    {
        ///usr/lib/lunaticpanel/plugins/gamehost
        var path = _pluginConfiguration.GetDynamicWebContentBase("Module", ["path"]);
        var correctPath = Path.Combine($"{Path.DirectorySeparatorChar}", "var", "lib", "lunaticpanel", "plugins", "test_assembly", "wwwroot", "module", "path");
        Assert.Equal(path, correctPath);
    }



    [Fact]
    public void GetDynamicWebContentFor_ShouldPass()
    {
        ///usr/lib/lunaticpanel/plugins/gamehost
        var path = _pluginConfiguration.GetDynamicWebContentFor("file.ss");
        var correctPath = Path.Combine($"{Path.DirectorySeparatorChar}", "var", "lib", "lunaticpanel", "plugins", "test_assembly", "wwwroot", "file.ss");
        Assert.Equal(path, correctPath);
    }

    [Fact]
    public void GetDynamicWebContentFor_WithModule_ShouldPass()
    {
        ///usr/lib/lunaticpanel/plugins/gamehost
        var path = _pluginConfiguration.GetDynamicWebContentFor("Path", "file.ss");
        var correctPath = Path.Combine($"{Path.DirectorySeparatorChar}", "var", "lib", "lunaticpanel", "plugins", "test_assembly", "wwwroot", "path", "file.ss");
        Assert.Equal(path, correctPath);
    }


    [Fact]
    public void GetDynamicWebContentFor_WithSubFolder_ShouldPass()
    {
        ///usr/lib/lunaticpanel/plugins/gamehost
        var path = _pluginConfiguration.GetDynamicWebContentFor(["path"], "file.ss");
        var correctPath = Path.Combine($"{Path.DirectorySeparatorChar}", "var", "lib", "lunaticpanel", "plugins", "test_assembly", "wwwroot", "path", "file.ss");
        Assert.Equal(path, correctPath);
    }


    [Fact]
    public void GetDynamicWebContentFor_WithSubFolderAndModule_ShouldPass()
    {
        ///usr/lib/lunaticpanel/plugins/gamehost
        var path = _pluginConfiguration.GetDynamicWebContentFor("Module", ["path"], "file.ss");
        var correctPath = Path.Combine($"{Path.DirectorySeparatorChar}", "var", "lib", "lunaticpanel", "plugins", "test_assembly", "wwwroot", "module", "path", "file.ss");
        Assert.Equal(path, correctPath);
    }
}

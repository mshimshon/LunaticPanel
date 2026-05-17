using LunaticPanel.Core.Utils.Plugin;

namespace LunaticPanel.Core.Tests.PluginLocationTests;

public class WebPathTests
{
    private PluginLocation _pluginConfiguration = default!;
    public WebPathTests()
    {
        _pluginConfiguration = new PluginLocation("Test.Assembly"); // should become test_assembly for linux folder
    }



    [Fact]
    public void GetRelativeStaticWebBase_ShouldPass()
    {
        ///usr/lib/lunaticpanel/plugins/gamehost
        var path = _pluginConfiguration.GetRelativeStaticWebBase();
        string[] chain = ["_plugins", "static", "Test.Assembly"];
        var correctPath = "/" + string.Join('/', chain);
        Assert.Equal(path, correctPath);
    }
    [Fact]
    public void GetRelativeStaticWebBase_WithSubFolder_ShouldPass()
    {
        ///usr/lib/lunaticpanel/plugins/gamehost
        var path = _pluginConfiguration.GetRelativeStaticWebBase(["path"]);
        string[] chain = ["_plugins", "static", "Test.Assembly", "path"];
        var correctPath = "/" + string.Join('/', chain);
        Assert.Equal(path, correctPath);
    }
    [Fact]
    public void GetRelativeStaticWebBase_WithModule_ShouldPass()
    {
        ///usr/lib/lunaticpanel/plugins/gamehost
        var path = _pluginConfiguration.GetRelativeStaticWebBase("Module");
        string[] chain = ["_plugins", "static", "Test.Assembly", "module"];
        var correctPath = "/" + string.Join('/', chain);
        Assert.Equal(path, correctPath);
    }

    [Fact]
    public void GetRelativeStaticWebBase_WithSubFolderAndModule_ShouldPass()
    {
        ///usr/lib/lunaticpanel/plugins/gamehost
        var path = _pluginConfiguration.GetRelativeStaticWebBase("Module", ["path"]);
        string[] chain = ["_plugins", "static", "Test.Assembly", "module", "path"];
        var correctPath = "/" + string.Join('/', chain);
        Assert.Equal(path, correctPath);
    }

    [Fact]
    public void GetRelativeStaticWebFor_ShouldPass()
    {
        ///usr/lib/lunaticpanel/plugins/gamehost
        var path = _pluginConfiguration.GetRelativeStaticWebFor("file.ss");
        string[] chain = ["_plugins", "static", "Test.Assembly", "file.ss"];
        var correctPath = "/" + string.Join('/', chain);
        Assert.Equal(path, correctPath);
    }
    [Fact]
    public void GetRelativeStaticWebFor_WithSubFolder_ShouldPass()
    {
        ///usr/lib/lunaticpanel/plugins/gamehost
        var path = _pluginConfiguration.GetRelativeStaticWebFor(["path"], "file.ss");
        string[] chain = ["_plugins", "static", "Test.Assembly", "path", "file.ss"];
        var correctPath = "/" + string.Join('/', chain);
        Assert.Equal(path, correctPath);
    }
    [Fact]
    public void GetRelativeStaticWebFor_WithModule_ShouldPass()
    {
        ///usr/lib/lunaticpanel/plugins/gamehost
        var path = _pluginConfiguration.GetRelativeStaticWebFor("Module", "file.ss");
        string[] chain = ["_plugins", "static", "Test.Assembly", "module", "file.ss"];
        var correctPath = "/" + string.Join('/', chain);
        Assert.Equal(path, correctPath);
    }

    [Fact]
    public void GetRelativeStaticWebFor_WithSubFolderAndModule_ShouldPass()
    {
        ///usr/lib/lunaticpanel/plugins/gamehost
        var path = _pluginConfiguration.GetRelativeStaticWebFor("Module", ["path"], "file.ss");
        string[] chain = ["_plugins", "static", "Test.Assembly", "module", "path", "file.ss"];
        var correctPath = "/" + string.Join('/', chain);
        Assert.Equal(path, correctPath);
    }






    [Fact]
    public void GetRelativeDynamicWebBase_ShouldPass()
    {
        ///usr/lib/lunaticpanel/plugins/gamehost
        var path = _pluginConfiguration.GetRelativeDynamicWebBase();
        string[] chain = ["_plugins", "dynamic", "Test.Assembly"];
        var correctPath = "/" + string.Join('/', chain);
        Assert.Equal(path, correctPath);
    }
    [Fact]
    public void GetRelativeDynamicWebBase_WithSubFolder_ShouldPass()
    {
        ///usr/lib/lunaticpanel/plugins/gamehost
        var path = _pluginConfiguration.GetRelativeDynamicWebBase(["path"]);
        string[] chain = ["_plugins", "dynamic", "Test.Assembly", "path"];
        var correctPath = "/" + string.Join('/', chain);
        Assert.Equal(path, correctPath);
    }
    [Fact]
    public void GetRelativeDynamicWebBase_WithModule_ShouldPass()
    {
        ///usr/lib/lunaticpanel/plugins/gamehost
        var path = _pluginConfiguration.GetRelativeDynamicWebBase("Module");
        string[] chain = ["_plugins", "dynamic", "Test.Assembly", "module"];
        var correctPath = "/" + string.Join('/', chain);
        Assert.Equal(path, correctPath);
    }

    [Fact]
    public void GetRelativeDynamicWebBase_WithSubFolderAndModule_ShouldPass()
    {
        ///usr/lib/lunaticpanel/plugins/gamehost
        var path = _pluginConfiguration.GetRelativeDynamicWebBase("Module", ["path"]);
        string[] chain = ["_plugins", "dynamic", "Test.Assembly", "module", "path"];
        var correctPath = "/" + string.Join('/', chain);
        Assert.Equal(path, correctPath);
    }

    [Fact]
    public void GetRelativeDynamicWebFor_ShouldPass()
    {
        ///usr/lib/lunaticpanel/plugins/gamehost
        var path = _pluginConfiguration.GetRelativeDynamicWebFor("file.ss");
        string[] chain = ["_plugins", "dynamic", "Test.Assembly", "file.ss"];
        var correctPath = "/" + string.Join('/', chain);
        Assert.Equal(path, correctPath);
    }
    [Fact]
    public void GetRelativeDynamicWebFor_WithSubFolder_ShouldPass()
    {
        ///usr/lib/lunaticpanel/plugins/gamehost
        var path = _pluginConfiguration.GetRelativeDynamicWebFor(["path"], "file.ss");
        string[] chain = ["_plugins", "dynamic", "Test.Assembly", "path", "file.ss"];
        var correctPath = "/" + string.Join('/', chain);
        Assert.Equal(path, correctPath);
    }
    [Fact]
    public void GetRelativeDynamicWebFor_WithModule_ShouldPass()
    {
        ///usr/lib/lunaticpanel/plugins/gamehost
        var path = _pluginConfiguration.GetRelativeDynamicWebFor("Module", "file.ss");
        string[] chain = ["_plugins", "dynamic", "Test.Assembly", "module", "file.ss"];
        var correctPath = "/" + string.Join('/', chain);
        Assert.Equal(path, correctPath);
    }

    [Fact]
    public void GetRelativeDynamicWebFor_WithSubFolderAndModule_ShouldPass()
    {
        ///usr/lib/lunaticpanel/plugins/gamehost
        var path = _pluginConfiguration.GetRelativeDynamicWebFor("Module", ["path"], "file.ss");
        string[] chain = ["_plugins", "dynamic", "Test.Assembly", "module", "path", "file.ss"];
        var correctPath = "/" + string.Join('/', chain);
        Assert.Equal(path, correctPath);
    }
}

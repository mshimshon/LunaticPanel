using LunaticPanel.Core.Abstraction;

namespace LunaticPanel.Core.Tests;

public class TestPluginConfiguration
{
    //public const string LunaticPanelFolderName = "lunaticpanel";
    //public const string LunaticPanelPluginsFolderName = "plugins";
    //public const string LinuxUsrFolderName = "usr";
    //public const string LinuxLibFolderName = "lib";
    //public const string LinuxEtcFolderName = "etc";
    //public const string LinuxVarFolderName = "var";

    private IPluginConfiguration _pluginConfiguration = default!;
    public TestPluginConfiguration()
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
}

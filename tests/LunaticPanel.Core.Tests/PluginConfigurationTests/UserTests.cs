using LunaticPanel.Core.Abstraction;

namespace LunaticPanel.Core.Tests.PluginConfigurationTests;

public class UserTests
{
    private IPluginConfiguration _pluginConfiguration = default!;
    public UserTests()
    {
        _pluginConfiguration = new PluginConfiguration("Test.Assembly"); // should become test_assembly for linux folder
    }

}

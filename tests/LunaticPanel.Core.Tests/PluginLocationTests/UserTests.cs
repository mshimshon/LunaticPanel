using LunaticPanel.Core.Utils.Abstraction.Plugin.Location.Exceptions;
using LunaticPanel.Core.Utils.Plugin;

namespace LunaticPanel.Core.Tests.PluginConfigurationTests;

public class UserTests
{
    private PluginLocation _pluginConfiguration = default!;
    public UserTests()
    {
        _pluginConfiguration = new PluginLocation("Test.Assembly"); // should become test_assembly for linux folder
    }

    [Fact]
    public async Task Throw_GlobalUserException()
    {
        // it just test if its trhow when no user is set as default
        await Assert.ThrowsAsync<GlobalUserRequiredException>(async () => _pluginConfiguration.RequiresGlobalUser("MyModule"));
    }



}

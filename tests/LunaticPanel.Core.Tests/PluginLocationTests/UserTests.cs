using LunaticPanel.Core.Abstraction.Exceptions;

namespace LunaticPanel.Core.Tests.PluginConfigurationTests;

public class UserTests
{
    private PluginConfiguration _pluginConfiguration = default!;
    public UserTests()
    {
        _pluginConfiguration = new PluginConfiguration("Test.Assembly"); // should become test_assembly for linux folder
    }

    [Fact]
    public void Throw_GlobalUserException()
    {
        // it just test if its trhow when no user is set as default
        Assert.Throws<GlobalUserRequiredException>(() => _pluginConfiguration.RequiresGlobalUser("MyModule"));
    }



}

using LunaticPanel.Core.Abstraction.Plugin;
using StatePulse.Net;
using System;

namespace LunaticPanel.Core.Tests.Common;

public class SensitiveHardCodeTest
{
    public void EnsureIPluginName_Pass()
    {
        string expected = "LunaticPanel.Core.Abstraction.Plugin.IPlugin";
        var actual = typeof(IPlugin).FullName;
        Assert.Equal(expected, actual);
    }

    /// <summary>
    /// Testing StatePulse type name if that change in the future at least we could know.
    /// </summary>
    public void EnsureStatePulseSingletonState_Pass()
    {
        string expected = "StatePulse.Net.IStateAccessor`1";
        var actual = typeof(IStateAccessor<>).FullName!;
        bool match = actual.StartsWith(expected);
        Assert.True(match);
    }
}

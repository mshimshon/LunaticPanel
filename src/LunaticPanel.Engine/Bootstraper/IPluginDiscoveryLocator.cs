using LunaticPanel.Core.Abstraction.DependencyInjection;
using LunaticPanel.Core.Abstraction.Plugin;
using LunaticPanel.Core.Extensions;
using LunaticPanel.Engine.Plugin;

using LunaticPanel.Engine.Services;
using NuGet.Versioning;
using System;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace LunaticPanel.Engine.Bootstraper;

public interface IPluginDiscoveryLocator
{

    string PluginLifecycleLocation { get; }
    string PluginLifecyclePreInstalledLocation { get; }
    string PluginBootstrapStateLocation { get; }
    string PluginLifecycleRuntimeLocation { get; }
    string PluginLifecycleApplyLocation { get; }
    string PluginLifecycleDeleteLocation { get; }
    string PluginLifecycleRollbackLocation { get; }
    string PluginLifecycleInstalledLocation { get; }
    void Initialize();
}

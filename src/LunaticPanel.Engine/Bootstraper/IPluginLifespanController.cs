using LunaticPanel.Core.Abstraction.DependencyInjection;
using LunaticPanel.Core.Abstraction.Plugin;
using LunaticPanel.Core.Extensions;
using LunaticPanel.Engine.Plugin;
using LunaticPanel.Engine.Web.Boostrap.Plugin;
using LunaticPanel.Engine.Web.Services;
using NuGet.Versioning;
using System;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace LunaticPanel.Engine.Web.Bootstraper;

public interface IPluginLifespanController
{

    PluginManifestEntity Manifest { get; }
    IPluginReference PluginReference { get; }
    IPluginInfo PluginInfo { get; }
    IPlugin Plugin { get; }
    string PluginDir { get; }
    string PluginVarDir { get; }
    string AssetStaticDir { get; }
    string AssetDynamicDir { get; }
    Task InitializeAsync();
    void ConfigureStaticAssets();
    Task SignalBeforeRuntimeAsync();
    void PrintPluginInfo();

}

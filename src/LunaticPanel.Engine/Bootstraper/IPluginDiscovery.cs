using LunaticPanel.Core.Abstraction.Plugin;
using LunaticPanel.Core.Extensions;
using LunaticPanel.Engine.Plugin;

using LunaticPanel.Engine.Services;
using Microsoft.Extensions.Configuration;
using NuGet.Versioning;
using System;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace LunaticPanel.Engine.Bootstraper;

public interface IPluginDiscovery
{

    IReadOnlyList<IPluginReference> LoadedPlugins { get; }
    IReadOnlyList<BootstrapStatusEntity> BootstrapState { get; }
    IReadOnlyList<PluginManifestEntity> PreInstalled { get; }
    IReadOnlyList<string> RuntimeTargets { get; }
    IReadOnlyList<PluginManifestEntity> ToApplyUpdates { get; }
    IReadOnlyList<PluginManifestEntity> Installed { get; }
    IReadOnlyList<PluginManifestEntity> Delete { get; }
    IReadOnlyList<PluginManifestEntity> Rollbacks { get; }
    List<PluginManifestEntity> DiscoverPackages(string path);
    void Commit_PreInstalled();
    void Commit_Delete();
    void Commit_Apply();
    void Commit_Install();
    void Commit_Runtime();
    Task Load_Runtime(IConfiguration configuration, CancellationToken ct = default);
    void Commit_All();
}

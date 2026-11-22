using LunaticPanel.Core.Abstraction.Plugin;
using McMaster.NETCore.Plugins;
using NuGet.Versioning;
using System.Reflection;

namespace LunaticPanel.Engine.Plugin;

public sealed record PluginReferenceEntity : IPluginReference
{
    private readonly PluginLoader _pluginLoader;
    private Assembly? _loaded = default;
    private IPlugin? _pluginEntry = default;
    public IPlugin Entry => _pluginEntry ?? throw new Exception("Entry not initialized.");
    public bool IsLoaded => _loaded != default && _pluginEntry != default;
    public NuGetVersion PanelVersion { get; }
    public PluginManifestEntity Manifest { get; }
    public NuGetVersion Version { get; }
    internal PluginReferenceEntity(PluginManifestEntity manifest, PluginLoader pluginLoader)
    {
        PanelVersion = NuGetVersion.Parse(manifest.PanelVersion);
        Version = NuGetVersion.Parse(manifest.Version);
        Manifest = manifest;
        _pluginLoader = pluginLoader;
    }

    public IPlugin Load()
    {
        if (IsLoaded)
            return _pluginEntry!;
        _loaded = _pluginLoader.LoadDefaultAssembly();
        var entryPoint = _loaded.GetType(Manifest.EntryClass, true)!;
        var asmName = entryPoint.Assembly.GetName().Name;
        if (asmName != Manifest.Id)
            throw new Exception($"{asmName} is not the same id as  {Manifest.Id}, plugin id must be the same as the root assembly and EntryPoint must reside at the base namespace.");
        if (!typeof(IPlugin).IsAssignableFrom(entryPoint))
            throw new Exception($"{entryPoint.FullName} does not implement {typeof(IPlugin).FullName}.");

        var plugin = Activator.CreateInstance(entryPoint);

        _pluginEntry = (IPlugin)plugin!;

        return _pluginEntry;
    }

    public void Unload()
    {
        _pluginLoader.Dispose();
        _pluginEntry = default;
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
    }
}

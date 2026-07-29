using LunaticPanel.Core.PluginValidator;
using System.Reflection;

namespace LunaticPanel.Engine.Plugin;

public static class DependencySettings
{

    public static AssemblyName[] SharedAssembliesToLoad { get; } =
    [
        .. CoreDependencies.OptionalCoreAssemblies.Select(p=> new AssemblyName(p)),
        .. CoreDependencies.RequiredCoreAssemblies.Select(p=> new AssemblyName(p)),
        new AssemblyName("MudBlazor"),
        new AssemblyName("System.Runtime"),
        new AssemblyName("System.Collections"),
        new AssemblyName("System.Net.Http"),
        new AssemblyName("Microsoft.AspNetCore.Components"),
        new AssemblyName("Microsoft.AspNetCore.Components.Web"),
        new AssemblyName("Microsoft.AspNetCore.Components.Forms"),
        new AssemblyName("Microsoft.AspNetCore.Components.Authorization"),
        new AssemblyName("Microsoft.JSInterop")
    ];
    private static AssemblyName[]? _sharedAssembliesToLoadCache = default;
    public static AssemblyName[] ScanSharedFrameworkNames()
        => SharedAssembliesToLoad;
}

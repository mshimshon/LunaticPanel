using System.Reflection;

namespace LunaticPanel.Engine.Plugin;

public static class DependencySettings
{
    public static AssemblyName[] SharedAssembliesToLoad { get; } =
    [
        // ==========================================
        // 1. Core
        // ==========================================
        new AssemblyName("LunaticPanel.Core"),
        new AssemblyName("LunaticPanel.Core.Abstraction"),
        new AssemblyName("LunaticPanel.Core.Extensions"),
        new AssemblyName("LunaticPanel.Core.Utils"),
        new AssemblyName("LunaticPanel.Core.Utils.Abstraction"),
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
    {
        return SharedAssembliesToLoad; // TODO: CLEAN UP
        if (_sharedAssembliesToLoadCache == default)
        {
            // 1. Check if we are running in a standard directory or single file bundle
            string binDir = AppContext.BaseDirectory;
            var diskDlls = Directory.GetFiles(binDir, "*.dll", SearchOption.TopDirectoryOnly)
                .Select(file =>
                {
                    try
                    {
                        var name = AssemblyName.GetAssemblyName(file);
                        return name;
                    }
                    catch
                    {
                        // Skip native, unmanaged, or corrupted DLL files that don't have CLR headers
                        return null;
                    }
                })
                .Select(p => p!);

            // 2. Fetch the framework assets tracked by the CLR bootstrap layer
            // (In single file mode, the runtime pre-populates these references at entry)
            var trustedPlatformAssemblies = (AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") as string)
                ?.Split(Path.PathSeparator)
                ?.Select(file =>
                {
                    try
                    {
                        var name = AssemblyName.GetAssemblyName(file);
                        return name;
                    }
                    catch
                    {
                        // Skip native, unmanaged, or corrupted DLL files that don't have CLR headers
                        return null;
                    }
                })
                ?.Where(p => p != default)
                ?.Select(p => p!)
                ?? Array.Empty<AssemblyName>();

            // 3. Combine both disk and bundled platform manifests
            var found = diskDlls.Concat(trustedPlatformAssemblies)
                .Where(asn => asn.Name != default)
                .Where(asn =>
                    asn.Name!.StartsWith("Microsoft.AspNetCore", StringComparison.OrdinalIgnoreCase) ||
                    asn.Name!.StartsWith("Microsoft.JSInterop", StringComparison.OrdinalIgnoreCase)
                );
            List<AssemblyName> result = new(SharedAssembliesToLoad);
            foreach (var item in found)
                if (!result.Any(p => string.Equals(p.Name, item.Name, StringComparison.OrdinalIgnoreCase)))
                    result.Add(item);
            _sharedAssembliesToLoadCache = result.ToArray();
            foreach (var item in _sharedAssembliesToLoadCache)
                Console.WriteLine($"{item.Name} host shared assembly.");
        }

        return _sharedAssembliesToLoadCache;
    }
}

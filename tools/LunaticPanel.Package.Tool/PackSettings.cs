using LunaticPanel.Core.Extensions;
using LunaticPanel.Package.Tool.Exceptions;

namespace LunaticPanel.Package.Tool;

internal static class PackSettings
{

    public static int DotNetVersion = 0;
    public static string LunaticPanelVersion = default!;
    //public static void PopulateExclusionDlls()
    //{
    //    var dllExclusion = _blacklistDlls.ToHashSet(StringComparer.OrdinalIgnoreCase);
    //    dllExclusion.UnionWith(ScanDllSdkLocation());

    //    dllExclusion.UnionWith(typeof(object).ScanDllAssemblyLocation());
    //    dllExclusion.UnionWith(typeof(WebApplication).ScanDllAssemblyLocation());
    //    dllExclusion.UnionWith(typeof(ConsoleApplicationBuilder).ScanAssemblyReferences());
    //    ExcludedDlls = dllExclusion;
    //    Console.Out.WriteLine($"Found {ExcludedDlls.Count} Dlls to Exclude from Packing".Green());
    //    foreach (var item in ExcludedDlls)
    //    {
    //        Console.Out.WriteLine($"{item} on exclusion list.".Blue());

    //    }
    //}

    private static HashSet<string> ScanDllAssemblyLocation(this Type t)
    {
        var aspNetDir = Path.GetDirectoryName(t.Assembly.Location)!;
        return Directory.GetFiles(aspNetDir, "*.dll")
            .Select(Path.GetFileNameWithoutExtension)
            .ToHashSet(StringComparer.OrdinalIgnoreCase)!;

    }

    private static HashSet<string> ScanDllSdkLocation()
    {


        string pathSep = Path.DirectorySeparatorChar.ToString();
        string dotnetRoot = Path.GetDirectoryName(typeof(object).Assembly.Location)!;
        Console.WriteLine($"Runtime Dotnet Location: {dotnetRoot}".Cyan());
        string runtimeDir = Path.GetFileName(dotnetRoot)!;
        Console.WriteLine($"Runtime Dotnet Dir: {runtimeDir}".Cyan());
        try
        {
            DotNetVersion = int.Parse(Path.GetFileName(runtimeDir).Split('.')[0]); // "10.0.9"

        }
        catch (Exception)
        {
            throw new DotnetSDKNotFoundException($"{runtimeDir} does not have version as last folder.");

        }

        string sharedFolder = $"{pathSep}shared{pathSep}";
        Console.WriteLine($"Checking for : {sharedFolder}".Yellow());
        if (!dotnetRoot.Contains(sharedFolder, StringComparison.OrdinalIgnoreCase))
            throw new DotnetSDKNotFoundException($"no {sharedFolder} found in path");

        string sdkRoot = Path.Combine(dotnetRoot.Split(sharedFolder)[0], "sdk");
        Console.WriteLine($"Testing for SDK folder : {sdkRoot}".Cyan());

        var sdkVersions = Directory.GetDirectories(sdkRoot)
            .Select(dir => Path.GetFileName(Path.TrimEndingDirectorySeparator(dir)))
            .Select(v => v.Split('.').Select(int.Parse).ToArray())
            .Where(v => v[0] == DotNetVersion)
                .OrderByDescending(v => v[0])
                .ThenByDescending(v => v[1])
                .ThenByDescending(v => v[2])
            .ToList();

        if (sdkVersions.Count <= 0)
            throw new DotnetSDKNotFoundException($".NET SDK {DotNetVersion} is required to pack");
        var selectedSdkVersion = sdkVersions.First();
        var sdkVersion = $"{selectedSdkVersion[0]}.{selectedSdkVersion[1]}.{selectedSdkVersion[2]}";
        Console.WriteLine($"SDK version is: {sdkVersion}".Cyan());

        string sdkDir = Path.Combine(sdkRoot, sdkVersion);
        Console.WriteLine($"Scanning : {sdkDir}".Cyan());
        var dlls = Directory.GetFiles(sdkDir, "*.dll", SearchOption.AllDirectories)
            .Select(Path.GetFileNameWithoutExtension)
            .ToHashSet(StringComparer.OrdinalIgnoreCase)!;
        string testChallengeDotNet = Path.Combine(sdkDir, "dotnet.dll");

        if (!File.Exists(testChallengeDotNet))
            throw new DotnetSDKNotFoundException(sdkDir);
        return dlls!;

    }

    private static HashSet<string> ScanAssemblyReferences(this Type t)
    {
        return t.Assembly.GetReferencedAssemblies()
            .Select(a => a.Name!)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
    }
}

namespace LunaticPanel.Package.Tool.Payloads;

public sealed record PluginManifestPayload
{
    public string Id { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string? Company { get; set; }
    public string Version { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string? Author { get; set; }
    public string PanelVersion { get; set; } = default!;
    public string DotnetVersion { get; set; } = default!;
    public string PluginEntryFile { get; set; } = default!;
    //var description = asm.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description;
    //var company = asm.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company;
    //var product = asm.GetCustomAttribute<AssemblyProductAttribute>()?.Product;
    //var title = asm.GetCustomAttribute<AssemblyTitleAttribute>()?.Title;
    //var fileVersion = asm.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version;
    //var infoVersion = asm.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
}

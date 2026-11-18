using System.Text.Json.Serialization;

namespace LunaticPanel.Engine.Plugin;

public sealed record PluginManifestEntity
{
    public PluginManifestEntity()
    {
    }
    [JsonIgnore]
    private string? _folderIdCache;
    [JsonIgnore]
    public string FolderId
    {
        get
        {
            if (_folderIdCache == default)
                _folderIdCache = Id?.Replace('.', '_')?.ToLower();
            return _folderIdCache!;
        }
    }
    [JsonIgnore]
    public string? PackageFileLocation { get; set; } = default!;
    [JsonIgnore]
    public string PackageFileName => PackageFileLocation != default ? Path.GetFileName(PackageFileLocation) : $"{Id}.{Version}.lpkg";
    public string Id { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string? Company { get; set; }
    public string Version { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string? Author { get; set; }
    public string? Copyright { get; set; }
    public string PanelVersion { get; set; } = default!;
    public string DotnetVersion { get; set; } = default!;
    public string PluginEntryFile { get; set; } = default!;
    public string EntryClass { get; set; } = default!;
}

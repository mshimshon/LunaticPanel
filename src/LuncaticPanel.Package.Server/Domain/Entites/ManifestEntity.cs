using LuncaticPanel.Package.Server.Domain.Entites.Enums;
using LuncaticPanel.Package.Server.Domain.Entites.ValueObjects;

namespace LuncaticPanel.Package.Server.Domain.Entites;

public sealed record ManifestEntity
{
    public PackageId Id { get; }
    public PackageVersion Version { get; }
    public PackagePanelVersion PanelVersion { get; }
    public PackageDotnetVersion DotnetVersion { get; }
    public PackagePluginEntryFile PluginEntryFile { get; }
    public PackageDescription Description { get; }
    public PackageAuthor Author { get; }
    public PackageTitle Title { get; init; }
    public PackageCopyright? Copyright { get; init; }
    public ManifestStatus Status { get; init; } = ManifestStatus.Hidden;
    public PackageEndOfLifeMessage? EndOfLifeMessage { get; init; }
    public ManifestEntity(PackageId packageId, PackageDescription description, PackageAuthor author,
        PackageVersion version, PackagePanelVersion panelVersion, PackageDotnetVersion dotnetVersion,
        PackagePluginEntryFile pluginEntryFile)
    {
        Id = packageId;
        Version = version;
        PanelVersion = panelVersion;
        DotnetVersion = dotnetVersion;
        PluginEntryFile = pluginEntryFile;
        Description = description;
        Author = author;
        Title = Title == default ? new(packageId.Value) : Title;
        if (EndOfLifeMessage != default)
            Status = ManifestStatus.EndOfLife;
    }
}

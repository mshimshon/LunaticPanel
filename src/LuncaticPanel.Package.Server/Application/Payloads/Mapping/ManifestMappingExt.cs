using LuncaticPanel.Package.Server.Application.Payloads.Enums;
using LuncaticPanel.Package.Server.Domain.Entites;
using LuncaticPanel.Package.Server.Domain.Entites.Enums;
using LuncaticPanel.Package.Server.Domain.Entites.ValueObjects;

namespace LuncaticPanel.Package.Server.Application.Payloads.Mapping;

public static class ManifestMappingExt
{
    public static ManifestPayload ToApplication(this ManifestEntity data)
    => new()
    {
        Author = data.Author.Value,
        Copyright = data.Copyright?.Value,
        Description = data.Description.Value,
        DotnetVersion = data.DotnetVersion.Value,
        EndOfLifeMessage = data.EndOfLifeMessage?.Value,
        Status = data.Status.ToApplication(),
        Id = data.Id.Value,
        PanelVersion = data.PanelVersion.Value,
        PluginEntryFile = data.PluginEntryFile.Value,
        Title = data.Title.Value,
        Version = data.Version.Value
    };

    public static ManifestEntity ToDomain(this ManifestPayload data)
        => new ManifestEntity(
            new PackageId(data.Id),
            new PackageDescription(data.Description),
            new PackageAuthor(data.Author),
            new PackageVersion(data.Version),
            new PackagePanelVersion(data.PanelVersion),
            new PackageDotnetVersion(data.DotnetVersion),
            new PackagePluginEntryFile(data.PluginEntryFile)
            )
        {
            Status = data.Status.ToDomain(),
            Title = new PackageTitle(data.Title),
            Copyright = data.Copyright != default ? new PackageCopyright(data.Copyright) : default,
            EndOfLifeMessage = data.EndOfLifeMessage != default ? new PackageEndOfLifeMessage(data.EndOfLifeMessage) : default
        };

    public static ManifestStatusPayload ToApplication(this ManifestStatus data)
    => data switch
    {
        ManifestStatus.EndOfLife => ManifestStatusPayload.EndOfLife,
        ManifestStatus.Visible => ManifestStatusPayload.Visible,
        _ => ManifestStatusPayload.Hidden
    };

    public static ManifestStatus ToDomain(this ManifestStatusPayload data)
        => data switch
        {
            ManifestStatusPayload.EndOfLife => ManifestStatus.EndOfLife,
            ManifestStatusPayload.Visible => ManifestStatus.Visible,
            _ => ManifestStatus.Hidden
        };
}

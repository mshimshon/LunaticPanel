using LuncaticPanel.Package.Server.Domain.Entites;
using LuncaticPanel.Package.Server.Domain.Entites.Enums;
using LuncaticPanel.Package.Server.Domain.Entites.ValueObjects;

namespace LunaticPanel.Package.LocalServer.Infrastructure.EntityFramework.Models.Mapping;

internal static class PackageMappingExt
{
    public static ManifestEntity ToDomain(this PackageInfoModel data)
    {
        PackageId id = new(data.PackageId);
        PackageDescription description = new(data.Description);
        PackageAuthor author = new(data.Author);
        PackageVersion version = new(data.Version);
        PackagePanelVersion panelVersion = new(data.PanelVersion);
        PackageDotnetVersion dotnetVersion = new(data.DotnetVersion);
        PackagePluginEntryFile pluginEntryFile = new(data.PluginEntryFile);
        bool isEoL = data.Package != default && data.Package.EndOfLifeMessage == default;
        ManifestStatus status = data.Hidden ? ManifestStatus.Hidden : ManifestStatus.Visible;
        if (isEoL)
            status = ManifestStatus.EndOfLife;
        return new ManifestEntity(id, description, author, version, panelVersion, dotnetVersion, pluginEntryFile)
        {
            Copyright = data.Copyright == default ? default : new(data.Copyright),
            EndOfLifeMessage = isEoL ? default : new(data.Package!.EndOfLifeMessage!)
        };
    }

    public static PackageModel ToPackageModel(this ManifestEntity data)
    {
        return new()
        {
            Id = data.Id.Value,
            EndOfLifeMessage = data.EndOfLifeMessage == default ? default : data.EndOfLifeMessage.Value
        };
    }

    public static PackageInfoModel ToPackageInfoModel(this ManifestEntity data)
    {
        return new()
        {
            PackageId = data.Id.Value,
            Author = data.Author.Value,
            Description = data.Description.Value,
            Copyright = data.Copyright?.Value,
            DotnetVersion = data.DotnetVersion.Value,
            PanelVersion = data.PanelVersion.Value,
            PluginEntryFile = data.PluginEntryFile.Value,
            Hidden = data.Status == ManifestStatus.Visible ? false : true,
            Title = data.Title.Value,
            Version = data.Version.Value,
            Package = data.ToPackageModel()
        };
    }
}

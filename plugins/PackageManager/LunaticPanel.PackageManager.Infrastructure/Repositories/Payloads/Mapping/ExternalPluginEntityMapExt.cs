using LunaticPanel.PackageManager.Domain.Entities;
using LunaticPanel.PackageManager.Domain.Entities.Enums;
using LunaticPanel.PackageManager.Domain.Entities.ValueObjects;
using LunaticPanel.PackageManager.Infrastructure.Repositories.Payloads.Enums;

namespace LunaticPanel.PackageManager.Infrastructure.Repositories.Payloads.Mapping;

internal static class ExternalPluginEntityMapExt
{
    public static PackageEntity MapToDomainEntity(this ExternalPluginPayload data, RepositorySourceInfo source)
    {

        var info = data.MapToDomainPackageInfo();
        var version = new PackageVersion(data.Manifest.Version);
        var panelVersion = new PackagePanelVersion(data.Manifest.PanelVersion);
        return new PackageEntity(info, source, version, panelVersion, new List<PackageDependency>());
    }

    public static PackageInfo MapToDomainPackageInfo(this ExternalPluginPayload data)
    {
        var id = new PackageId(data.Manifest.Id);
        var title = new PackageName(data.Manifest.Title);
        var desc = new PackageDescription(data.Manifest.Description ?? "No Description Found.");
        var status = data.StartupState.MapToDomain();
        return new PackageInfo(id, title, desc, status)
        {

        };
    }
    public static PackageState MapToDomain(this ExternalPluginStartupState data)
        => data switch
        {
            ExternalPluginStartupState.Enabled => PackageState.Enabled,
            ExternalPluginStartupState.Disabled => PackageState.Disabled,
            _ => PackageState.Unknown
        };

}

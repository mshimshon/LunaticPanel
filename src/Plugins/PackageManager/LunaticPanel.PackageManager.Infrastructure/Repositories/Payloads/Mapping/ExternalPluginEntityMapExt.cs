using LunaticPanel.PackageManager.Domain.Entities;
using LunaticPanel.PackageManager.Domain.Entities.Enums;
using LunaticPanel.PackageManager.Domain.Entities.ValueObjects;
using LunaticPanel.PackageManager.Infrastructure.Repositories.Payloads.Enums;

namespace LunaticPanel.PackageManager.Infrastructure.Repositories.Payloads.Mapping;

internal static class ExternalPluginEntityMapExt
{
    public static PackageEntity MapToDomainEntity(this ExternalPluginEntityPayload data, RepositorySourceInfo source)
    {

        var info = data.MapToDomainPackageInfo();
        var version = new PackageVersion(data.Identity.PakageVersion);
        var panelVersion = new PackagePanelVersion(data.Identity.PanelVersion);
        return new PackageEntity(info, source, version, panelVersion, new List<PackageDependency>());
    }

    public static PackageInfo MapToDomainPackageInfo(this ExternalPluginEntityPayload data)
    {
        var id = new PackageId(data.Identity.PackageId);
        var title = new PackageName(data.Identity.DisplayName);
        var desc = new PackageDescription(data.Identity.Description ?? "No Description Found.");
        var status = data.Lifecycle.StartupState.MapToDomain();
        return new PackageInfo(id, title, desc, status)
        {

        };
    }
    public static PackageState MapToDomain(this ExternalPluginEntityLifecycleStartupState data)
        => data switch
        {
            ExternalPluginEntityLifecycleStartupState.Enabled => PackageState.Enabled,
            ExternalPluginEntityLifecycleStartupState.Disabled => PackageState.Disabled,
            _ => PackageState.Unknown
        };

}

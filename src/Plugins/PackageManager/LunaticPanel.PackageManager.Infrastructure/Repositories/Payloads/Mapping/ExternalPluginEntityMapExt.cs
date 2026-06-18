using LunaticPanel.PackageManager.Domain.Entites;
using LunaticPanel.PackageManager.Domain.Entites.Enums;
using LunaticPanel.PackageManager.Domain.Entites.ValueObjects;
using LunaticPanel.PackageManager.Infrastructure.Repositories.Payloads.Enums;

namespace LunaticPanel.PackageManager.Infrastructure.Repositories.Payloads.Mapping;

internal static class ExternalPluginEntityMapExt
{
    public static PackageEntity MapToDomainEntity(this ExternalPluginEntityPayload data, RepositorySourceInfo source)
        => new PackageEntity(data.MapToDomainPackageInfo(), source, new(data.Identity.PakageVersion), new List<PackageDependency>());

    public static PackageInfo MapToDomainPackageInfo(this ExternalPluginEntityPayload data)
        => new PackageInfo(new(data.Identity.PackageId), new(data.Identity.DisplayName),
            new("This shit is not yet supported"), data.Lifecycle.StartupState.MapToDomain());
    public static PackageState MapToDomain(this ExternalPluginEntityLifecycleStartupState data)
        => data switch
        {
            ExternalPluginEntityLifecycleStartupState.Enabled => PackageState.Enabled,
            ExternalPluginEntityLifecycleStartupState.Disabled => PackageState.Disabled,
            _ => PackageState.Unknown
        };

}

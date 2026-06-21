using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Payloads.Enums;
using LunaticPanel.PackageManager.Infrastructure.Repositories.Payloads.Enums;

namespace LunaticPanel.PackageManager.Infrastructure.Repositories.Payloads.Mapping;

internal static class ExternalSourceRepositoryMapExt
{
    public static RepositorySourcePayload ToApplicationPayload(this ExternalSourceRepositoryPayload data)
        => new()
        {
            Name = data.Name,
            Source = data.Source,
            SourceType = data.SourceType.ToApplicationPayload(),
            State = data.State.ToApplicationPayload()
        };
    public static RepositorySourceTypePayload ToApplicationPayload(this ExternalSourceRepositoryTypePayload data)
        => data switch
        {
            ExternalSourceRepositoryTypePayload.Remote => RepositorySourceTypePayload.Remote,
            _ => RepositorySourceTypePayload.Local
        };

    public static RepositorySourceStatePayload ToApplicationPayload(this ExternalSourceRepositoryStatePayload data)
    => data switch
    {
        ExternalSourceRepositoryStatePayload.Enabled => RepositorySourceStatePayload.Enabled,
        ExternalSourceRepositoryStatePayload.Disabled => RepositorySourceStatePayload.Disabled,
        _ => RepositorySourceStatePayload.Unknown
    };



    public static ExternalSourceRepositoryPayload ToInfrastructurePayload(this RepositorySourcePayload data)
    => new()
    {
        Name = data.Name,
        Source = data.Source,
        SourceType = data.SourceType.ToInfrastructurePayload(),
        State = data.State.ToInfrastructurePayload()
    };
    public static ExternalSourceRepositoryTypePayload ToInfrastructurePayload(this RepositorySourceTypePayload data)
        => data switch
        {
            RepositorySourceTypePayload.Remote => ExternalSourceRepositoryTypePayload.Remote,
            _ => ExternalSourceRepositoryTypePayload.Local
        };

    public static ExternalSourceRepositoryStatePayload ToInfrastructurePayload(this RepositorySourceStatePayload data)
    => data switch
    {
        RepositorySourceStatePayload.Enabled => ExternalSourceRepositoryStatePayload.Enabled,
        RepositorySourceStatePayload.Disabled => ExternalSourceRepositoryStatePayload.Disabled,
        _ => ExternalSourceRepositoryStatePayload.Unknown
    };
}

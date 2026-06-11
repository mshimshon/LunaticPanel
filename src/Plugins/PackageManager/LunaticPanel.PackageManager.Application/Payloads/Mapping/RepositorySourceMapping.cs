using LunaticPanel.PackageManager.Application.Payloads.Enums;
using LunaticPanel.PackageManager.Domain.Entites;
using LunaticPanel.PackageManager.Domain.Entites.Enums;
using LunaticPanel.PackageManager.Domain.Entites.ValueObjects;

namespace LunaticPanel.PackageManager.Application.Payloads.Mapping;

public static class RepositorySourceMapping
{
    public static RepositorySourcePayload ToApplicationPayload(this RepositorySourceEntity data)
        => new()
        {
            Name = data.Name.Value,
            Source = data.Info.Source.Value,
            SourceType = data.Info.SourceType.ToApplicationPayload()
        };


    public static RepositorySourceTypePayload ToApplicationPayload(this RepositorySourceType data)
        => data switch
        {
            RepositorySourceType.Local => RepositorySourceTypePayload.Local,
            RepositorySourceType.Remote => RepositorySourceTypePayload.Remote,
            _ => throw new ArgumentOutOfRangeException(nameof(data))
        };


    public static RepositorySourceEntity ToDomainEntity(this RepositorySourcePayload data)
    {
        RepositorySource source =
            data.SourceType == Enums.RepositorySourceTypePayload.Local ?
            new RepositorySourceLocal(data.Source) :
            new RepositorySourceRemote(data.Source);
        RepositorySourceName name = new RepositorySourceName(data.Name);
        var sourceType = data.SourceType.ToDomainEntity();
        var state = data.State.ToDomainEntity();
        return data.Failure == default ?
            new RepositorySourceEntity(name, new(source, sourceType), state) :
            new RepositorySourceEntity(name, new(source, sourceType), new(data.Failure), state);
    }


    public static RepositorySourceType ToDomainEntity(this RepositorySourceTypePayload data)
        => data switch
        {
            RepositorySourceTypePayload.Local => RepositorySourceType.Local,
            RepositorySourceTypePayload.Remote => RepositorySourceType.Remote,
            _ => throw new ArgumentOutOfRangeException(nameof(data))
        };

    public static RepositorySourceState ToDomainEntity(this RepositorySourceStatePayload data)
        => data switch
        {
            RepositorySourceStatePayload.Unknown => RepositorySourceState.Unknown,
            RepositorySourceStatePayload.Enabled => RepositorySourceState.Enabled,
            RepositorySourceStatePayload.Disabled => RepositorySourceState.Disabled,
            _ => throw new ArgumentOutOfRangeException(nameof(data))
        };

    public static RepositorySourceStatePayload ToApplicationPayload(this RepositorySourceState data)
        => data switch
        {
            RepositorySourceState.Unknown => RepositorySourceStatePayload.Unknown,
            RepositorySourceState.Enabled => RepositorySourceStatePayload.Enabled,
            RepositorySourceState.Disabled => RepositorySourceStatePayload.Disabled,
            _ => throw new ArgumentOutOfRangeException(nameof(data))
        };
}
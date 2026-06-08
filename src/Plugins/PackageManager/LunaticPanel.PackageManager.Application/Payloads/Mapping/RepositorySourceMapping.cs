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
        return new(name, new(source, data.SourceType.ToDomainEntity()));
    }


    public static RepositorySourceType ToDomainEntity(this RepositorySourceTypePayload data)
        => data switch
        {
            RepositorySourceTypePayload.Local => RepositorySourceType.Local,
            RepositorySourceTypePayload.Remote => RepositorySourceType.Remote,
            _ => throw new ArgumentOutOfRangeException(nameof(data))
        };
}
using LunaticPanel.PackageManager.Application.Payloads.Enums;
using LunaticPanel.PackageManager.Domain.Entites;
using LunaticPanel.PackageManager.Domain.Entites.Enums;
using LunaticPanel.PackageManager.Domain.Entites.ValueObjects;

namespace LunaticPanel.PackageManager.Application.Payloads.Mapping;

public static class PackageMapping
{
    public static PackagePayload ToApplicationPayload(this PackageEntity data)
        => new()
        {
            Info = data.Info.ToApplicationPayload(),
            RepositorySource = data.Source.Source.Value,
            RepositoryType = data.Source.SourceType.ToApplicationPayload(),
            Version = data.Version.Value,
            Dependencies = data.Dependencies.Select(p => p.ToApplicationPayload()).ToList(),
            Failure = data.Failure?.Value
        };

    public static PackageInfoPayload ToApplicationPayload(this PackageInfo data)
    => new()
    {
        AutoUpdateScore = data.AutoUpdateScore.Value,
        Description = data.Description.Value,
        Name = data.Name.Value,
        PackageId = data.Id.Value,
        Rating = data.Rating?.Value ?? -1,
        State = data.State.ToApplicationPayload()
    };
    public static PackageDependencyPayload ToApplicationPayload(this PackageDependency data)
    => new()
    {
        Id = data.Id.Value,
        Name = data.Name.Value,
        Version = data.Version.Value
    };
    public static PackageEntity ToDomainEntity(this PackagePayload data)
    {
        var info = data.Info.ToDomainEntity();
        RepositorySource source =
            data.RepositoryType == Enums.RepositorySourceTypePayload.Local ?
            new RepositorySourceLocal(data.RepositorySource) :
            new RepositorySourceRemote(data.RepositorySource);

        var sourceInfo = new RepositorySourceInfo(source, data.RepositoryType.ToDomainEntity());
        var depList = data.Dependencies.Select(p => p.ToDomainEntity()).ToArray();
        var version = new PackageVersion(data.Version);
        return data.Failure == default ? new PackageEntity(info, sourceInfo, version, depList) :
            new PackageEntity(info, sourceInfo, version, depList, new(data.Failure));
    }

    public static PackageInfo ToDomainEntity(this PackageInfoPayload data)
        => new(
            new(data.PackageId),
            new(data.Name),
            new(data.Description),
            data.State.ToDomainEntity())
        {
            AutoUpdateScore = new(data.AutoUpdateScore),
            Rating = data.Rating <= 0 ? default : new(data.Rating),
        };

    public static PackageDependency ToDomainEntity(this PackageDependencyPayload data)
        => new(new(data.Name), new(data.Id), new(data.Version));


    public static PackageState ToDomainEntity(this PackageStatePayload data)
    => data switch
    {
        PackageStatePayload.Unknown => PackageState.Unknown,
        PackageStatePayload.Enabled => PackageState.Enabled,
        PackageStatePayload.Disabled => PackageState.Disabled,
        _ => throw new ArgumentOutOfRangeException(nameof(data))
    };

    public static PackageStatePayload ToApplicationPayload(this PackageState data)
=> data switch
{
    PackageState.Unknown => PackageStatePayload.Unknown,
    PackageState.Enabled => PackageStatePayload.Enabled,
    PackageState.Disabled => PackageStatePayload.Disabled,
    _ => throw new ArgumentOutOfRangeException(nameof(data))
};
}

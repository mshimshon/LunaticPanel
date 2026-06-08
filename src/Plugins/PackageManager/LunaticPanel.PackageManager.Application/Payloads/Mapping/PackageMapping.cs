using LunaticPanel.PackageManager.Domain.Entites;
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
            Version = data.Version.Value
        };

    public static PackageInfoPayload ToApplicationPayload(this PackageInfo data)
    => new()
    {
        AutoUpdateScore = data.AutoUpdateScore.Value,
        Description = data.Description.Value,
        Name = data.Name.Value,
        PackageId = data.Id.Value,
        Rating = data.Rating?.Value ?? -1
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

        var version = new PackageVersion(data.Version);
        return new PackageEntity(info, sourceInfo, version, data.Dependencies.Select(p => p.ToDomainEntity()).ToArray());
    }

    public static PackageInfo ToDomainEntity(this PackageInfoPayload data)
        => new(new(data.PackageId), new(data.Name), new(data.Description))
        {
            AutoUpdateScore = new(data.AutoUpdateScore),
            Rating = data.Rating <= 0 ? default : new(data.Rating)
        };

    public static PackageDependency ToDomainEntity(this PackageDependencyPayload data)
        => new(new(data.Name), new(data.Id), new(data.Version));
}

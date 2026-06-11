using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Payloads.Enums;
using LunaticPanel.PackageManager.Application.Payloads.Mapping;
using LunaticPanel.PackageManager.Domain.Entites;
using LunaticPanel.PackageManager.Domain.Entites.Enums;
using LunaticPanel.PackageManager.Domain.Entites.ValueObjects;

namespace LunaticPanel.PackageManager.Tests.Application.Mapping;

public class PackageMappingTests
{
    [Theory]
    [InlineData("MyPlugin.Dependency", "Test Name", "Test Desc", 85, 4, "https://github.com", RepositorySourceTypePayload.Remote, "1.0.0", "dep-1", "Dep1", "2.0.0", RepositorySourceType.Remote)]
    [InlineData("Core.Framework", "Local Plugin", "Local Desc", 95, 5, "C:\\local\\path", RepositorySourceTypePayload.Local, "3.1.2", "dep-2", "Dep2", "1.1.0", RepositorySourceType.Local)]
    public void PackageEntity_ToApplicationPayload_ShouldMapCorrectly(
        string packageId, string name, string description, int score, int rating,
        string repoSource, RepositorySourceTypePayload payloadType, string version,
        string depId, string depName, string depVersion, RepositorySourceType domainSourceType)
    {
        var info = new PackageInfo(new(packageId), new(name), new(description))
        {
            AutoUpdateScore = new(score),
            Rating = new(rating)
        };
        RepositorySource source = payloadType == RepositorySourceTypePayload.Local
            ? new RepositorySourceLocal(repoSource)
            : new RepositorySourceRemote(repoSource);
        var sourceInfo = new RepositorySourceInfo(source, domainSourceType);
        var pkgVersion = new PackageVersion(version);
        var dependencies = new[] { new PackageDependency(new(depName), new(depId), new(depVersion)) };

        var entity = new PackageEntity(info, sourceInfo, pkgVersion, dependencies);

        var payload = entity.ToApplicationPayload();

        Assert.NotNull(payload);
        Assert.Equal(version, payload.Version);
        Assert.Equal(repoSource, payload.RepositorySource);
        Assert.Equal(payloadType, payload.RepositoryType);
        Assert.NotNull(payload.Info);
        Assert.Equal(packageId, payload.Info.PackageId);
        Assert.Equal(name, payload.Info.Name);
        Assert.Equal(description, payload.Info.Description);
        Assert.Equal(score, payload.Info.AutoUpdateScore);
        Assert.Equal(rating, payload.Info.Rating);
    }

    [Theory]
    [InlineData("MyPlugin.Dependency", "Test Name", "Test Desc", 50)]
    [InlineData("Core.Framework", "Another Name", "Another Desc", 10)]
    public void PackageInfo_ToApplicationPayload_ShouldHandleNullRating(string id, string name, string description, int score)
    {
        var info = new PackageInfo(new(id), new(name), new(description))
        {
            AutoUpdateScore = new(score),
            Rating = null
        };

        var payload = info.ToApplicationPayload();

        Assert.Equal(-1, payload.Rating);
    }

    [Theory]
    [InlineData("My Plugin Dependency", "MyPlugin.Dependency", "1.0.0")]
    [InlineData("Core Framework", "Core.Framework", "4.5.6")]
    public void PackageDependency_ToApplicationPayload_ShouldMapCorrectly(string depName, string depId, string version)
    {
        var dependency = new PackageDependency(new(depName), new(depId), new(version));

        var payload = dependency.ToApplicationPayload();

        Assert.NotNull(payload);
        Assert.Equal(depId, payload.Id);
        Assert.Equal(depName, payload.Name);
        Assert.Equal(version, payload.Version);
    }

    [Theory]
    [InlineData(RepositorySourceTypePayload.Local, typeof(RepositorySourceLocal), "2.1.0", "C:\\local\\path", "id-999", "Payload Name", "Payload Desc", 90, 5, "d1", "Dep One", "1.0.0")]
    [InlineData(RepositorySourceTypePayload.Remote, typeof(RepositorySourceRemote), "1.0.0", "https://github.com", "id-111", "Remote Name", "Remote Desc", 40, 2, "d2", "Dep Two", "2.0.0")]
    public void PackagePayload_ToDomainEntity_ShouldMapCorrectly_ForDifferentRepositoryTypes(
        RepositorySourceTypePayload payloadType, Type expectedSourceType, string version, string repoSource,
        string packageId, string name, string description, int score, int rating,
        string depId, string depName, string depVersion)
    {
        var payload = new PackagePayload
        {
            Version = version,
            RepositorySource = repoSource,
            RepositoryType = payloadType,
            Info = new PackageInfoPayload
            {
                PackageId = packageId,
                Name = name,
                Description = description,
                AutoUpdateScore = score,
                Rating = rating
            },
            Dependencies = new List<PackageDependencyPayload>
            {
                new() { Id = depId, Name = depName, Version = depVersion }
            }
        };

        var entity = payload.ToDomainEntity();

        Assert.NotNull(entity);
        Assert.Equal(version, entity.Version.Value);
        Assert.Equal(repoSource, entity.Source.Source.Value);
        Assert.IsType(expectedSourceType, entity.Source.Source);

        Assert.Equal(packageId, entity.Info.Id.Value);
        Assert.Equal(name, entity.Info.Name.Value);
        Assert.Equal(description, entity.Info.Description.Value);
        Assert.Equal(score, entity.Info.AutoUpdateScore.Value);
        Assert.NotNull(entity.Info.Rating);
        Assert.Equal(rating, entity.Info.Rating.Value);

        Assert.Single(entity.Dependencies);
        var dependency = entity.Dependencies[0];
        Assert.Equal(depId, dependency.Id.Value);
        Assert.Equal(depName, dependency.Name.Value);
        Assert.Equal(depVersion, dependency.Version.Value);
    }

    [Theory]
    [InlineData(0, "id-1", "Dependency", "Desc-1", 10)]
    [InlineData(-5, "id-2", "AnotherDependency", "Desc-2", 20)]
    public void PackageInfoPayload_ToDomainEntity_ShouldHandleInvalidOrZeroRating(int invalidRating, string packageId, string name, string description, int score)
    {
        var payload = new PackageInfoPayload
        {
            PackageId = packageId,
            Name = name,
            Description = description,
            AutoUpdateScore = score,
            Rating = invalidRating
        };

        var entity = payload.ToDomainEntity();

        Assert.Null(entity.Rating);
    }

    [Theory]
    [InlineData(0, "id-1", "Dependency", "Desc-1", 10)]
    [InlineData(-5, "id-2", "AnotherDependency", "Desc-2", 20)]
    public void PackageInfoPayload_ToDomainEntity_ShouldCorrectlyConvertState(int invalidRating, string packageId, string name, string description, int score)
    {
        var payload = new PackageInfoPayload
        {
            PackageId = packageId,
            Name = name,
            Description = description,
            AutoUpdateScore = score,
            Rating = invalidRating,
            State = PackageStatePayload.Disabled
        };

        var entity = payload.ToDomainEntity();

        Assert.True(entity.State == PackageState.Disabled);
    }

    [Theory]
    [InlineData("Core.Framework", "Dependency", "4.5.6")]
    [InlineData("Core.AnotherDependency", "Another Dependency", "1.0.1")]
    public void PackageDependencyPayload_ToDomainEntity_ShouldMapCorrectly(string id, string name, string version)
    {
        var payload = new PackageDependencyPayload
        {
            Id = id,
            Name = name,
            Version = version
        };

        var entity = payload.ToDomainEntity();

        Assert.NotNull(entity);
        Assert.Equal(id, entity.Id.Value);
        Assert.Equal(name, entity.Name.Value);
        Assert.Equal(version, entity.Version.Value);
    }
}

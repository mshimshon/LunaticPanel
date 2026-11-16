using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Payloads.Enums;
using LunaticPanel.PackageManager.Domain.Entities;
using LunaticPanel.PackageManager.Domain.Entities.Enums;
using LunaticPanel.PackageManager.Domain.Entities.ValueObjects;

namespace LunaticPanel.PackageManager.Tests.Application.Mediator.Extensions;

public static class PackageExt
{
    public static PackageManifestPayload CreatePackageManifest(
    string id,
    string title,
    string version,
    string panelVersion) => new()
    {
        Id = id,
        Title = title,
        Company = "Contoso",
        Version = version,
        Description = "Package test description.",
        Author = "Whatever",
        Copyright = "© Contoso",
        PanelVersion = panelVersion,
        DotnetVersion = "8.0.12",
        PluginEntryFile = "Package.dll"
    };
    public static PackagePayload CreatePackage(
        string id,
        string title,
        string version,
        string panelVersion) => new()
        {
            Info = new()
            {
                PackageId = id,
                Description = "Package test description.",
                Name = title,
            },
            AutoUpdate = true,
            Version = version,
            PanelVersion = panelVersion
        };


    public static RepositorySourcePayload CreatePackageSource(string name, string source, RepositorySourceTypePayload sourceType, RepositorySourceStatePayload sourceState, string? failure = default)
    {
        return new RepositorySourcePayload()
        {
            Failure = failure ?? default,
            Name = name,
            Source = source,
            SourceType = sourceType,
            State = sourceState
        };
    }
    public static RepositorySourceEntity CreateRepositorySourceEntity(string name, string source, RepositorySourceType sourceType, RepositorySourceState state)
        => new RepositorySourceEntity(
            new RepositorySourceName(name),
            CreateRepositorySourceInfo(source, sourceType),
            state
        );
    public static RepositorySourceInfo CreateRepositorySourceInfo(string source, RepositorySourceType sourceType)
        => new RepositorySourceInfo(
            sourceType == RepositorySourceType.Local ? new RepositorySourceLocal(source) : new RepositorySourceRemote(source),
            sourceType
        );
    public static PackageEntity CreatePackageEntity(string id,
    string title,
    string version,
    string panelVersion, PackageState state, string source, RepositorySourceType sourceType) => new PackageEntity(
        new PackageInfo(
            new PackageId(id),
            new PackageName(title),
            new PackageDescription("Package test description."),
            state),
        CreateRepositorySourceInfo(source, sourceType),
        new PackageVersion(version),
        new PackagePanelVersion(panelVersion),
        Array.Empty<PackageDependency>());
}

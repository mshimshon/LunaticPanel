using LunaticPanel.PackageManager.Domain.Entites.ValueObjects;

namespace LunaticPanel.PackageManager.Application.Services;

public interface IPackageService
{
    /*
        Solution is the following
        Infrastruicture Implement Application Service.
        Application Service only return Application Payloads.
        Infrastructure Implements Infrastructure Services Contracts.
        Application Injecting Services for Infra IRemotePackageSourceService which wraps the external services and Domain Repositories.
        Application has Payloads Mapping DTOs -> Entities and Entities to DTOs
        Infrastructure has Payloads prefixes ExternalNAMEResponse and own mapping between External and Application DTOs
        Infrastructure will use Application to convert Entities to Application DTO where needed.
        Infrastructure will convert from External DTOs to Entities where required.
     */

    ICollection<PackageInfo> SearchAsync(string q, CancellationToken ct = default);
    ICollection<PackageInfo> GetAvailableRollbackAsync(CancellationToken ct = default);

    // TODO: THIS GOES INTO REPOSITORY
    //Task InstallAsync(PackageId id, PackageVersion version, RepositorySourceInfo sourceInfo, CancellationToken ct = default);
    //Task UpdateAsync(PackageId id,PackageVersion currentVersion, PackageVersion targetVersion, RepositorySourceInfo sourceInfo, CancellationToken ct = default);
    //Task DeleteAsync(PackageId id, CancellationToken ct = default);
    //Task EnableAsync(PackageId id, CancellationToken ct = default);
    //Task DisableAsync(PackageId id, CancellationToken ct = default);
}

using LunaticPanel.PackageManager.Domain.Entites.ValueObjects;

namespace LunaticPanel.PackageManager.Application.Services;

public interface IPackageService
{
    /*
        Solution is the following
        Application Implements Application Use-Case Services.
        Infrastructure Implements Infrastructure Services Contracts.
        Application Injecting Services for Infra IRemotePackageSourceService which wraps the external services and Domain Repositories.
        Application has Payloads Mapping DTOs -> Entities and Entities to DTOs
        Infrastructure has Payloads prefixes ExternalNAMEResponse and own mapping between External and Application DTOs
        Infrastructure will use Application to convert Entities to Application DTO where needed.
        Infrastructure will convert from External DTOs to Entities where required.
     */

    ICollection<PackageInfo> SearchAsync(string q, CancellationToken ct = default);
    Task InstallAsync(PackageId id, PackageVersion version, RepositorySourceInfo sourceInfo, CancellationToken ct = default);
    ICollection<PackageInfo> GetAvailableRollbackAsync(CancellationToken ct = default);

    // TODO: DECIDE WHERE IT GOES
    //Task DeleteAsync(PackageId id, CancellationToken ct = default);
    //Task EnableAsync(PackageId id, CancellationToken ct = default);
    //Task DisableAsync(PackageId id, CancellationToken ct = default);
    //Task UpdateAsync(PackageId id, PackageVersion version, RepositorySourceInfo sourceInfo, CancellationToken ct = default);
}

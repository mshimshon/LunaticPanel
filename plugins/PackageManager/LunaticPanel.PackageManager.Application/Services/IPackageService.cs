using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Payloads.Requests;

namespace LunaticPanel.PackageManager.Application.Services;

public interface IPackageService
{
    /*
        Solution is the following
        Infrastructure Implement Application Service.
        Application Service only return Application Payloads.
        Infrastructure Implements Infrastructure Services Contracts.
        Application Injecting Services for Infra IRemotePackageSourceService which wraps the external services and Domain Repositories.
        Application has Payloads Mapping DTOs -> Entities and Entities to DTOs
        Infrastructure has Payloads prefixes ExternalNAMEResponse and own mapping between External and Application DTOs
        Infrastructure will use Application to convert Entities to Application DTO where needed.
        Infrastructure will convert from External DTOs to Entities where required.
     */

    Task<ICollection<PackagePayload>> SearchAsync(SearchRequest data, CancellationToken ct = default);
}

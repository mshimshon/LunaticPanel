using LuncaticPanel.Package.Server.Application.Payloads.Responses;

namespace LuncaticPanel.Package.Server.Application.Services;

public interface IPackageValidatorService
{
    Task<PackageValidationResponse> ValidateRemoteAsync(string target, CancellationToken ct = default);
    Task<PackageValidationResponse> ValidateLocalAsync(string target, CancellationToken ct = default);
}

using LunaticPanel.Package.Server.Application.Payloads;
using LunaticPanel.Package.Server.Application.Payloads.Requests;
using LunaticPanel.Package.Server.Application.Payloads.Responses;

namespace LunaticPanel.Package.Server.Application.Services;

public interface IPackageUpload
{
    public Task<PackageUploadAccessResponse> RequestUploadRouteAsync(CancellationToken ct = default);
    public Task ConfirmNotInQueueAsync(ManifestPayload manifest, CancellationToken ct = default);
    public Task QueuePackageForValidationAsync(PackageUploadRequest data, CancellationToken ct = default);
}

using LunaticPanel.Package.Server.Application.Payloads;
using LunaticPanel.Package.Server.Application.Payloads.Responses;

namespace LunaticPanel.Package.Server.Application.Services;

public interface IPackageUploader
{
    public Task<ManifestPayload> GetManifestFromUploadIdAsync(string uploadId, CancellationToken ct = default);
    public Task<PackageUploadAccessResponse> RequestUploadRouteAsync(ManifestPayload manifest, CancellationToken ct = default);
    public Task ConfirmNotInQueueAsync(ManifestPayload manifest, CancellationToken ct = default);
    public Task QueuePackageForValidationAsync(string uploadId, CancellationToken ct = default);
}

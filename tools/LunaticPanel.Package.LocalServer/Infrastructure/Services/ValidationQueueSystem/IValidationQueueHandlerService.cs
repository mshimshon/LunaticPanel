using LunaticPanel.Package.Server.Application.Payloads;
using LunaticPanel.Package.Server.Application.Payloads.Responses;

namespace LunaticPanel.Package.LocalServer.Infrastructure.Services.ValidationQueueSystem;

public interface IValidationQueueHandlerService
{
    Task ValidateUploadIdAsync(string uploadId, CancellationToken ct = default);
    Task<bool> HasExistingPendingUploadAsync(ManifestPayload manifestPayload, CancellationToken ct = default);
    Task QueueUpAsync(ManifestPayload manifestPayload, CancellationToken ct = default);
    Task<string> GenerateUploadIdAsync(ManifestPayload manifest, CancellationToken ct = default);
    Task<ManifestPayload> GetManifestFromIdAsync(string uploadId, CancellationToken ct = default);
    Task CacheUploadRouteAsync(PackageUploadAccessResponse uploadRoute, CancellationToken ct = default);
    Task AssociateUploadIdToFileAsync(string uploadId, string path, CancellationToken ct = default);
}

using LunaticPanel.Package.LocalServer.Infrastructure.Exceptions;
using LunaticPanel.Package.LocalServer.Infrastructure.Services.ValidationQueueSystem;
using LunaticPanel.Package.Server.Application.Payloads;
using LunaticPanel.Package.Server.Application.Payloads.Responses;
using LunaticPanel.Package.Server.Application.Services;

namespace LunaticPanel.Package.LocalServer.Infrastructure.Services;

public class PackageUploaderService : IPackageUploader
{
    private readonly IValidationQueueHandlerService _validationQueueHandlerService;

    public PackageUploaderService(IValidationQueueHandlerService validationQueueHandlerService)
    {
        _validationQueueHandlerService = validationQueueHandlerService;
    }


    public async Task ConfirmNotInQueueAsync(ManifestPayload manifest, CancellationToken ct = default)
    {
        bool alreadyInQueue = await _validationQueueHandlerService.HasExistingPendingUploadAsync(manifest, ct);
        if (alreadyInQueue)
            throw new InfrastructureException("AlreadyInQueue", "The package is already in queue for validation.");
    }

    public async Task<ManifestPayload> GetManifestFromUploadIdAsync(string uploadId, CancellationToken ct = default)
        => await _validationQueueHandlerService.GetManifestFromIdAsync(uploadId, ct);
    public Task QueuePackageForValidationAsync(string uploadId, CancellationToken ct = default)
        => throw new NotImplementedException();

    public async Task<PackageUploadAccessResponse> RequestUploadRouteAsync(ManifestPayload manifest, CancellationToken ct = default)
    {
        var uploadId = await _validationQueueHandlerService.GenerateUploadIdAsync(manifest, ct);
        var result = new PackageUploadAccessResponse(uploadId, $"/v1/upload", new());
        await _validationQueueHandlerService.CacheUploadRouteAsync(result, ct);
        return result;
    }
}

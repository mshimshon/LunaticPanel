using LunaticPanel.Package.Server.Application.Exceptions;
using LunaticPanel.Package.Server.Application.Mediator.Engine;
using LunaticPanel.Package.Server.Application.Payloads.Mapping;
using LunaticPanel.Package.Server.Application.Services;
using LunaticPanel.Package.Server.Domain.Repositories;

namespace LunaticPanel.Package.Server.Application.Mediator.Commands.Handlers;

internal class PackageUploadHandler : IRequestHandler<PackageUploadCommand>
{
    private readonly IPackageUpload _packageUpload;
    private readonly IManifestReadRepository _manifestReadRepository;

    public PackageUploadHandler(IPackageUpload packageUpload, IManifestReadRepository manifestReadRepository)
    {
        _packageUpload = packageUpload;
        _manifestReadRepository = manifestReadRepository;
    }
    public async Task HandleAsync(PackageUploadCommand data, CancellationToken ct = default)
    {
        var entity = data.Data.Manifest.ToDomain();
        bool packageMissing = false;
        // Throw if not found
        try
        {
            await _manifestReadRepository.GetAsync(entity.Id, entity.Version, ct);
        }
        catch (Exception)
        {
            packageMissing = true;
        }

        if (!packageMissing)
            throw new PackageAlreadyAvailableException();

        await _packageUpload.ConfirmNotInQueueAsync(data.Data.Manifest, ct);
        await _packageUpload.QueuePackageForValidationAsync(data.Data, ct);
    }
}

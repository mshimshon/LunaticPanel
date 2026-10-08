using LunaticPanel.Package.Server.Application.Exceptions;
using LunaticPanel.Package.Server.Application.Mediator.Engine;
using LunaticPanel.Package.Server.Application.Payloads.Mapping;
using LunaticPanel.Package.Server.Application.Payloads.Responses;
using LunaticPanel.Package.Server.Application.Services;
using LunaticPanel.Package.Server.Domain.Repositories;

namespace LunaticPanel.Package.Server.Application.Mediator.Queries.Handlers;

internal class GetUploadAccessHandler : IRequestHandler<GetUploadAccessQuery, PackageUploadAccessResponse>
{
    private readonly IPackageUploader _packageUpload;
    private readonly IManifestReadRepository _manifestReadRepository;

    public GetUploadAccessHandler(IPackageUploader packageUpload, IManifestReadRepository manifestReadRepository)
    {
        _packageUpload = packageUpload;
        _manifestReadRepository = manifestReadRepository;
    }
    public async Task<PackageUploadAccessResponse> HandleAsync(GetUploadAccessQuery data, CancellationToken ct = default)
    {
        var entity = data.Manifest.ToDomain();
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

        await _packageUpload.ConfirmNotInQueueAsync(data.Manifest, ct);
        return await _packageUpload.RequestUploadRouteAsync(data.Manifest, ct);
    }

}

using LunaticPanel.Package.Server.Application.Payloads;
using LunaticPanel.Package.Server.Application.Payloads.Responses;
using LunaticPanel.Package.Server.Application.Services;
using LunaticPanel.Package.Server.Domain.Entites.ValueObjects;
using MaksimShimshon.Mediator;

namespace LunaticPanel.Package.Server.Application.Mediator.Queries.Handlers;

internal sealed class GetPackageDownloadTargetHandler : IRequestHandler<GetPackageDownloadTargetQuery, PackageDownloadTargetResponse>
{
    private readonly IMediator _mediator;
    private readonly IPackageDownloadResolver _downloadResolver;

    public GetPackageDownloadTargetHandler(IMediator mediator, IPackageDownloadResolver downloadResolver)
    {
        _mediator = mediator;
        _downloadResolver = downloadResolver;
    }
    public async Task<PackageDownloadTargetResponse> HandleAsync(GetPackageDownloadTargetQuery data, CancellationToken ct = default)
    {
        var id = new PackageId(data.Id);
        var version = new PackageVersion(data.Version);
        ManifestPayload manifest = await _mediator.ExecuteAsync(new GetSpecificPackageVersionQuery(id.Value, version.Value));
        return await _downloadResolver.GetDownloadLocationAsync(id, version, ct);
    }
}

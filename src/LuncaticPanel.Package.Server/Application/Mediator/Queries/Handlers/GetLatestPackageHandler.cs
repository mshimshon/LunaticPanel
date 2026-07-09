using LuncaticPanel.Package.Server.Application.Mediator.Engine;
using LuncaticPanel.Package.Server.Application.Payloads;

namespace LuncaticPanel.Package.Server.Application.Mediator.Queries.Handlers;

internal sealed class GetLatestPackageHandler : IRequestHandler<GetLatestPackageQuery, ManifestPayload>
{
    public Task<ManifestPayload> HandleAsync(GetLatestPackageQuery data, CancellationToken ct = default)
        => throw new NotImplementedException();
}

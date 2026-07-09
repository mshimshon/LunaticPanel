using LuncaticPanel.Package.Server.Application.Mediator.Engine;
using LuncaticPanel.Package.Server.Application.Payloads;

namespace LuncaticPanel.Package.Server.Application.Mediator.Queries.Handlers;

internal sealed class GetSpecificPackageVersionHandler : IRequestHandler<GetSpecificPackageVersionQuery, ManifestPayload>
{
    public Task<ManifestPayload> HandleAsync(GetSpecificPackageVersionQuery data, CancellationToken ct = default) => throw new NotImplementedException();
}

using LunaticPanel.Package.Server.Application.Payloads;
using LunaticPanel.Package.Server.Application.Payloads.Responses;
using MaksimShimshon.Mediator;

namespace LunaticPanel.Package.Server.Application.Mediator.Queries;

public sealed record GetUploadAccessQuery(ManifestPayload Manifest) : IRequest<PackageUploadAccessResponse>
{
}

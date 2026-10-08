using LunaticPanel.Package.Server.Application.Mediator.Engine;
using LunaticPanel.Package.Server.Application.Payloads;
using LunaticPanel.Package.Server.Application.Payloads.Responses;

namespace LunaticPanel.Package.Server.Application.Mediator.Queries;

public sealed record GetUploadAccessQuery(ManifestPayload Manifest) : IRequest<PackageUploadAccessResponse>
{
}

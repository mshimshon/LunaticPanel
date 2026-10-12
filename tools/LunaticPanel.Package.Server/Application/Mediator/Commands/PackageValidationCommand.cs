using LunaticPanel.Package.Server.Application.Payloads.Requests;
using LunaticPanel.Package.Server.Application.Payloads.Responses;
using MaksimShimshon.Mediator;

namespace LunaticPanel.Package.Server.Application.Mediator.Commands;

public sealed record PackageValidationCommand(PackageValidationRequest Data) : IRequest<PackageValidationResponse>
{
}

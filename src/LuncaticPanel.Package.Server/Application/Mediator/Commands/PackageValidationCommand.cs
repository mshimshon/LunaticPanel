using LuncaticPanel.Package.Server.Application.Mediator.Engine;
using LuncaticPanel.Package.Server.Application.Payloads.Requests;
using LuncaticPanel.Package.Server.Application.Payloads.Responses;

namespace LuncaticPanel.Package.Server.Application.Mediator.Commands;

public sealed record PackageValidationCommand(PackageValidationRequest Data) : IRequest<PackageValidationResponse>
{
}

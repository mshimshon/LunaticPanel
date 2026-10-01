using LuncaticPanel.Package.Server.Application.Mediator.Commands.Exceptions;
using LuncaticPanel.Package.Server.Application.Payloads.Requests;

namespace LuncaticPanel.Package.Server.Application.Services;

public interface IPackageValidationEvents
{
    Task OnValidationFailure(PackageValidationRequest request, PackageValidationException ex);
}

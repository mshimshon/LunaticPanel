using LunaticPanel.Package.Server.Application.Mediator.Commands.Exceptions;
using LunaticPanel.Package.Server.Application.Payloads.Requests;

namespace LunaticPanel.Package.Server.Application.Services;

public interface IPackageValidationEvents
{
    Task OnValidationFailure(PackageValidationRequest request, PackageValidationException ex);
}

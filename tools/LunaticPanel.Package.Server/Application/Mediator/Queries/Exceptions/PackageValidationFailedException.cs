using LunaticPanel.Package.Server.Application.Exceptions;

namespace LunaticPanel.Package.Server.Application.Mediator.Queries.Exceptions;

public class PackageValidationFailedException : AppLayerException
{
    public PackageValidationFailedException(string failure) :
        base(nameof(PackageValidationFailedException), $"Package validation failed with '{failure}'.")
    {
    }
}

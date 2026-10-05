using LunaticPanel.Package.Server.Domain.Exceptions;

namespace LunaticPanel.Package.Server.Domain.Validators.Exceptions;

public sealed class PackageTitleNullException : DomainCodedException
{
    public PackageTitleNullException() :
        base(nameof(PackageTitleNullException), "Package Title cannot be null.")
    {
    }
}

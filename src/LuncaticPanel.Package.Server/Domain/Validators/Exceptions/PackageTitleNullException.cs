using LuncaticPanel.Package.Server.Domain.Exceptions;

namespace LuncaticPanel.Package.Server.Domain.Validators.Exceptions;

public sealed class PackageTitleNullException : DomainCodedException
{
    public PackageTitleNullException() :
        base(nameof(PackageTitleNullException), "Package Title cannot be null.")
    {
    }
}

using LuncaticPanel.Package.Server.Domain.Exceptions;

namespace LuncaticPanel.Package.Server.Application.Validators.Exceptions;

public sealed class PackageEntryFileDllViolationException : DomainCodedException
{
    public PackageEntryFileDllViolationException(string dll) :
        base(nameof(PackageEntryFileDllViolationException), $"'{dll}' must be a .dll file name.")
    {
    }
}

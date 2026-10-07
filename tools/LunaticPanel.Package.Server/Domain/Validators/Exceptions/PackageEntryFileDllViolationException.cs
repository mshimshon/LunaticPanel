using LunaticPanel.Package.Server.Domain.Exceptions;

namespace LunaticPanel.Package.Server.Domain.Validators.Exceptions;

public sealed class PackageEntryFileDllViolationException : DomainCodedException
{
    public PackageEntryFileDllViolationException(string dll) :
        base(nameof(PackageEntryFileDllViolationException), $"'{dll}' must be a .dll file name.")
    {
    }
}

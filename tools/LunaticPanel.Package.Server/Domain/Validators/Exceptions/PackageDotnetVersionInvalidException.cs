using LunaticPanel.Package.Server.Domain.Entites;
using LunaticPanel.Package.Server.Domain.Exceptions;

namespace LunaticPanel.Package.Server.Domain.Validators.Exceptions;

public sealed class PackageDotnetVersionInvalidException : DomainCodedException
{
    public PackageDotnetVersionInvalidException(string dotnetVersion) :
        base(nameof(PackageDotnetVersionInvalidException), $"'{dotnetVersion}' {nameof(ManifestEntity.DotnetVersion)} is not valid.")
    {
    }
}

using LuncaticPanel.Package.Server.Domain.Entites;
using LuncaticPanel.Package.Server.Domain.Exceptions;

namespace LuncaticPanel.Package.Server.Application.Validators.Exceptions;

public sealed class PackageDotnetVersionInvalidException : DomainCodedException
{
    public PackageDotnetVersionInvalidException(string dotnetVersion) :
        base(nameof(PackageDotnetVersionInvalidException), $"'{dotnetVersion}' {nameof(ManifestEntity.DotnetVersion)} is not valid.")
    {
    }
}

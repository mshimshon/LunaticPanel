using LuncaticPanel.Package.Server.Domain.Exceptions;
using LuncaticPanel.Package.Server.Domain.Query;

namespace LuncaticPanel.Package.Server.Domain.Validators.Exceptions;

public sealed class QueryKeywordsViolationException : DomainCodedException
{
    public QueryKeywordsViolationException() :
        base(nameof(QueryKeywordsViolationException), $"{nameof(ManifestQueryModel.Keywords)} violates policy a-Z, spaces, dots and 0-9 allowed.")
    {
    }
}

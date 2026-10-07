using LunaticPanel.Package.Server.Domain.Exceptions;
using LunaticPanel.Package.Server.Domain.Query;

namespace LunaticPanel.Package.Server.Domain.Validators.Exceptions;

public sealed class QueryKeywordsViolationException : DomainCodedException
{
    public QueryKeywordsViolationException() :
        base(nameof(QueryKeywordsViolationException), $"{nameof(ManifestQueryModel.Keywords)} violates policy a-Z, spaces, dots and 0-9 allowed.")
    {
    }
}

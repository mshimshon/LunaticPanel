using LunaticPanel.Package.Server.Domain.Exceptions;
using LunaticPanel.Package.Server.Domain.Query;

namespace LunaticPanel.Package.Server.Domain.Validators.Exceptions;

public sealed class QueryKeywordsEmptyException : DomainCodedException
{
    public QueryKeywordsEmptyException() :
        base(nameof(QueryKeywordsEmptyException), $"{nameof(ManifestQueryModel.Keywords)} when set cannot be empty.")
    {
    }
}

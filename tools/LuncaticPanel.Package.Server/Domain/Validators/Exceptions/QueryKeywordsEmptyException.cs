using LuncaticPanel.Package.Server.Domain.Exceptions;
using LuncaticPanel.Package.Server.Domain.Query;

namespace LuncaticPanel.Package.Server.Domain.Validators.Exceptions;

public sealed class QueryKeywordsEmptyException : DomainCodedException
{
    public QueryKeywordsEmptyException() :
        base(nameof(QueryKeywordsEmptyException), $"{nameof(ManifestQueryModel.Keywords)} when set cannot be empty.")
    {
    }
}

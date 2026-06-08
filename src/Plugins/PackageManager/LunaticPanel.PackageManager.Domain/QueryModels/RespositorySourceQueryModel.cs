using LunaticPanel.PackageManager.Domain.Entites.Enums;
using LunaticPanel.PackageManager.Domain.QueryModels.Interfaces;

namespace LunaticPanel.PackageManager.Domain.QueryModels;

public sealed record RespositorySourceQueryModel : QueryModel, IRespositorySourceQueryModel
{

    private readonly List<RepositorySourceType> _repositorySourceTypes = new List<RepositorySourceType>();
    public IReadOnlyList<RepositorySourceType> RepositorySourceTypes => _repositorySourceTypes.AsReadOnly();

    private readonly List<string> _keywords = new List<string>();
    public IReadOnlyList<string> Keywords => _keywords.AsReadOnly();

    public RespositorySourceQueryModel FilterBySourceType(params RepositorySourceType[] repositorySourceTypes)
    {
        _repositorySourceTypes.AddRange(repositorySourceTypes);
        return this;
    }

    public RespositorySourceQueryModel SearchByKeywords(string keywords)
    {
        var multipleKeys = keywords.Split(' ');
        _keywords.AddRange(multipleKeys);
        return this;
    }
}

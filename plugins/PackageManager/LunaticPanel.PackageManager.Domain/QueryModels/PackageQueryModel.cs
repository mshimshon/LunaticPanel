using LunaticPanel.PackageManager.Domain.Entities.Enums;
using LunaticPanel.PackageManager.Domain.Entities.ValueObjects;
using LunaticPanel.PackageManager.Domain.QueryModels;
using LunaticPanel.PackageManager.Domain.QueryModels.Enums;
using LunaticPanel.PackageManager.Domain.QueryModels.Interfaces;

namespace LunaticPanel.PackageManager.Domain.QueryCriterias;

public sealed record PackageQueryModel : QueryModel, IPackageQueryModel
{

    public PackageVersion? SpecificVersion { get; private set; }
    public ValueComparision SpecificVersionComparision { get; private set; }
    public PackageQueryModel OnlyVersion(PackageVersion packageVersion, ValueComparision comparision)
    {
        SpecificVersion = packageVersion;
        SpecificVersionComparision = comparision;
        return this;
    }

    private List<RepositorySourceInfo> _repositorySources = new List<RepositorySourceInfo>();
    public IReadOnlyList<RepositorySourceInfo> RepositorySources => _repositorySources.AsReadOnly();
    public PackageQueryModel FilterBySources(params RepositorySourceInfo[] sourceInfo)
    {
        _repositorySources.AddRange(sourceInfo);
        return this;
    }

    private List<PackageState> _packageStates = new List<PackageState>();
    public IReadOnlyList<PackageState> PackageStates => _packageStates.AsReadOnly();
    public PackageQueryModel FilterByStates(params PackageState[] packageStates)
    {
        _packageStates.AddRange(packageStates);
        return this;
    }

    private List<PackageId> _packageIds = new List<PackageId>();
    public IReadOnlyList<PackageId> PackageIds => _packageIds.AsReadOnly();
    public PackageQueryModel FilterByIds(params PackageId[] packageIds)
    {
        _packageIds.AddRange(packageIds);
        return this;
    }

    private List<string> _keywords = new List<string>();
    public IReadOnlyList<string> Keywords => _keywords.AsReadOnly();
    public PackageQueryModel SearchByKeywords(string keywords)
    {
        var multipleKeys = keywords.Split(' ');
        _keywords.AddRange(multipleKeys);
        return this;
    }

}

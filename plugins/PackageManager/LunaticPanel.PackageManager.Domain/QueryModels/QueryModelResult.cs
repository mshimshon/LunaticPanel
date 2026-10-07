using LunaticPanel.PackageManager.Domain.QueryModels.Interfaces;

namespace LunaticPanel.PackageManager.Domain.QueryModels;

public sealed record QueryModelResult<T> : IQueryModelResult<T> where T : class
{
    public ICollection<T> Result { get; init; } = new List<T>();

    public int Position { get; init; }

    public int Total { get; init; }
}

using LunaticPanel.PackageManager.Domain.QueryModels.Interfaces;

namespace LunaticPanel.PackageManager.Domain.QueryModels;

public abstract record QueryModel : IQueryModel
{
    public int Position { get; }
    public int MaxResult { get; }
    protected QueryModel()
    {
        MaxResult = 25;
    }
    protected QueryModel(int position)
    {
        Position = position;
    }
    protected QueryModel(int position, int maxResult) : this(position)
    {
        MaxResult = maxResult > 50 ? 50 : maxResult < 10 ? 10 : maxResult;
    }
}

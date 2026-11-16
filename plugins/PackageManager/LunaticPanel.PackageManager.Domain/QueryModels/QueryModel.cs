using LunaticPanel.PackageManager.Domain.QueryModels.Interfaces;

namespace LunaticPanel.PackageManager.Domain.QueryModels;

public abstract record QueryModel : IQueryModel
{
    public const int DEFAULT_MAX_RESULT = 25;
    public const int MAX_CEILING_RESULT = 50;
    public const int MIN_FLOORING_RESULT = 10;
    public int Position { get; }
    public int MaxResult { get; }
    protected QueryModel()
    {
        MaxResult = DEFAULT_MAX_RESULT;
    }
    protected QueryModel(int position)
    {
        Position = position < 0 ? 0 : position;
    }
    protected QueryModel(int position, int maxResult) : this(position)
    {
        MaxResult = maxResult > MAX_CEILING_RESULT ? MAX_CEILING_RESULT : maxResult < MIN_FLOORING_RESULT ? MIN_FLOORING_RESULT : maxResult;
    }
}

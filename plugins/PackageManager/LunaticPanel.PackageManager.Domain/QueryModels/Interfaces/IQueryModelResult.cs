namespace LunaticPanel.PackageManager.Domain.QueryModels.Interfaces;

public interface IQueryModelResult<T> where T : class
{
    public ICollection<T> Result { get; }
    public int Position { get; }
    public int Total { get; }
}

namespace LuncaticPanel.Package.Server.Domain.Query;

public interface IQueryResultModel<T> where T : class
{
    public ICollection<T> Result { get; }
    public int Position { get; }
    public int Total { get; }
}

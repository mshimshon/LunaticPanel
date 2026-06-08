using System.Linq.Expressions;

namespace LunaticPanel.PackageManager.Domain.Respositories;

public interface ISpecification<T> where
    T : class
{
    Expression<Func<T, bool>> Criteria { get; }
}
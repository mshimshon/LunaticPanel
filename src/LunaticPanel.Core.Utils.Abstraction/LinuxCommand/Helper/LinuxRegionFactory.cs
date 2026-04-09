using System.Linq.Expressions;
using System.Reflection;

namespace LunaticPanel.Core.Utils.Abstraction.LinuxCommand.Extensions;

internal static class RegionFactory<T> where T : LinuxRegionContext, new()
{
    public static readonly Action<T, ILinuxCommand> AssignLinuxCommand;

    static RegionFactory()
    {
        var prop = typeof(T).GetProperty(
            nameof(LinuxRegionContext.LinuxCommand),
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
        );

        if (prop == null || !prop.CanWrite)
            throw new InvalidOperationException(
                $"{typeof(T).Name} must define writable LinuxCommand property"
            );

        var instanceParam = Expression.Parameter(typeof(T));
        var commandParam = Expression.Parameter(typeof(ILinuxCommand));

        var assign = Expression.Assign(
            Expression.Property(instanceParam, prop),
            commandParam
        );

        AssignLinuxCommand = Expression
            .Lambda<Action<T, ILinuxCommand>>(assign, instanceParam, commandParam)
            .Compile();
    }
}



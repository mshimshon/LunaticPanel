using LunaticPanel.Package.LocalServer.Infrastructure.EntityFramework.Models.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace LunaticPanel.Package.LocalServer.Infrastructure.EntityFramework.Extensions;

internal static class EntityEntryExt
{
    public static void Timestamp(this EntityEntry item)
    {
        var hasContract = typeof(IModelTimestamps).IsAssignableFrom(item.Entity.GetType());
        if (!hasContract) return;
        var entity = (IModelTimestamps)item.Entity;
        if (item.State == EntityState.Modified)
            entity.Updated = DateTime.UtcNow;
        if (item.State == EntityState.Added)
        {
            entity.Updated = DateTime.UtcNow;
            entity.Created = DateTime.UtcNow;
        }

    }
}

using LunaticPanel.PackageManager.Domain.Entites.ValueObjects;

namespace LunaticPanel.PackageManager.Application.Services;

public class PackageService : IPackageService
{
    public PackageService()
    {

    }
    public ICollection<PackageInfo> GetAvailableRollbackAsync(CancellationToken ct = default) => throw new NotImplementedException();
    public Task InstallAsync(PackageId id, PackageVersion version, RepositorySourceInfo sourceInfo, CancellationToken ct = default) => throw new NotImplementedException();
    public ICollection<PackageInfo> SearchAsync(string q, CancellationToken ct = default) => throw new NotImplementedException();
}

using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Payloads.Requests;
using LunaticPanel.PackageManager.Application.Services;

namespace LunaticPanel.PackageManager.Tests.Application.Mock;

internal class PackageService : IPackageService
{
    List<PackagePayload> _cache = new()
    {
        new PackagePayload(){ Info = new()
        {
            Name = "Package Test",
            Description = "No Descriptions",
            PackageId = "Package.Test",
            State = PackageManager.Application.Payloads.Enums.PackageStatePayload.Disabled,
        },
            RepositorySource = "local://myfolder/", RepositoryType = PackageManager.Application.Payloads.Enums.RepositorySourceTypePayload.Local,
            Version = "0.0.1"
        },
        new PackagePayload(){ Info = new()
        {
            Name = "Package Test 2",
            Description = "No Descriptions",
            PackageId = "Package.Test.Two",
            State = PackageManager.Application.Payloads.Enums.PackageStatePayload.Disabled,
        },
            RepositorySource = "local://myfolder/", RepositoryType = PackageManager.Application.Payloads.Enums.RepositorySourceTypePayload.Local,
            Version = "0.2.1"
        }
    };
    List<PackagePayload> _cacheRollbacks = new()
    {

    };
    public Task CreateRollbackAsync(PackagePayload data, CancellationToken ct = default)
    {
        _cacheRollbacks.RemoveAll(p => p.Info.PackageId == data.Info.PackageId);
        _cacheRollbacks.Add(data);
        return Task.CompletedTask;
    }

    public Task<ICollection<PackagePayload>> GetAvailableRollbackAsync(CancellationToken ct = default)
    {
        ICollection<PackagePayload> result = _cacheRollbacks.ToList();
        return Task.FromResult(result);
    }

    public Task<ICollection<PackagePayload>> SearchAsync(SearchRequest data, CancellationToken ct = default)
    {
        ICollection<PackagePayload> result = _cache.ToList();
        return Task.FromResult(result);
    }
}

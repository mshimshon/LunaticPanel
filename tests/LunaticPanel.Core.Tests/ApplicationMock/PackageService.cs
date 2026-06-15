using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Services;

namespace LunaticPanel.Core.Tests.ApplicationMock;

internal class PackageService : IPackageService
{
    List<PackagePayload> _cache = new()
    {
        new PackagePayload(){ Info = new()
        {
            Name = "Package Test",
            Description = "No Descriptions",
            PackageId = "Package.Test",
            State = PackageManager.Application.Payloads.Enums.PackageStatePayload.Unknown,
        },
            RepositorySource = "local://myfolder/", RepositoryType = PackageManager.Application.Payloads.Enums.RepositorySourceTypePayload.Local,
            Version = "0.0.1"
        },
        new PackagePayload(){ Info = new()
        {
            Name = "Package Test 2",
            Description = "No Descriptions",
            PackageId = "Package.Test.Two",
            State = PackageManager.Application.Payloads.Enums.PackageStatePayload.Unknown,
        },
            RepositorySource = "local://myfolder/", RepositoryType = PackageManager.Application.Payloads.Enums.RepositorySourceTypePayload.Local,
            Version = "0.2.1"
        }
    };
    public Task<ICollection<PackagePayload>> GetAvailableRollbackAsync(CancellationToken ct = default)
    {
        ICollection<PackagePayload> result = _cache.ToList();
        return Task.FromResult(result);
    }

    public Task<ICollection<PackagePayload>> SearchAsync(string q, CancellationToken ct = default)
    {
        ICollection<PackagePayload> result = _cache.ToList();
        return Task.FromResult(result);
    }
}

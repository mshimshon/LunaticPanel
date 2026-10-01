using LunaticPanel.Package.LocalServer.Infrastructure.Extensions;
using LunaticPanel.Package.LocalServer.Infrastructure.LunaPackage.Exceptions;
using LuncaticPanel.Package.Server.Application.Payloads.Responses;
using LuncaticPanel.Package.Server.Application.Services;
using LuncaticPanel.Package.Server.Domain.Entites.ValueObjects;

namespace LunaticPanel.Package.LocalServer.Infrastructure.LunaPackage;

internal sealed class PackageDownloadResolver : IPackageDownloadResolver
{
    public IHttpContextAccessor HttpContextAccessor { get; }

    public PackageDownloadResolver(IHttpContextAccessor httpContextAccessor)
    {
        HttpContextAccessor = httpContextAccessor;
    }


    public Task<PackageDownloadTargetResponse> GetDownloadLocationAsync(PackageId packageId, PackageVersion packageVersion, CancellationToken ct = default)
    {
        var filename = $"{packageId.Value}.{packageVersion.Value}";
        var encodedFilename = $"{filename.ToBase32()}.lpkg";
        var req = HttpContextAccessor.HttpContext!.Request;
        string baseUrl = $"{req.Scheme}://{req.Host}{req.PathBase}";
        string? append = default;
        foreach (var item in InfrastructureServiceRegisterExt.PackageStorageUpload)
        {
            var file = Path.Combine(item.Value, encodedFilename);
            if (File.Exists(file))
            {
                append = $"{item.Value.ToBase32()}/{encodedFilename}";
                break;
            }
        }
        if (append == default)
            throw new PackageFileNotFoundException(packageId, packageVersion);
        var result = new PackageDownloadTargetResponse()
        {
            Target = $"{baseUrl}/{append}"
        };
        return Task.FromResult(result);
    }
}

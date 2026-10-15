using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Services;
using LunaticPanel.PackageManager.Infrastructure.Extensions;
using LunaticPanel.PackageManager.Infrastructure.Services.Payloads.Mapping;

namespace LunaticPanel.PackageManager.Infrastructure.Services;

internal class PackageFileReader : IPackageFileReader
{
    public PackagePayload GetPackageInformation(string file) => PackageFileExt.GetPackageInformation(file);
    public PackageManifestPayload ReadManifestFromArchive(string input) => PackageFileExt.ReadManifestFromArchive(input).ToApplicationManifest();
}

using LunaticPanel.PackageManager.Application.Payloads;

namespace LunaticPanel.PackageManager.Application.Services;

public interface IPackageFileReader
{
    PackagePayload GetPackageInformation(string file);
    PackageManifestPayload ReadManifestFromArchive(string input);
}

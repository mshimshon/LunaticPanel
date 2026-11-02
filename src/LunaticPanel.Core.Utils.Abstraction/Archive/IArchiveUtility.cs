using System;

namespace LunaticPanel.Core.Utils.Abstraction.Archive;

public interface IArchiveUtility
{
    Task<Stream> ReadFile(string archivePath, string relativePath, ArchiveType type);
    Task<T?> ReadJson<T>(string archivePath, string relativePath, ArchiveType type);
    Task ExtractTo(string archivePath, string destination, ArchiveType type);
    Task PackTarGz(string inputFileOrFolder, string outputFile, CancellationToken ct = default);
    Task PackZip(string inputFileOrFolder, string outputFile, CancellationToken ct = default);

}

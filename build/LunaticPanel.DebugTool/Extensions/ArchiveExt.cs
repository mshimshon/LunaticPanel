using System.Formats.Tar;
using System.IO.Compression;

namespace LunaticPanel.DebugTool.Extensions;

internal static class ArchiveExt
{
    public static async Task CreateFileTarGzAsync(string inputFilePath, string outputTarGzPath, CancellationToken cancellationToken = default)
    {
        // Create the output stream with async support
        await using FileStream fs = new(outputTarGzPath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true);
        await using GZipStream gz = new(fs, CompressionMode.Compress, leaveOpen: true);
        await using var tarWriter = new TarWriter(gz);

        // Asynchronously write the single file entry
        await tarWriter.WriteEntryAsync(inputFilePath, Path.GetFileName(inputFilePath), cancellationToken);
    }
    public static async Task CreateFolderTarGzAsync(string inputFolderPath, string outputTarGzPath, CancellationToken cancellationToken = default)
    {
        await using FileStream fs = new(outputTarGzPath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true);
        await using GZipStream gz = new(fs, CompressionMode.Compress, leaveOpen: true);

        // includeBaseDirectory: false ensures the root folder name is NOT included in the archive
        await TarFile.CreateFromDirectoryAsync(inputFolderPath, gz, includeBaseDirectory: false, cancellationToken);
    }
    public static async Task CreateZipFolderAsync(string inputFolderPath, string outputZipPath, CancellationToken cancellationToken = default)
    {
        // includeBaseDirectory: false ensures the root folder name is NOT included in the archive
        await ZipFile.CreateFromDirectoryAsync(
            inputFolderPath,
            outputZipPath,
            CompressionLevel.Optimal,
            includeBaseDirectory: false,
            cancellationToken
        );
    }
    public static async Task CreateZipAsync(string inputFilePath, string outputZipPath, CancellationToken cancellationToken = default)
    {
        // Ensure the file exists before attempting to zip
        if (!File.Exists(inputFilePath))
            throw new FileNotFoundException("Input file not found.", inputFilePath);

        await using var archive = ZipFile.Open(outputZipPath, ZipArchiveMode.Create);

        // Asynchronously add the single file using only its filename
        await archive.CreateEntryFromFileAsync(inputFilePath, Path.GetFileName(inputFilePath), CompressionLevel.SmallestSize, cancellationToken);
    }
}

using System;
using System.Formats.Tar;
using System.IO.Compression;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LunaticPanel.Core.Extensions;

public static class ArchiveUtilityExt
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        ReferenceHandler = ReferenceHandler.IgnoreCycles
    };

    public static async Task ExtractTo(string archivePath, string destination, ArchiveType type)
    {

        if (type == ArchiveType.Zip || type == ArchiveType.LunaPackage)
        {
            await ZipFile.ExtractToDirectoryAsync(archivePath, destination, overwriteFiles: true);
            return;
        }

        using (FileStream fileStream = File.OpenRead(archivePath))
        {
            using (GZipStream gzipStream = new(fileStream, CompressionMode.Decompress))
            {
                await TarFile.ExtractToDirectoryAsync(gzipStream, destination, overwriteFiles: true);
            }
        }

    }

    public static async Task<Stream> ReadFile(string archivePath, string relativePath, ArchiveType type)
    {
        if (type == ArchiveType.Zip || type == ArchiveType.LunaPackage)
            return await ReadFileFromZip(archivePath, relativePath);
        return await ReadFileFromTarGz(archivePath, relativePath);
    }

    private static async Task<Stream> ReadFileFromZip(string archivePath, string relativePath)
    {
        using (var zip = ZipFile.OpenRead(archivePath))
        {
            var entry = zip.GetEntry(relativePath);
            if (entry == null)
                throw new FileNotFoundException($"{relativePath} not found in package");
            using (var stream = entry.Open())
            {
                var ms = new MemoryStream();
                stream.CopyTo(ms);
                ms.Position = 0;
                return ms;
            }
        }
    }

    private static async Task<Stream> ReadFileFromTarGz(string archivePath, string relativePath)
    {
        using var fs = new FileStream(archivePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        using var gzip = new GZipStream(fs, CompressionMode.Decompress);
        using var tar = new TarReader(gzip);

        TarEntry entry;
        while ((entry = await tar.GetNextEntryAsync()) != null)
        {
            if (entry.Name == relativePath && entry.DataStream != null)
            {
                var ms = new MemoryStream();
                await entry.DataStream.CopyToAsync(ms);
                ms.Position = 0; // Rewind the stream so it's ready to be read by the caller
                return ms; // Everything else disposes safely right here
            }
        }

        throw new FileNotFoundException($"{relativePath} not found in tar.gz package");
    }

    public static async Task<T?> ReadJson<T>(string archivePath, string relativePath, ArchiveType type)
    {
        try
        {
            var stream = await ReadFile(archivePath, relativePath, type);

            return JsonSerializer.Deserialize<T>(stream, _jsonSerializerOptions);
        }
        catch (System.Exception ex)
        {
            System.Console.Error.WriteLine($" Failure to read JSON for '{archivePath}'/'{relativePath}': {ex.Message}");
            return default(T);
        }

    }
    public static async Task PackZip(string inputFileOrFolder, string outputFile, CancellationToken ct = default)
    {
        if (Directory.Exists(inputFileOrFolder))
            await CreateZipAsync(inputFileOrFolder, outputFile, ct);
        else if (File.Exists(inputFileOrFolder))
            await CreateZipAsync(inputFileOrFolder, outputFile, ct);
        else throw new Exception($"'{inputFileOrFolder}' is neither a valid folder nor a valid file.");

    }

    public static async Task PackTarGz(string inputFileOrFolder, string outputFile, CancellationToken ct = default)
    {
        if (Directory.Exists(inputFileOrFolder))
            await CreateFolderTarGzAsync(inputFileOrFolder, outputFile, ct);
        else if (File.Exists(inputFileOrFolder))
            await CreateFileTarGzAsync(inputFileOrFolder, outputFile, ct);
        else throw new Exception($"'{inputFileOrFolder}' is neither a valid folder nor a valid file.");

    }
    private static async Task CreateFileTarGzAsync(string inputFilePath, string outputTarGzPath, CancellationToken ct = default)
    {
        // Create the output stream with async support
        await using (FileStream fs = new(outputTarGzPath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
        {
            await using (GZipStream gz = new(fs, CompressionMode.Compress, leaveOpen: true))
            {
                await using (var tarWriter = new TarWriter(gz))
                {
                    await tarWriter.WriteEntryAsync(inputFilePath, Path.GetFileName(inputFilePath), ct);
                }
            }
        }

    }

    private static async Task CreateFolderTarGzAsync(string inputFolderPath, string outputTarGzPath, CancellationToken ct = default)
    {
        await using (FileStream fs = new(outputTarGzPath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
        {
            await using (GZipStream gz = new(fs, CompressionMode.Compress, leaveOpen: true))
            {
                await TarFile.CreateFromDirectoryAsync(inputFolderPath, gz, includeBaseDirectory: false, ct);
            }
        }
    }
    private static async Task CreateZipFolderAsync(string inputFolderPath, string outputZipPath, CancellationToken ct = default)
    {
        // includeBaseDirectory: false ensures the root folder name is NOT included in the archive
        await ZipFile.CreateFromDirectoryAsync(
            inputFolderPath,
            outputZipPath,
            CompressionLevel.Optimal,
            includeBaseDirectory: false,
            ct
        );
    }
    private static async Task CreateZipAsync(string inputFilePath, string outputZipPath, CancellationToken ct = default)
    {
        // Ensure the file exists before attempting to zip
        if (!File.Exists(inputFilePath))
            throw new FileNotFoundException("Input file not found.", inputFilePath);

        await using var archive = ZipFile.Open(outputZipPath, ZipArchiveMode.Create);

        // Asynchronously add the single file using only its filename
        await archive.CreateEntryFromFileAsync(inputFilePath, Path.GetFileName(inputFilePath), CompressionLevel.SmallestSize, ct);
    }
}

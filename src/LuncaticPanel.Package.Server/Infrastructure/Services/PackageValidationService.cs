using LunaticPanel.Core.Utils.Abstraction.LinuxCommand;
using LuncaticPanel.Package.Server.Application.Payloads;
using LuncaticPanel.Package.Server.Application.Payloads.Responses;
using LuncaticPanel.Package.Server.Application.Services;
using LuncaticPanel.Package.Server.Infrastructure.Exceptions;
using LuncaticPanel.Package.Server.Infrastructure.Payloads;
using LuncaticPanel.Package.Server.Infrastructure.Payloads.Responses;
using Octokit;
using System.IO.Compression;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace LuncaticPanel.Package.Server.Infrastructure.Services;

internal sealed class PackageValidationService : IPackageValidatorService
{
    private const string URL_PATTERN_VAL = @"^https?://[^?#]+\.lpkg(?:\?.*)?$";
    private readonly HttpClient _http;
    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
#if DEBUG
        WriteIndented = true,
#endif
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        PropertyNameCaseInsensitive = true,
    };
    private readonly ILinuxCommand _linuxCommand;

    public PackageValidationService(IHttpClientFactory httpClientFactory, ILinuxCommand linuxCommand)
    {
        Console.WriteLine($"PackageValidationService Created");

        // Prevent this service from being a linux service.
        if (!OperatingSystem.IsLinux())
            throw new UnsupportedHostingPlatformException();
        _http = httpClientFactory.CreateClient();
        _linuxCommand = linuxCommand;
    }
    public async Task<PackageValidationResponse> ValidateLocalAsync(string target, CancellationToken ct = default)
    {
        Console.WriteLine($"Validate Local Package {target}");
        if (string.IsNullOrWhiteSpace(target))
            throw new PackageTargetEmptyException();
        else if (!File.Exists(target))
            throw new PackageTargetNoFoundException();
        Console.WriteLine($"Reading Local Package Manifest {target}");
        var manifest = ReadManifestFromArchive(target);
        Console.WriteLine($"Package Compiled Using Tool v{manifest.PanelVersion}");
        var tool = await FetchValidatorToolAsync(manifest.PanelVersion);
        Console.WriteLine($"Package Compiled Tool Available at {tool}");
        return await RunValidationAsync(tool, target, target, ct);
    }
    public async Task<PackageValidationResponse> ValidateRemoteAsync(string target, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(target))
            throw new PackageTargetEmptyException();
        else if (Regex.IsMatch(target, URL_PATTERN_VAL, RegexOptions.IgnoreCase))
            throw new PackageTargetPatternViolationException();
        var filePath = await DownloadPackageAsync(target, ct);
        var manifest = ReadManifestFromArchive(filePath);
        var tool = await FetchValidatorToolAsync(manifest.PanelVersion);
        return await RunValidationAsync(tool, filePath, target, ct);
    }
    private async Task<PackageValidationResponse> RunValidationAsync(string tool, string package, string target, CancellationToken ct = default)
    {
        var validationCommandResult = await _linuxCommand
            .BuildCommand($"\"{tool}\" validate --input=\"{package}\"")
            .AutoCleanConsoleStream()
            .ExecPayloadOrDefaultAsync<ToolResultExternalResponse<ManifestPayload>>(default, ct);

        if (validationCommandResult == default)
            throw new PackageValidationFailedUnknownException();

        if (validationCommandResult.Error != default)
            throw new PackageValidationFailedException(validationCommandResult.Error.Code, validationCommandResult.Error.Message);
        if (validationCommandResult.Data == default)
            throw new PackageValidationFailedNoDataException();
        return new PackageValidationResponse()
        {
            Location = Application.Payloads.Enums.PackageValidationLocation.Remote,
            Manifest = validationCommandResult.Data,
            ValidatorSource = tool,
            ValidatorVersion = validationCommandResult.Data.PanelVersion,
            Target = target
        };
    }
    private async Task<string> DownloadPackageAsync(string target, CancellationToken ct = default)
    {
        try
        {
            using var response = await _http.GetAsync(target, HttpCompletionOption.ResponseHeadersRead);
            if (!response.IsSuccessStatusCode)
                throw new PackageDownloadFailureException(response.StatusCode.ToString(), response.ReasonPhrase);
            string tmpRoot = Path.GetTempPath();
            string tmpSub = Path.Combine(tmpRoot, "lunapkg");
            if (!Directory.Exists(tmpSub))
                Directory.CreateDirectory(tmpSub);
            string filename = Path.GetRandomFileName();
            string fullDownloadPath = Path.Combine(tmpSub, $"{filename}.dl");
            string fullTargetPath = Path.Combine(tmpSub, $"{filename}.lpkg");

            await using var input = await response.Content.ReadAsStreamAsync();
            await using var output = File.Create(fullDownloadPath);
            await input.CopyToAsync(output);
            File.Move(fullDownloadPath, fullTargetPath);
            return fullTargetPath;
        }
        catch (ArgumentNullException)
        {
            throw new PackageDownloadOuputNullOrInvalidException();
        }
        catch (ArgumentException)
        {
            throw new PackageDownloadOuputNullOrInvalidException();
        }
        catch (PathTooLongException)
        {
            throw new PackageDownloadPathTooLongException();
        }
        catch (UnauthorizedAccessException)
        {
            throw new PackageDownloadPermissionDeniedException();
        }
        catch (FileNotFoundException)
        {
            throw new PackageDownloadOuputNullOrInvalidException();
        }
        catch (IOException)
        {
            throw new PackageDownloadDiskErrorException();
        }
        catch (Exception)
        {
            throw;
        }
    }
    public ManifestExternalPayload ReadManifestFromArchive(string input)
    {
        try
        {
            using var zip = ZipFile.OpenRead(input);
            // Find the manifest entry
            var entry = zip.GetEntry("manifest.json");
            if (entry == null)
                throw new FileNotFoundException("manifest.json not found in package");
            using var stream = entry.Open();
            return JsonSerializer.Deserialize<ManifestExternalPayload>(stream, _jsonSerializerOptions)!;
        }
        catch (FileNotFoundException)
        {
            throw new PackageManifestNotFoundException();
        }
        catch (Exception)
        {
            throw new PackageManifestFailedReadingException();
        }

    }
    public async Task<string> FetchValidatorToolAsync(string panelVersion, CancellationToken ct = default)
    {
        // Fallback local
        string tmpRoot = Path.GetTempPath();
        string tmpSub = Path.Combine(tmpRoot, "lunapkg_tools");
        if (!Directory.Exists(tmpSub))
            Directory.CreateDirectory(tmpSub);
        string cacheToolPath = Path.Combine(tmpSub, $".lpkg_{panelVersion}.cache");
        bool useLocalTool = false;
        bool hasLocalTool = File.Exists(cacheToolPath);
        if (hasLocalTool)
            useLocalTool = DateTime.UtcNow.Date == DateTime.Parse(File.ReadAllText(cacheToolPath)).Date;
        bool fallback = useLocalTool;
        Console.WriteLine($"Tool Fallback ({fallback}) On ({hasLocalTool}) ? {cacheToolPath}");
        if (!fallback)
        {
            try
            {
                var releaseAsset = await GetLatestMajorToolUrlAsync(panelVersion, ct);
                if (releaseAsset.Value == default || releaseAsset.Key == default)
                    throw new PackageToolNotFoundException(panelVersion);

                await DownloadAndInstallTool(releaseAsset.Value, releaseAsset.Key, ct);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
                Console.WriteLine($"Local Fallback? ({hasLocalTool})-> {cacheToolPath}");
                if (!hasLocalTool)
                    throw;
            }
        }

        Console.WriteLine($"Tool Fetch Completed");
        string tmpTargetDir = Path.Combine(tmpSub, panelVersion);
        string? selectedTool = Directory.GetDirectories(tmpTargetDir, "", SearchOption.TopDirectoryOnly)
            .Where(p => p.StartsWith($"{panelVersion}."))
            .OrderByDescending(x => x)
            .FirstOrDefault();
        if (selectedTool == default)
            throw new FailedToLocateRequiredValidatorException(panelVersion);

        string pathToExe = Path.Combine(selectedTool, "LunaticPanel.Package.Tool");
        if (!File.Exists(pathToExe))
            throw new FailedToLocateRequiredValidatorException(panelVersion);
        return pathToExe;
    }
    private readonly string _versionRegexPattern = @"^[vV](?<version>\d+\.\d+\.\d+)$";
    private async Task<KeyValuePair<string, string>> GetLatestMajorToolUrlAsync(string panelMajor, CancellationToken ct = default)
    {
        Console.WriteLine($"Get Latest Release for Tool v{panelMajor}");
        IReadOnlyList<Release>? releases = default;
        var client = new GitHubClient(new ProductHeaderValue("LunaticPanel"));
        var clientResult = client.Repository.Release.GetAll("mshimshon", "LunaticPanel");

        releases = await clientResult;
        Console.WriteLine($"Github Release Status {clientResult.Status}");
        Console.WriteLine($"Latest Release Result ? {releases.Count}");
        if (releases == default || releases.Count <= 0)
            throw new PackageToolNotFoundException(panelMajor);
        var parsed = releases
            .Where(p => !string.IsNullOrWhiteSpace(p.TagName))
            .Select(p =>
            {
                var match = Regex.Match(p.TagName, _versionRegexPattern, RegexOptions.IgnoreCase);
                if (!match.Success) return null;
                return new { Release = p, Version = new Version(match.Groups["version"].Value) };
            })
            .Where(p => p != null)
            .ToList();
        if (parsed.Count <= 0)
            throw new PackageToolNotFoundException(panelMajor);
        var matchingMajor = parsed
            .Where(x => x!.Version.Major.ToString() == panelMajor)
            .OrderByDescending(x => x!.Version)
            .FirstOrDefault();

        if (matchingMajor == null)
            throw new PackageToolNotFoundException(panelMajor);

        // Find lpkg_tool asset
        var asset = matchingMajor.Release.Assets.FirstOrDefault(a => a.Name.StartsWith("lpkg_tool", StringComparison.OrdinalIgnoreCase));

        if (asset == null)
            throw new PackageToolNotFoundException(panelMajor);

        return new($"{matchingMajor.Version.Major}.{matchingMajor.Version.Minor}.{matchingMajor.Version.Build}", asset.BrowserDownloadUrl);
    }
    private async Task DownloadAndInstallTool(string target, string toolVersion, CancellationToken ct = default)
    {
        string tmpRoot = Path.GetTempPath();
        string tmpSub = Path.Combine(tmpRoot, "lunapkg_tools");
        if (!Directory.Exists(tmpSub))
            Directory.CreateDirectory(tmpSub);
        string major = toolVersion.Split('.')[0];
        string cacheToolPath = Path.Combine(tmpSub, $".lpkg_{major}.cache");
        string cacheToolPathWrite = Path.Combine(tmpSub, $".lpkg_{major}.write");

        string tmpDownloads = Path.Combine(tmpSub, "downloads");

        using var response = await _http.GetAsync(target, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();
        string filename = Path.GetRandomFileName();
        string fullDownloadPath = Path.Combine(tmpDownloads, $"{filename}.dl");
        string fullTargetPath = Path.Combine(tmpDownloads, $"{filename}.zip");
        await using var input = await response.Content.ReadAsStreamAsync();
        await using var output = File.Create(fullDownloadPath);
        await input.CopyToAsync(output);
        File.Move(fullDownloadPath, fullTargetPath);
        string targetRoot = Path.Combine(tmpSub, toolVersion);
        await ExtractZipToRoot(fullTargetPath, targetRoot, ct);
        File.WriteAllText(cacheToolPathWrite, DateTime.UtcNow.ToString());
        File.Move(cacheToolPath, cacheToolPathWrite);
    }
    private async Task ExtractZipToRoot(string zipPath, string targetRoot, CancellationToken ct = default)
    {
        if (Directory.Exists(targetRoot))
            Directory.Delete(targetRoot, true);
        Directory.CreateDirectory(targetRoot);

        using var zip = ZipFile.OpenRead(zipPath);

        foreach (var entry in zip.Entries)
        {
            // Skip directory entries
            if (string.IsNullOrEmpty(entry.Name))
                continue;
            string outputPath = Path.Combine(targetRoot, entry.FullName);
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
            await entry.ExtractToFileAsync(outputPath);
        }
    }
}

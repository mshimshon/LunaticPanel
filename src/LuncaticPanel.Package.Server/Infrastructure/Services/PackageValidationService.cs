using LunaticPanel.Core.Utils.Abstraction.LinuxCommand;
using LuncaticPanel.Package.Server.Application.Payloads;
using LuncaticPanel.Package.Server.Application.Payloads.Responses;
using LuncaticPanel.Package.Server.Application.Services;
using LuncaticPanel.Package.Server.Infrastructure.Exceptions;
using LuncaticPanel.Package.Server.Infrastructure.Payloads;
using LuncaticPanel.Package.Server.Infrastructure.Payloads.Responses;
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
        _http = httpClientFactory.CreateClient();
        _linuxCommand = linuxCommand;
    }
    public async Task<PackageValidationResponse> ValidateLocalAsync(string target, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(target))
            throw new PackageTargetEmptyException();
        else if (!File.Exists(target))
            throw new PackageTargetNoFoundException();
        var manifest = ReadManifestFromArchive(target);
        var tool = await FetchValidatorToolAsync(manifest.PanelVersion);
        return await RunValidationAsync(tool, target, target, ct);
    }

    public async Task<PackageValidationResponse> ValidateRemoteAsync(string target, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(target))
            throw new PackageTargetEmptyException();
        else if (Regex.IsMatch(target, URL_PATTERN_VAL, RegexOptions.IgnoreCase))
            throw new PackageTargetPatternViolationException();
        var filePath = await DownloadAsync(target, ct);
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
    private async Task<string> DownloadAsync(string target, CancellationToken ct = default)
    {
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
            try
            {
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
        // TODO: FETCH FROM GITHUB RELEASE
        // Fallback local
        string tmpRoot = Path.GetTempPath();
        string tmpSub = Path.Combine(tmpRoot, "lunapkg_tools");
        if (!Directory.Exists(tmpSub))
            Directory.CreateDirectory(tmpSub);
        string tmpTargetDir = Path.Combine(tmpSub, panelVersion);
        string? toolFallback = Directory.GetDirectories(tmpTargetDir, "", SearchOption.TopDirectoryOnly).FirstOrDefault(p => p.StartsWith($"{panelVersion}."));
        if (toolFallback == default)
            throw new FailedToLocateRequiredValidatorException(panelVersion);
        string pathToExe = Path.Combine(toolFallback, "LunaticPanel.Package.Tool");
        if (!File.Exists(pathToExe))
            throw new FailedToLocateRequiredValidatorException(panelVersion);
        return pathToExe;
    }
}

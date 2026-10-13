using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.Core.Utils.Abstraction.Plugin.Location;
using LunaticPanel.Core.Utils.Abstraction.SafeFileWriter;
using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Services;
using LunaticPanel.PackageManager.Keys;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LunaticPanel.PackageManager.Infrastructure.Services;

internal class SourceFileService : ISourceFileService
{
    private readonly string _sourceFile;
    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        ReferenceHandler = ReferenceHandler.IgnoreCycles
    };
    private readonly ISafeFileWriter _safeFileWriter;
    private readonly ICrazyReport<SourceFileService> _crazyReport;

    public SourceFileService(IPluginLocation pluginLocation, ISafeFileWriter safeFileWriter, ICrazyReport<SourceFileService> crazyReport)
    {
        _sourceFile = pluginLocation.GetConfigFor(LPPackageManagerKeys.MODULE_NAME, "sources.json");
        _safeFileWriter = safeFileWriter;
        _crazyReport = crazyReport;
        _crazyReport.SetModule(LPPackageManagerKeys.MODULE_NAME);
    }

    public async Task<ICollection<RepositorySourcePayload>> GetSourcesAsync(CancellationToken ct = default)
    {
        if (File.Exists(_sourceFile))
        {
            _crazyReport.ReportInfo("Reading Package Source from File '{0}'", _sourceFile);
            string jsonStr = File.ReadAllText(_sourceFile);
            List<RepositorySourcePayload>? sources = JsonSerializer.Deserialize<List<RepositorySourcePayload>>(jsonStr, _serializerOptions);
            return sources ?? Array.Empty<RepositorySourcePayload>().ToList();
        }
        var defaultValue = new List<RepositorySourcePayload>() {
            new RepositorySourcePayload(){
                Name = "Local Directory",
                Source = "/var/lib/lunaticpanel/plugins/lunaticpanel_packagemanager/local_lpkgs",
                SourceType =  Application.Payloads.Enums.RepositorySourceTypePayload.Local,
                State = Application.Payloads.Enums.RepositorySourceStatePayload.Enabled
            },
            new RepositorySourcePayload(){
                Name = "Official Repository",
                Source = "https://lpkg.lunaticpanel.com/",
                SourceType =  Application.Payloads.Enums.RepositorySourceTypePayload.Remote,
                State = Application.Payloads.Enums.RepositorySourceStatePayload.Enabled
            },
        };
        _crazyReport.ReportWarning("'{0}' Does not exist, fallback to default values and create the source file.", _sourceFile);
        try
        {

            await _safeFileWriter.WriteThenCopyFileAsync(_sourceFile, JsonSerializer.Serialize(defaultValue, _serializerOptions), ct);
            _crazyReport.ReportSuccess("'{0}' was created.", _sourceFile);

        }
        catch (Exception ex)
        {
            _crazyReport.ReportErrorException(ex.Message, ex);
        }
        return defaultValue;
    }

    public async Task<ICollection<RepositorySourcePayload>> SaveSourcesAsync(List<RepositorySourcePayload> sourcePayloads, CancellationToken ct = default)
    {
        string sources = JsonSerializer.Serialize(sourcePayloads, _serializerOptions);
        await File.WriteAllTextAsync(_sourceFile, sources);
        return await GetSourcesAsync(ct);
    }
}

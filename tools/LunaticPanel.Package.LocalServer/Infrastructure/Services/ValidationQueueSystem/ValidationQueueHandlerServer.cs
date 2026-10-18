using LunaticPanel.Package.LocalServer.Infrastructure.Exceptions;
using LunaticPanel.Package.LocalServer.Infrastructure.Services.ValidationQueueSystem.Structs;
using LunaticPanel.Package.Server.Application.Mediator.Commands;
using LunaticPanel.Package.Server.Application.Payloads;
using LunaticPanel.Package.Server.Application.Payloads.Enums;
using LunaticPanel.Package.Server.Application.Payloads.Requests;
using LunaticPanel.Package.Server.Application.Payloads.Responses;
using MaksimShimshon.Mediator;

namespace LunaticPanel.Package.LocalServer.Infrastructure.Services.ValidationQueueSystem;

public class ValidationQueueHandlerServer : IValidationQueueHandlerService
{
    private Dictionary<string, DateTime> _timeoutControl = new();
    private Dictionary<string, ManifestPayload> _idToManifestCache = new();
    private Dictionary<PackageKey, string> _manifestToUploadIdCache = new();
    private Dictionary<string, string> _uploadIdToFile = new();
    private Dictionary<string, PackageUploadAccessResponse> _uploadRouteToUploadIdCache = new();
    private Queue<string> _validationQueue = new();
    private CancellationTokenSource? _cts;
    private bool _isRunning = false;
    private readonly object _lock = new Object();
    private readonly IServiceProvider _serviceProvider;

    public ValidationQueueHandlerServer(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    public Task CacheUploadRouteAsync(PackageUploadAccessResponse uploadRoute, CancellationToken ct = default)
    {
        lock (_lock)
        {
            _uploadRouteToUploadIdCache.Add(uploadRoute.UploadId, uploadRoute);
            return Task.CompletedTask;
        }
    }

    public Task<string> GenerateUploadIdAsync(ManifestPayload manifest, CancellationToken ct = default)
    {
        lock (_lock)
        {
            var packageKey = new PackageKey(manifest.Id, manifest.Version);
            if (_manifestToUploadIdCache.ContainsKey(packageKey))
            {
                DateTime expiry = _timeoutControl[_manifestToUploadIdCache[packageKey]];
                TimeSpan remaining = expiry - DateTime.UtcNow;
                throw new InfrastructureException($"AlreadyGrantedPackageUploadAccess", $"This package already has granted access. Wait {remaining.TotalMinutes:F0} minutes for expiry or use the granted upload key.");
            }
            string uploadId = Guid.NewGuid().ToString();
            _idToManifestCache[uploadId] = manifest;
            _manifestToUploadIdCache[packageKey] = uploadId;
            _timeoutControl[uploadId] = DateTime.UtcNow.AddMinutes(30);
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            _ = TimeoutLoop(_cts.Token);
            return Task.FromResult(uploadId);
        }
    }
    public Task<ManifestPayload> GetManifestFromIdAsync(string uploadId, CancellationToken ct = default)
    {
        lock (_lock)
        {
            if (!_idToManifestCache.TryGetValue(uploadId, out ManifestPayload? result))
                throw new InfrastructureException("FailedToRetrieveManifestFromUploadKey", "Could not retrieve manifest from upload key.");
            if (result == default)
                throw new InfrastructureException("FailedToRetrieveManifestFromUploadKey", "Could not retrieve manifest from upload key.");

            return Task.FromResult(result);
        }

    }

    public Task<bool> HasExistingPendingUploadAsync(ManifestPayload manifestPayload, CancellationToken ct = default)
    {
        lock (_lock)
        {
            var result = _manifestToUploadIdCache.ContainsKey(new PackageKey(manifestPayload.Id, manifestPayload.Version));
            return Task.FromResult(result);
        }
    }

    public Task QueueUpAsync(string uploadId, CancellationToken ct = default)
    {
        _validationQueue.Enqueue(uploadId);

        if (!_isRunning)
            _ = ProcessQueueAsync();
        return Task.CompletedTask;
    }

    public async Task ProcessQueueAsync()
    {
        lock (_lock)
        {
            if (_isRunning)
                return;
            _isRunning = true;
        }
        do
        {
            await Task.Delay(500);
            if (!_validationQueue.TryDequeue(out string? uploadId))
                continue;
            if (uploadId == default)
                continue;
            if (!_uploadIdToFile.TryGetValue(uploadId, out string? file))
                continue;
            if (string.IsNullOrWhiteSpace(file))
                continue;
            var scope = _serviceProvider.CreateScope();
            var sp = scope.ServiceProvider;
            try
            {

                IMediator mediator = sp.GetRequiredService<IMediator>();
                var validationRequest = new PackageValidationRequest()
                {
                    Target = file,
                    LocationType = PackageValidationLocation.Local
                };
                var validationResult = await mediator.ExecuteAsync(new PackageValidationCommand(validationRequest));
                string pathBase = "/var/lib/lunaticpanel_lpkg_localserver/lpkgs";

                string servingDest = Path.Combine(pathBase, "available", $"{validationResult.Manifest.Id}.{validationResult.Manifest.Version}.lpkg");
                File.Move(file, servingDest);

                await mediator.ExecuteAsync(new CreateManifestCommand(validationResult.Manifest));
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                await ClearExpiredUpload(uploadId);
                continue;
            }
            finally
            {
                scope.Dispose();
            }
        } while (_validationQueue.Count > 0);

        lock (_lock)
        {
            if (!_isRunning)
                return;
            _isRunning = false;
        }
    }

    public Task ValidateUploadIdAsync(string uploadId, CancellationToken ct = default)
    {
        lock (_lock)
        {
            if (!_timeoutControl.ContainsKey(uploadId))
                throw new InfrastructureException("UploadIdInvalidException", "The upload id is not valid.");
            return Task.CompletedTask;
        }
    }

    public Task AssociateUploadIdToFileAsync(string uploadId, string path, CancellationToken ct = default)
    {
        lock (_lock)
        {
            _uploadIdToFile[uploadId] = path;
            return Task.CompletedTask;
        }
    }


    private async Task ClearExpiredUpload(string uploadId)
    {
        lock (_lock)
        {
            _idToManifestCache.Remove(uploadId);
            var id = _manifestToUploadIdCache.FirstOrDefault(p => p.Value == uploadId);
            _manifestToUploadIdCache.Remove(id.Key);
            _timeoutControl.Remove(uploadId);
            _uploadRouteToUploadIdCache.Remove(uploadId);
            _uploadIdToFile.Remove(uploadId);

        }
    }
    private async Task TimeoutLoop(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            string nextKey;
            DateTime nextTime;

            lock (_lock)
            {
                if (_timeoutControl.Count == 0)
                    return;

                // Find earliest expiration
                var kv = _timeoutControl.OrderBy(x => x.Value).First();
                nextKey = kv.Key;
                nextTime = kv.Value;
            }

            var delay = nextTime - DateTime.UtcNow;

            if (delay <= TimeSpan.Zero)
            {
                lock (_lock)
                {
                    _timeoutControl.Remove(nextKey);
                }
                _ = ClearExpiredUpload(nextKey);
                continue;
            }

            try
            {
                await Task.Delay(delay, token);
            }
            catch (TaskCanceledException)
            {
                return; // loop reset
            }
        }
    }

    public Task<bool> HasPendingValidationAsync(ManifestPayload manifestPayload, CancellationToken ct = default)
    {
        lock (_lock)
        {
            var packageKey = new PackageKey(manifestPayload.Id, manifestPayload.Version);
            if (!_manifestToUploadIdCache.ContainsKey(packageKey))
                return Task.FromResult(false);

            var result = _validationQueue.Contains(_manifestToUploadIdCache[packageKey]);
            return Task.FromResult(result);
        }
    }
}

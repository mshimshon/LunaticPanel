using LunaticPanel.DebugTool.Extensions;
using LunaticPanel.DebugTool.Payloads;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using static LunaticPanel.DebugTool.Extensions.MSBuildExt;
using static LunaticPanel.DebugTool.Extensions.SubsystemExt;
namespace LunaticPanel.DebugTool.Services;

internal sealed class DeploymentService
{

    private readonly string _operatingSystemImgFile;
    private readonly string _serviceInstalledFile;
    private readonly string _wslDataFolder;
    private readonly string _deployEnvironmentName = "lpcli_deploy";
    private readonly string[] _systemDependencies = [
        "wget"
        ];
    private readonly string _tempLocation;
    private bool _skipStep = false;
    public ConfigurationPayload Configuration { get; }
    public DeploymentService(ConfigurationPayload configuration)
    {
        var tmp = Path.GetTempPath();
        _tempLocation = Path.Combine(tmp, "lpcli", "temp");
        var snapLocation = Path.Combine(_tempLocation, "snapshots");
        var cliWSLDataTmp = Path.Combine(_tempLocation, "data");
        if (!Directory.Exists(cliWSLDataTmp))
            Directory.CreateDirectory(cliWSLDataTmp);
        _operatingSystemImgFile = Path.Combine(_tempLocation, "os_debian.tar.gz");
        _serviceInstalledFile = Path.Combine(_tempLocation, "os_debian_service.tar.gz");
        _wslDataFolder = cliWSLDataTmp;
        Configuration = configuration;
        if (!string.IsNullOrWhiteSpace(Configuration.Snap))
        {
            var snapshotLocation = Path.Combine(snapLocation, $"{Configuration.Snap}.tar.gz");
            _skipStep = File.Exists(snapshotLocation);
            Console.WriteLine($"Snapshot Mode ({_skipStep}): {Configuration.Snap} ({snapshotLocation})");
        }

    }

    public async Task CleanUp(bool soft, CancellationToken ct = default)
    {
        var tmp = Path.GetTempPath();
        Console.WriteLine($"Temp: {_tempLocation}");
        if (!soft)
        {
            bool deployExist = await WslDistroExists(_deployEnvironmentName);
            if (deployExist)
                await DestroyAsync(_deployEnvironmentName);
            if (Directory.Exists(_tempLocation))
                Directory.Delete(_tempLocation, true);
        }
        else
        {
            if (Directory.Exists(Path.Combine(_tempLocation, "archives")))
                Directory.Delete(Path.Combine(_tempLocation, "archives"), true);
            if (Directory.Exists(Path.Combine(_tempLocation, "build")))
                Directory.Delete(Path.Combine(_tempLocation, "build"), true);
            if (Directory.Exists(Path.Combine(_tempLocation, "lpkgs")))
                Directory.Delete(Path.Combine(_tempLocation, "lpkgs"), true);
            if (Directory.Exists(Path.Combine(_tempLocation, "lpkgs_unpack")))
                Directory.Delete(Path.Combine(_tempLocation, "lpkgs_unpack"), true);
            if (Directory.Exists(Path.Combine(_tempLocation, "publish")))
                Directory.Delete(Path.Combine(_tempLocation, "publish"), true);
            if (Directory.Exists(Path.Combine(_tempLocation, "systemd")))
                Directory.Delete(Path.Combine(_tempLocation, "systemd"), true);
            if (File.Exists(Path.Combine(_tempLocation, "bootstrap.json")))
                File.Delete(Path.Combine(_tempLocation, "bootstrap.json"));
        }



    }
    public async Task DeployAsync(CancellationToken ct = default)
    {
        bool isFresh = false;
        Console.Out.WriteLine("Checking Distro Availability");
        bool debianExist = await WslDistroExists("Debian");

        if (!_skipStep)
        {
            bool distroInstallRequired = !Configuration.SkipSubSystemRebuild || !File.Exists(_operatingSystemImgFile);
            if (debianExist && distroInstallRequired)
            {
                Console.Out.WriteLine("We require to destroy 'Debian' distro currently on your WSL.");
                Console.Out.WriteLine("Export it under another name or continue pressing Y/n");
                bool allowedDestroy = Configuration.NoInteraction ? true : false;
                if (!Configuration.NoInteraction)
                {
                    var key = Console.ReadKey(intercept: true);
                    allowedDestroy = key.Key == ConsoleKey.Y;
                }
                else
                    Console.Out.WriteLine("No Interaction mode activate Y is automatic.");

                if (allowedDestroy)
                    await DestroyAsync("Debian");
            }

            if (distroInstallRequired)
                isFresh = await InstallDistro();
            else
            {
                Console.Out.WriteLine($"Use Existing '{_operatingSystemImgFile}'.");
                bool deployExist = await WslDistroExists(_deployEnvironmentName);
                if (deployExist)
                {
                    await DestroyAsync(_deployEnvironmentName);
                }
            }
            bool serviceInstallRequired = !Configuration.SkipServiceRebuild || !File.Exists(_serviceInstalledFile) || isFresh;


            if (serviceInstallRequired)
                await InstallServicesAsync(ct);
            else
            {
                Console.Out.WriteLine($"Use Existing '{_serviceInstalledFile}'.");
                bool deployExist = await WslDistroExists(_deployEnvironmentName);
                if (deployExist)
                    await DestroyAsync(_deployEnvironmentName);

                await ImportAsync(_deployEnvironmentName, _wslDataFolder, _serviceInstalledFile);
            }
        }
        else
        {
            bool deployExist = await WslDistroExists(_deployEnvironmentName);
            if (deployExist)
                await DestroyAsync(_deployEnvironmentName);
            await ImportAsync(_deployEnvironmentName, _wslDataFolder, Path.Combine(_tempLocation, "snapshots", $"{Configuration.Snap}.tar.gz"));
        }





        await InstallPlugins(ct);
        await RunPostProcessing(ct);
        await ShudownAsync(_deployEnvironmentName);
        await StartAsync(_deployEnvironmentName);
        //foreach (var item in Configuration.Compose.Services)
        //    await RunAsync(_deployEnvironmentName, $"systemctl status {item.ServiceName} || true");

        await RunAsync(_deployEnvironmentName, $"cat /var/lib/lunaticpanel/config/bootstrap.json");
        await FinalizeDeployment();
    }
    private async Task RunPostProcessing(CancellationToken ct = default)
    {
        var tmp = Path.GetTempPath();
        var cliArchivesTmp = Path.Combine(_tempLocation, "archives");
        if (Directory.Exists(cliArchivesTmp))
            Directory.Delete(cliArchivesTmp, true);
        Directory.CreateDirectory(cliArchivesTmp);
        Configuration.PrintDebug($"[PostProcessing]::Starting (Searching Snapshot? {_skipStep}).");

        foreach (var pp in Configuration.Compose.PostProcessing)
        {

            string? finalizeFrom = string.Empty;
            string? targetTo = string.Empty;
            if (_skipStep && !string.IsNullOrWhiteSpace(pp.Snap) && pp.Snap == Configuration.Snap)
            {
                Configuration.PrintDebug($"[PostProcessing]::Snapshot Step Match, Stop Skipping.");
                _skipStep = false;
                continue;
            }
            else if (!_skipStep && !string.IsNullOrWhiteSpace(pp.Snap))
            {
                Configuration.PrintDebug($"[PostProcessing]::Snapshot Step Creating Snapshot.");
                await CreateSnapshot(pp.Snap, ct);
                continue;
            }
            else if (!string.IsNullOrWhiteSpace(pp.Command))
            {
                await RunAsync(_deployEnvironmentName, pp.Command!);
                continue;
            }
            else if (!string.IsNullOrWhiteSpace(pp.DotnetProject) && !string.IsNullOrWhiteSpace(pp.PublishTo))
            {
                Configuration.PrintDebug($"[PostProcessing]::Detected Dotnet Project to publish.");
                await PublishProjectAsync(pp.DotnetProject!);
                string filename = Path.GetFileNameWithoutExtension(pp.DotnetProject);
                var cliPublishTmp = Path.Combine(_tempLocation, "publish");
                finalizeFrom = Path.Combine(cliPublishTmp, filename);
                targetTo = pp.PublishTo;
                Configuration.PrintDebug($"[PostProcessing]::'{finalizeFrom}' -> '{targetTo}'.");
            }
            else if (!string.IsNullOrWhiteSpace(pp.DotnetProject) && !string.IsNullOrWhiteSpace(pp.BuildTo))
            {
                Configuration.PrintDebug($"[PostProcessing]::Detected Dotnet Project to build.");
                await BuildProjectAsync(pp.DotnetProject!);
                string filename = Path.GetFileNameWithoutExtension(pp.DotnetProject);
                var cliPublishTmp = Path.Combine(_tempLocation, "build");
                finalizeFrom = Path.Combine(cliPublishTmp, filename);
                targetTo = pp.BuildTo;
                Configuration.PrintDebug($"[PostProcessing]::'{finalizeFrom}' -> '{targetTo}'.");
            }
            else if (!string.IsNullOrWhiteSpace(pp.DotnetProject) && !string.IsNullOrWhiteSpace(pp.PluginPackTo))
            {
                Configuration.PrintDebug($"[PostProcessing]::Detected Dotnet Project to Pack as LPKG.");
                await BuildProjectAsync(pp.DotnetProject!);
                string filename = Path.GetFileNameWithoutExtension(pp.DotnetProject);
                var cliPublishTmp = Path.Combine(_tempLocation, "build");
                var buildLocation = Path.Combine(cliPublishTmp, filename);
                var cliLpkgTmp = Path.Combine(_tempLocation, "lpkgs");
                var item = await PackProjectAsync(pp.DotnetProject, ct);
                var lpkgFileLocation = Path.Combine(cliLpkgTmp, $"{item.Id}.{item.Version}.lpkg");
                finalizeFrom = lpkgFileLocation;
                if (pp.PluginPackTo.EndsWith(".lpkg"))
                    targetTo = pp.PluginPackTo;
                if (pp.PluginPackTo.EndsWith('/'))
                    targetTo = pp.PluginPackTo + $"{item.Id}.{item.Version}.lpkg";
                else
                    targetTo = pp.PluginPackTo + $"/{item.Id}.{item.Version}.lpkg";

                await CopyFileAsync(_deployEnvironmentName, finalizeFrom, targetTo);

                Configuration.PrintDebug($"[PostProcessing]::'{finalizeFrom}' -> '{targetTo}'.");
                return;
            }
            else if (!string.IsNullOrWhiteSpace(pp.File) && !string.IsNullOrWhiteSpace(pp.FileTo))
            {
                Configuration.PrintDebug($"[PostProcessing]::Detected File to copy.");
                if (!File.Exists(pp.File))
                    throw new Exception($"'{pp.File}' not found.");
                finalizeFrom = pp.File;
                targetTo = pp.FileTo;
                Configuration.PrintDebug($"[PostProcessing]::'{finalizeFrom}' -> '{targetTo}'.");
            }
            else if (!string.IsNullOrWhiteSpace(pp.Folder) && !string.IsNullOrWhiteSpace(pp.FolderTo))
            {
                Configuration.PrintDebug($"[PostProcessing]::Detected folder to copy.");
                if (!Directory.Exists(pp.Folder))
                    throw new Exception($"'{pp.Folder}' not found.");
                finalizeFrom = pp.Folder;
                targetTo = pp.FolderTo;
                Configuration.PrintDebug($"[PostProcessing]::'{finalizeFrom}' -> '{targetTo}'.");
            }
            else
            {
                throw new Exception($"1: Invalid pp command {pp}");
            }
            if (string.IsNullOrWhiteSpace(targetTo) || string.IsNullOrWhiteSpace(finalizeFrom))
                throw new Exception($"2: Invalid pp command {pp}");

            if (!string.IsNullOrWhiteSpace(pp.Archive))
            {
                var tmpArchivePath = Path.GetTempFileName();
                if (File.Exists(finalizeFrom))
                    if (pp.Archive == "zip")
                        await ArchiveExt.CreateZipAsync(finalizeFrom, tmpArchivePath, ct);
                    else if (pp.Archive == "zip")
                        await ArchiveExt.CreateFileTarGzAsync(finalizeFrom, tmpArchivePath, ct);
                    else throw new Exception($"{pp.Archive} not supported format.");
                else if (Directory.Exists(finalizeFrom))
                    if (pp.Archive == "zip")
                        await ArchiveExt.CreateZipFolderAsync(finalizeFrom, tmpArchivePath, ct);
                    else if (pp.Archive == "tar.gz")
                        await ArchiveExt.CreateFolderTarGzAsync(finalizeFrom, tmpArchivePath, ct);
                    else throw new Exception($"{pp.Archive} not supported format.");
                else
                    throw new Exception($"{finalizeFrom} does not exist.");
                await CopyFileAsync(_deployEnvironmentName, tmpArchivePath, targetTo);
                File.Delete(tmpArchivePath);
            }
            else if (File.Exists(finalizeFrom))
                await CopyFileAsync(_deployEnvironmentName, finalizeFrom, targetTo);
            else if (Directory.Exists(finalizeFrom))
                await CopyDirAsync(_deployEnvironmentName, finalizeFrom, targetTo);
            else
                throw new Exception($"3: Invalid pp command {pp}");

        }

    }
    private async Task InstallPlugins(CancellationToken ct = default)
    {
        List<JsonObject> bootstrapDefinition = new List<JsonObject>();
        bool containsSnapTarget = _skipStep && Configuration.Compose.Plugins.Any(p => !string.IsNullOrWhiteSpace(p.Snap) && p.Snap == Configuration.Snap);
        if (_skipStep && !containsSnapTarget) return;
        foreach (var plugin in Configuration.Compose.Plugins)
        {
            if (_skipStep && !string.IsNullOrWhiteSpace(plugin.Snap) && plugin.Snap == Configuration.Snap)
            {
                Configuration.PrintDebug($"[Plugin]::Snapshot Step Match Stop Skipping.");
                string bootstrapProgressFile = Path.Combine(_tempLocation, "snapshots", $"{plugin.Snap}_bootstrap.json");
                if (!File.Exists(bootstrapProgressFile))
                    throw new Exception($"'{bootstrapProgressFile}' does not exist run --hard-reset to fully clear cache.");
                bootstrapDefinition = JsonSerializer.Deserialize<List<JsonObject>>(File.ReadAllText(bootstrapProgressFile))!;

                _skipStep = false;
                continue;
            }
            else if (_skipStep)
            {
                Configuration.PrintDebug($"[Plugin]::{plugin.Id} Already into snapshot, skipping.");
                continue;
            }
            else if (!_skipStep && !string.IsNullOrWhiteSpace(plugin.Snap))
            {
                Configuration.PrintDebug($"[Plugin]::Snapshot Step Creating...");
                string bootstrapProgressFile = Path.Combine(_tempLocation, "snapshots", $"{plugin.Snap}_bootstrap.json");
                if (File.Exists(bootstrapProgressFile))
                    File.Delete(bootstrapProgressFile);
                File.WriteAllText(bootstrapProgressFile, JsonSerializer.Serialize(bootstrapDefinition));
                await CreateSnapshot(plugin.Snap, ct);

            }
            else if (!string.IsNullOrWhiteSpace(plugin.DotnetProject))
            {
                await BuildProjectAsync(plugin.DotnetProject);
                var manifestTmp = await PackProjectAsync(plugin, ct);
                await UnpackPackageToLinuxLocation(manifestTmp, ct);
                await InstallPluginInSubSystem(plugin, manifestTmp, ct);


                var def = BuildBootstrapManifestFrom(plugin, manifestTmp);
                bootstrapDefinition.Add(def);
            }

        }
        await BuildAndPublishBootstrap(bootstrapDefinition);
    }
    private JsonObject BuildBootstrapManifestFrom(PluginComposePayload csproj, PackToolPluginManifestExternalPayload manifestExternalPayload)
    {
        string linuxFolder = manifestExternalPayload.Id.ToLower().Replace('.', '_');
        var identityNode = new JsonObject
        {
            ["PackageId"] = manifestExternalPayload.Id,
            ["PakageVersion"] = manifestExternalPayload.Version,
            ["DisplayName"] = manifestExternalPayload.Title
        };

        var lifecycleNode = new JsonObject
        {
            ["State"] = 4,
            ["StartupState"] = csproj.Enabled ? 1 : 0
        };

        // 2. Build the Entity parent container
        var entityNode = new JsonObject
        {
            ["Identity"] = identityNode,
            ["Lifecycle"] = lifecycleNode
        };

        // 3. Create the plugin list item entry
        var pluginEntry = new JsonObject
        {
            ["Entity"] = entityNode,
            ["PluginDir"] = $"/srv/lunaticpanel/plugins/{linuxFolder}"
        };
        return pluginEntry;
    }
    private async Task InstallPluginInSubSystem(PluginComposePayload csproj, PackToolPluginManifestExternalPayload manifestExternalPayload, CancellationToken ct = default)
    {
        string linuxFolder = manifestExternalPayload.Id.ToLower().Replace('.', '_');
        string filename = Path.GetFileNameWithoutExtension(csproj.DotnetProject!);
        var tmp = Path.GetTempPath();
        var cliLpkgUnpackTmp = Path.Combine(_tempLocation, "lpkgs_unpack");
        var cliLpkgDir = Path.Combine(cliLpkgUnpackTmp, linuxFolder);
        var cliLpkgTmp = Path.Combine(_tempLocation, "lpkgs");

        var clilpkgLocal = Path.Combine(cliLpkgTmp, $"{manifestExternalPayload.Id}.{manifestExternalPayload.Version}.lpkg");
        // /usr/lib/lunaticpanel/plugins
        await CopyDirAsync(_deployEnvironmentName, cliLpkgDir, $"/srv/lunaticpanel/plugins/{linuxFolder}");
        await RunAsync(_deployEnvironmentName, $"ls /srv/lunaticpanel/plugins/{linuxFolder}");
        ///var/lib/lunatic_panel_package/lpkgs
        await CopyFileAsync(_deployEnvironmentName, clilpkgLocal, $"/var/lib/lunaticpanel_lpkg_localserver/lpkgs/awaiting/{manifestExternalPayload.Id}.{manifestExternalPayload.Version}.lpkg");


    }
    private async Task<PackToolPluginManifestExternalPayload> PackProjectAsync(PluginComposePayload payload, CancellationToken ct = default)
        => await PackProjectAsync(payload.DotnetProject!, ct);
    private async Task<PackToolPluginManifestExternalPayload> PackProjectAsync(string dotnetProject, CancellationToken ct = default)
    {
        string filename = Path.GetFileNameWithoutExtension(dotnetProject!);
        var tmp = Path.GetTempPath();
        var cliPublishTmp = Path.Combine(_tempLocation, "build");
        var cliLpkgTmp = Path.Combine(_tempLocation, "lpkgs");
        var cliLpkgUnpackTmp = Path.Combine(_tempLocation, "lpkgs_unpack");
        if (!Directory.Exists(cliLpkgUnpackTmp))
            Directory.CreateDirectory(cliLpkgUnpackTmp);
        var cliLpkgDir = Path.Combine(cliPublishTmp, filename);
        string currentFolder = AppDomain.CurrentDomain.BaseDirectory;
        var cliPackingTool = Path.Combine(currentFolder, "LunaticPanel.Package.Tool.exe");
        var cmd = $"pack --input \"{cliLpkgDir}\" --output \"{cliLpkgTmp}\"";
        var result = await ProcessExt.RunProcessAsync(cliPackingTool, cmd);
        Console.Out.WriteLine("Finished Packing");
        string[] lines = result.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        bool payloadFound = false;
        StringBuilder jsonData = new StringBuilder();
        string? finalData = default;
        Console.Out.WriteLine("Extracting Payload");
        int startIndex = Array.LastIndexOf(lines, "<<<PAYLOAD_BEGIN>>>"), stopIndex = Array.LastIndexOf(lines, "<<<PAYLOAD_END>>>");
        if (startIndex < 0 || stopIndex < 0)
            throw new Exception($"Failed to get manifest for {dotnetProject}");
        for (int i = startIndex + 1; i < stopIndex; i++)
        {
            jsonData.AppendLine(lines[i]);
        }
        var tmpJsonResult = jsonData.ToString();
        finalData = string.IsNullOrWhiteSpace(tmpJsonResult) ? default : tmpJsonResult;

        Console.Out.WriteLine($"Payload Found ? {payloadFound} ({finalData != default})");
        if (finalData == default)
            throw new Exception($"Failed to get manifest for {dotnetProject}");
        PackToolResultExternalPayload? resultReponse = JsonSerializer.Deserialize<PackToolResultExternalPayload>(finalData);
        if (resultReponse == default || resultReponse.Data == default)
            throw new Exception($"Failed to get manifest for {dotnetProject}");
        return resultReponse.Data;
    }
    private async Task UnpackPackageToLinuxLocation(PackToolPluginManifestExternalPayload item, CancellationToken ct = default)
    {
        try
        {
            var cliLpkgTmp = Path.Combine(_tempLocation, "lpkgs");
            var cliLpkgUnpackTmp = Path.Combine(_tempLocation, "lpkgs_unpack");
            string currentFolder = AppDomain.CurrentDomain.BaseDirectory;
            var cliPackingTool = Path.Combine(currentFolder, "LunaticPanel.Package.Tool.exe");

            Console.Out.WriteLine($"Plugin {item.Id} is packed and ready to deploy.");
            string linuxName = item.Id.ToLower().Replace('.', '_');
            string unpackTo = Path.Combine(cliLpkgUnpackTmp, linuxName);
            if (Directory.Exists(unpackTo))
                Directory.Delete(unpackTo, true);
            Directory.CreateDirectory(unpackTo);
            var cmdUnpack = $"unpack --root --input \"{Path.Combine(cliLpkgTmp, $"{item.Id}.{item.Version}.lpkg")}\" --output \"{unpackTo}\"";
            await ProcessExt.RunProcessAsync(cliPackingTool, cmdUnpack);
        }
        catch
        {
            Console.Error.WriteLine(item);
            throw;
        }
    }
    private async Task BuildAndPublishBootstrap(List<JsonObject> defs, CancellationToken ct = default)
    {
        var tmp = Path.GetTempPath();
        var cliPublishTmp = Path.Combine(_tempLocation, "bootstrap.json");
        if (File.Exists(cliPublishTmp))
            File.Delete(cliPublishTmp);

        ///etc/lunaticpanel/bootstrap.json
        var root = new JsonObject
        {
            ["KnownPlugins"] = new JsonArray(defs.ToArray<JsonNode>())
        };
        string json = JsonSerializer.Serialize(root);
        File.WriteAllText(cliPublishTmp, json);
        await CopyFileAsync(_deployEnvironmentName, cliPublishTmp, "/var/lib/lunaticpanel/config/bootstrap.json");


    }
    private async Task InstallServicesAsync(CancellationToken ct = default)
    {
        Console.Out.WriteLine("Requires Fresh Service Deployment.");
        bool deployExist = await WslDistroExists(_deployEnvironmentName);
        if (deployExist)
        {
            await DestroyAsync(_deployEnvironmentName);
        }
        await ImportAsync(_deployEnvironmentName, _wslDataFolder, _operatingSystemImgFile);
        await PrintDistros();
        await DeployServicesAsync(ct);
        await ExportAsync(_deployEnvironmentName, _serviceInstalledFile);
        await PrintDistros();
        await StartAsync(_deployEnvironmentName);
    }
    private async Task<bool> InstallDistro(CancellationToken ct = default)
    {
        Console.Out.WriteLine("Requires Fresh Distro.");
        await InstallOfficialDebian();
        await SubsystemConfigure(ct);
        await ShudownAsync("Debian");
        await ExportAsync("Debian", _operatingSystemImgFile);
        await DestroyAsync("Debian");
        return true;
    }
    private async Task FinalizeDeployment()
    {
        List<Process> serviceShown = new();
        if (!Configuration.NoOpen)
        {
            IEnumerable<ServiceComposePayload>? toOpen = new List<ServiceComposePayload>();
            Configuration.PrintDebug($"Open Only: {Configuration.OpenOnly.Length}");
            if (Configuration.OpenOnly.Length > 0)
                toOpen = Configuration.Compose.Services.Where(p => Configuration.OpenOnly.Contains(p.ServiceName, StringComparer.OrdinalIgnoreCase));
            else
                toOpen = Configuration.Compose.Services.Where(p => p.Show);
            foreach (var item in toOpen)
                serviceShown.Add(ShowServiceAsync(_deployEnvironmentName, item.ServiceName!));

        }
        if (Configuration.OpenShell)
            serviceShown.Add(ShowShellAsync(_deployEnvironmentName));
        await WaitForCtrlCAsync();
        await CleanUp(serviceShown);
    }

    private async Task CleanUp(List<Process> processes)
    {
        if (Configuration.AutoKill)
            foreach (var item in processes)
                item.Kill();
        await ShudownAsync(_deployEnvironmentName);

    }
    public Task WaitForCtrlCAsync()
    {
        var tcs = new TaskCompletionSource();

        // 1. Standard handling for CTRL+C keys typed inside the terminal window
        Console.CancelKeyPress += (sender, e) =>
        {
            e.Cancel = true; // Block immediate exit sequence
            tcs.TrySetResult();
        };

        // 2. FIX: Catches the OS "Window Close / Kill" signal (clicking the 'X' button)
        using var closeSignal = PosixSignalRegistration.Create(PosixSignal.SIGTERM, context =>
        {
            context.Cancel = true; // Request a brief window to run cleanup code
            tcs.TrySetResult();
        });

        Console.WriteLine("Press CTRL+C or click the 'X' to cancel deployment and exit.");
        return tcs.Task;
    }
    private async Task SubsystemConfigure(CancellationToken ct = default)
    {
        await RunAsync("Debian", "echo -e '[boot]\nsystemd=true' > /etc/wsl.conf");
        await ShudownAsync("Debian");
        await StartAsync("Debian");
        var deps = _systemDependencies.Union(Configuration.Compose.Apt).ToArray();
        if (deps.Length > 0)
            await RunAsync("Debian", $"apt-get update && apt-get install -y {string.Join(' ', deps)}");
        var username = Guid.NewGuid().ToString().Replace("-", string.Empty);
        var password = Guid.NewGuid().ToString().Replace("-", string.Empty);
        Console.Out.WriteLine($"Random Username ({username}) and Password ({password}) generated.");
        await RunAsync("Debian", $"useradd -m {username}");
        await RunAsync("Debian", $"printf '%s:%s\n' '{username}' '{password}' | chpasswd");
        await RunAsync("Debian", $"printf '[user]\ndefault=%s\n' '{username}' >> /etc/wsl.conf");
        Console.Out.WriteLine($"Debian WSL Configure.");

        await ShudownAsync("Debian");
    }

    private async Task DeployServicesAsync(CancellationToken ct = default)
    {
        foreach (var item in Configuration.Compose.Services.Where(p => File.Exists(p.DotnetProject)))
            await PublishDotnetService(item);
        foreach (var item in Configuration.Compose.Services.Where(p => File.Exists(p.DotnetProject)))
            await CopyDotnetServices(item);

        foreach (var item in Configuration.Compose.Services.Where(p => p.DebUrl != default))
            await DownloadInstallDep(item);

        foreach (var item in Configuration.Compose.Services)
        {
            string serviceFile = GenerateServiceFile(item);
            await CopyFileAsync(_deployEnvironmentName, serviceFile, $"/etc/systemd/system/{item.ServiceName}.service");
        }
        await RunAsync(_deployEnvironmentName, $"systemctl daemon-reload");

        foreach (var item in Configuration.Compose.Services)
            await RunAsync(_deployEnvironmentName, $"systemctl enable {item.ServiceName}");

        await ShudownAsync(_deployEnvironmentName);

    }

    private string GenerateServiceFile(ServiceComposePayload serviceComposePayload)
    {
        var tmp = Path.GetTempPath();
        var cliSysmdTmp = Path.Combine(_tempLocation, "systemd");
        if (Directory.Exists(cliSysmdTmp))
            Directory.Delete(cliSysmdTmp, true);
        Directory.CreateDirectory(cliSysmdTmp);
        var serviceFileTmp = Path.Combine(cliSysmdTmp, Path.GetFileName(Path.GetTempFileName()));
        var commandBuilder = new StringBuilder();


        // 1. /etc/systemd/system/{serviceName}.service
        commandBuilder.AppendLine("[Unit]");
        commandBuilder.AppendLine($"Description={serviceComposePayload.Description}");
        commandBuilder.AppendLine($"After=network.target {string.Join(' ', serviceComposePayload.DependsOn)}");
        if (serviceComposePayload.DependsOn.Count > 0)
            commandBuilder.AppendLine($"Requires={string.Join(' ', serviceComposePayload.DependsOn)}");
        commandBuilder.AppendLine(); // Empty line separator
        commandBuilder.AppendLine("[Service]");
        commandBuilder.AppendLine("Type=simple");
        commandBuilder.AppendLine($"WorkingDirectory={serviceComposePayload.WorkingDir}");
        commandBuilder.AppendLine($"ExecStart={serviceComposePayload.ExecStart} {serviceComposePayload.StartupParameters ?? ""}");
        commandBuilder.AppendLine("Restart=no");
        foreach (var item in serviceComposePayload.Environment)
        {
            commandBuilder.AppendLine($"Environment={item}");

        }
        commandBuilder.AppendLine($"LogsDirectory={serviceComposePayload.ServiceName}");
        commandBuilder.AppendLine(); // Empty line separator
        commandBuilder.AppendLine("[Install]");
        commandBuilder.AppendLine("WantedBy=multi-user.target");
        string serviceFileContent = commandBuilder.ToString();
        Console.Out.WriteLine(serviceFileContent);
        File.WriteAllText(serviceFileTmp, serviceFileContent);
        return serviceFileTmp;
    }

    private async Task DownloadInstallDep(ServiceComposePayload serviceComposePayload)
    {
        await RunAsync(_deployEnvironmentName, $"wget '{serviceComposePayload.DebUrl!}' -O '/tmp/{serviceComposePayload.ServiceName}.deb");
        await RunAsync(_deployEnvironmentName, $"apt install -y '/tmp/{serviceComposePayload.ServiceName}.deb");
    }

    private async Task PublishDotnetService(ServiceComposePayload serviceComposePayload)
    {
        await PublishProjectAsync(serviceComposePayload.DotnetProject!);
    }

    private async Task CopyDotnetServices(ServiceComposePayload serviceComposePayload)
    {
        string filename = Path.GetFileNameWithoutExtension(serviceComposePayload.DotnetProject!);
        var tmp = Path.GetTempPath();
        var cliPublishTmp = Path.Combine(_tempLocation, "publish");
        var cliProjectOutput = Path.Combine(cliPublishTmp, filename);
        await CopyDirAsync(_deployEnvironmentName, cliProjectOutput, serviceComposePayload.WorkingDir!);
        await RunAsync(_deployEnvironmentName, $"ls '{serviceComposePayload.WorkingDir}'");

        Console.Out.WriteLine($"Service Target -> '{serviceComposePayload.ExecStart}'");
        await RunAsync(_deployEnvironmentName, $"[ -f '{serviceComposePayload.ExecStart}' ] || exit 1");
        await RunAsync(_deployEnvironmentName, $"chmod +x '{serviceComposePayload.ExecStart}'");
    }
    private async Task CreateSnapshot(string snap, CancellationToken ct = default)
    {
        string file = Path.Combine(_tempLocation, "snapshots", $"{snap}.tar.gz");
        if (!Directory.Exists(Path.Combine(_tempLocation, "snapshots")))
            Directory.CreateDirectory(Path.Combine(_tempLocation, "snapshots"));
        if (File.Exists(file))
            File.Delete(file);
        await ShudownAsync(_deployEnvironmentName);
        await ExportAsync(_deployEnvironmentName, file);
        await StartAsync(_deployEnvironmentName);
    }
}

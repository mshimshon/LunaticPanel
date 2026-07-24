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
    public ConfigurationPayload Configuration { get; }
    public DeploymentService(ConfigurationPayload configuration)
    {
        var tmp = Path.GetTempPath();
        var cliTmp = Path.Combine(tmp, "lpcli");
        var cliWSLDataTmp = Path.Combine(cliTmp, "data");
        if (!Directory.Exists(cliWSLDataTmp))
            Directory.CreateDirectory(cliWSLDataTmp);
        _operatingSystemImgFile = Path.Combine(cliTmp, "os_debian.tar.gz");
        _serviceInstalledFile = Path.Combine(cliTmp, "os_debian_service.tar.gz");
        _wslDataFolder = cliWSLDataTmp;
        Configuration = configuration;
    }


    public async Task DeployAsync(CancellationToken ct = default)
    {
        bool isFresh = false;
        Console.Out.WriteLine("Checking Distro Availability");
        bool debianExist = await WslDistroExists("Debian");


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
            {
                await DestroyAsync(_deployEnvironmentName);
            }
            await ImportAsync(_deployEnvironmentName, _wslDataFolder, _serviceInstalledFile);
        }
        await InstallPlugins(ct);
        await ShudownAsync(_deployEnvironmentName);
        await StartAsync(_deployEnvironmentName);
        foreach (var item in Configuration.Compose.Services)
            await RunAsync(_deployEnvironmentName, $"systemctl status {item.ServiceName}");

        await RunAsync(_deployEnvironmentName, $"cat /etc/lunaticpanel/bootstrap.json");
        await FinalizeDeployment();
    }

    private async Task InstallPlugins(CancellationToken ct = default)
    {
        var csprojects = Configuration.Compose.Plugins.Where(p => p.DotnetProject != default && p.DotnetProject.EndsWith(".csproj"));
        foreach (var item in csprojects)
            await BuildProjectAsync(item.DotnetProject!);
        Dictionary<PluginComposePayload, PackToolPluginManifestExternalPayload> manifests = new();
        foreach (var item in csprojects)
        {
            var manifestTmp = await PackProjectAsync(item, ct);
            manifests[item] = manifestTmp;
        }
        List<JsonObject> bootstrapDefinition = new List<JsonObject>();
        foreach (var item in manifests)
        {
            var def = await InstallPluginInSubSystem(item.Key, item.Value, ct);
            bootstrapDefinition.Add(def);
        }

        await BuildAndPublishBootstrap(bootstrapDefinition);
    }

    private async Task<JsonObject> InstallPluginInSubSystem(PluginComposePayload csproj, PackToolPluginManifestExternalPayload manifestExternalPayload, CancellationToken ct = default)
    {
        string linuxFolder = manifestExternalPayload.Id.ToLower().Replace('.', '_');
        string filename = Path.GetFileNameWithoutExtension(csproj.DotnetProject!);
        var tmp = Path.GetTempPath();
        var cliTmp = Path.Combine(tmp, "lpcli");
        var cliLpkgUnpackTmp = Path.Combine(cliTmp, "lpkgs_unpack");
        var cliLpkgDir = Path.Combine(cliLpkgUnpackTmp, linuxFolder);
        var cliLpkgTmp = Path.Combine(cliTmp, "lpkgs");

        var clilpkgLocal = Path.Combine(cliLpkgTmp, $"{manifestExternalPayload.Id}.{manifestExternalPayload.Version}.lpkg");
        // /usr/lib/lunaticpanel/plugins
        await CopyDirAsync(_deployEnvironmentName, cliLpkgDir, $"/usr/lib/lunaticpanel/plugins/{linuxFolder}");
        await RunAsync(_deployEnvironmentName, $"ls /usr/lib/lunaticpanel/plugins/{linuxFolder}");
        ///var/lib/lunaticpanel_package/lpkgs
        await CopyFileAsync(_deployEnvironmentName, clilpkgLocal, $"/lib/lunaticpanel_package/lpkgs/{manifestExternalPayload.Id}.{manifestExternalPayload.Version}.lpkg");
        // 1. Build the leaf objects (Identity and Lifecycle)
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
            ["PluginDir"] = $"/usr/lib/lunaticpanel/plugins/{linuxFolder}"
        };
        return pluginEntry;

    }
    private async Task<PackToolPluginManifestExternalPayload> PackProjectAsync(PluginComposePayload pluginComposePayload, CancellationToken ct = default)
    {
        string filename = Path.GetFileNameWithoutExtension(pluginComposePayload.DotnetProject!);
        var tmp = Path.GetTempPath();
        var cliTmp = Path.Combine(tmp, "lpcli");
        var cliPublishTmp = Path.Combine(cliTmp, "build");
        var cliLpkgTmp = Path.Combine(cliTmp, "lpkgs");
        var cliLpkgUnpackTmp = Path.Combine(cliTmp, "lpkgs_unpack");
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
            throw new Exception($"Failed to get manifest for {pluginComposePayload.DotnetProject}");
        for (int i = startIndex + 1; i < stopIndex; i++)
        {
            jsonData.AppendLine(lines[i]);
        }
        var tmpJsonResult = jsonData.ToString();
        finalData = string.IsNullOrWhiteSpace(tmpJsonResult) ? default : tmpJsonResult;

        Console.Out.WriteLine($"Payload Found ? {payloadFound} ({finalData != default})");

        try
        {
            if (finalData == default)
                throw new Exception($"Failed to get manifest for {pluginComposePayload.DotnetProject}");
            PackToolResultExternalPayload? resultReponse = JsonSerializer.Deserialize<PackToolResultExternalPayload>(finalData);
            if (resultReponse == default || resultReponse.Data == default)
                throw new Exception($"Failed to read manifest for {finalData}");
            Console.Out.WriteLine($"Plugin {resultReponse.Data.Id} is packed and ready to deploy.");
            string linuxName = resultReponse.Data.Id.ToLower().Replace('.', '_');
            string unpackTo = Path.Combine(cliLpkgUnpackTmp, linuxName);
            if (Directory.Exists(unpackTo))
                Directory.Delete(unpackTo, true);
            Directory.CreateDirectory(unpackTo);
            var cmdUnpack = $"unpack --root --input \"{Path.Combine(cliLpkgTmp, $"{resultReponse.Data.Id}.{resultReponse.Data.Version}.lpkg")}\" --output \"{unpackTo}\"";
            await ProcessExt.RunProcessAsync(cliPackingTool, cmdUnpack);
            return resultReponse.Data;
        }
        catch
        {
            Console.Error.WriteLine(finalData);
            throw;
        }


    }
    private async Task BuildAndPublishBootstrap(List<JsonObject> defs, CancellationToken ct = default)
    {
        var tmp = Path.GetTempPath();
        var cliTmp = Path.Combine(tmp, "lpcli");
        var cliPublishTmp = Path.Combine(cliTmp, "bootstrap.json");
        if (File.Exists(cliPublishTmp))
            File.Delete(cliPublishTmp);

        ///etc/lunaticpanel/bootstrap.json
        var root = new JsonObject
        {
            ["KnownPlugins"] = new JsonArray(defs.ToArray<JsonNode>())
        };
        string json = JsonSerializer.Serialize(root);
        File.WriteAllText(cliPublishTmp, json);
        await CopyFileAsync(_deployEnvironmentName, cliPublishTmp, "/etc/lunaticpanel/bootstrap.json");


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
        return true;
    }
    private async Task FinalizeDeployment()
    {
        List<Process> serviceShown = new();
        foreach (var item in Configuration.Compose.Services.Where(p => p.Show))
            serviceShown.Add(ShowServiceAsync(_deployEnvironmentName, item.ServiceName!));

        await WaitForCtrlCAsync();
        await CleanUp(serviceShown);
    }

    private async Task CleanUp(List<Process> processes)
    {
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
        var cliTmp = Path.Combine(tmp, "lpcli");
        var cliSysmdTmp = Path.Combine(cliTmp, "systemd");
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
        commandBuilder.AppendLine("Restart=always");
        commandBuilder.AppendLine("RestartSec=5");
        commandBuilder.AppendLine($"User=root");
        foreach (var item in serviceComposePayload.Environment)
        {
            commandBuilder.AppendLine($"Environment={item}");

        }
        commandBuilder.AppendLine($"LogsDirectory={serviceComposePayload.ServiceName}");
        commandBuilder.AppendLine($"StandardOutput=file:/var/log/{serviceComposePayload.ServiceName}.stdout.log");
        commandBuilder.AppendLine($"StandardError=file:/var/log/{serviceComposePayload.ServiceName}.stderr.log");
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
        var cliTmp = Path.Combine(tmp, "lpcli");
        var cliPublishTmp = Path.Combine(cliTmp, "publish");
        var cliProjectOutput = Path.Combine(cliPublishTmp, filename);
        await CopyDirAsync(_deployEnvironmentName, cliProjectOutput, serviceComposePayload.WorkingDir);
        await RunAsync(_deployEnvironmentName, $"ls '{serviceComposePayload.WorkingDir}'");

        Console.Out.WriteLine($"Service Target -> '{serviceComposePayload.ExecStart}'");
        await RunAsync(_deployEnvironmentName, $"[ -f '{serviceComposePayload.ExecStart}' ] || exit 1");
        await RunAsync(_deployEnvironmentName, $"chmod +x '{serviceComposePayload.ExecStart}'");
    }

}

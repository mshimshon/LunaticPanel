using LunaticPanel.DebugTool.Payloads;
using System.Diagnostics;
using System.Text;
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
        Console.Out.WriteLine($"Checking Distro Availability 'Debian' ({debianExist})");
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
        {

            Console.Out.WriteLine("Requires Fresh Distro.");
            await InstallOfficialDebian();
            await SubsystemConfigure(ct);
            await ShudownAsync("Debian");
            await ExportAsync("Debian", _operatingSystemImgFile);
            await ImportAsync(_deployEnvironmentName, _wslDataFolder, _operatingSystemImgFile);
            await StartAsync(_deployEnvironmentName);
            isFresh = true;
        }
        else
            Console.Out.WriteLine($"Use Existing '{_operatingSystemImgFile}'.");

        bool deployExist = await WslDistroExists(_deployEnvironmentName);
        if (deployExist)
            await DestroyAsync(_deployEnvironmentName);


        bool serviceInstallRequired = !Configuration.SkipServiceRebuild || !File.Exists(_serviceInstalledFile) || isFresh || !deployExist;
        if (serviceInstallRequired)
        {
            Console.Out.WriteLine("Requires Fresh Service Deployment.");
            await DeployServicesAsync(ct);
            await ShudownAsync(_deployEnvironmentName);
            await ExportAsync(_deployEnvironmentName, _serviceInstalledFile);
        }
        else
        {
            Console.Out.WriteLine($"Use Existing '{_serviceInstalledFile}'.");
            await DestroyAsync(_deployEnvironmentName);
            await ImportAsync(_deployEnvironmentName, _wslDataFolder, _operatingSystemImgFile);
        }
        List<Process> serviceShown = new();
        foreach (var item in Configuration.Compose.Services.Where(p => p.Value.Show))
            serviceShown.Add(ShowServiceAsync(_deployEnvironmentName, item.Key));

        foreach (var item in serviceShown)
            await item.WaitForExitAsync();
        if (serviceShown.Count <= 0)
            await WaitForCtrlCAsync();
        else
            Console.WriteLine("Clearing WSL environment.");
        await ShudownAsync(_deployEnvironmentName);
    }
    public static Task WaitForCtrlCAsync()
    {
        var tcs = new TaskCompletionSource();

        Console.CancelKeyPress += (sender, e) =>
        {
            e.Cancel = true;     // prevent process termination
            tcs.TrySetResult();  // complete the task
        };

        Console.WriteLine("Press CTRL+C to shutdown deployment and exit.");

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
        foreach (var item in Configuration.Compose.Services.Where(p => File.Exists(p.Value.DotnetProject)))
            await PublishDotnetService(item.Value);
        foreach (var item in Configuration.Compose.Services.Where(p => File.Exists(p.Value.DotnetProject)))
            await CopyDotnetServices(item.Value);

        foreach (var item in Configuration.Compose.Services.Where(p => p.Value.DebUrl != default))
            await DownloadInstallDep(item.Key, item.Value);

        foreach (var item in Configuration.Compose.Services)
            await RunAsync(_deployEnvironmentName, GenerateServiceFile(item.Key, item.Value));
        await RunAsync(_deployEnvironmentName, $"systemctl daemon-reload");
        foreach (var item in Configuration.Compose.Services)
            await RunAsync(_deployEnvironmentName, $"systemctl enable {item.Key}");
        await RunAsync(_deployEnvironmentName, $"reboot");
        foreach (var item in Configuration.Compose.Services)
            await RunAsync(_deployEnvironmentName, $"systemctl status {item.Key}");
    }

    private string GenerateServiceFile(string serviceName, ServiceComposePayload serviceComposePayload)
    {
        var commandBuilder = new StringBuilder();

        // 1. Define the Heredoc file creation command
        commandBuilder.AppendLine($"sudo tee /etc/systemd/system/{serviceName}.service << 'EOF'");
        commandBuilder.AppendLine("[Unit]");
        commandBuilder.AppendLine($"Description={serviceComposePayload.Description}");
        commandBuilder.AppendLine($"After=network.target {string.Join(' ', serviceComposePayload.DependsOn)}");
        if (serviceComposePayload.DependsOn.Count > 0)
            commandBuilder.AppendLine($"Requires={string.Join(' ', serviceComposePayload.DependsOn)}");
        commandBuilder.AppendLine(); // Empty line separator
        commandBuilder.AppendLine("[Service]");
        commandBuilder.AppendLine("Type=simple");
        commandBuilder.AppendLine($"ExecStart={serviceComposePayload.ExecStart}");
        commandBuilder.AppendLine("Restart=always");
        commandBuilder.AppendLine("RestartSec=5");
        commandBuilder.AppendLine($"User=root");
        commandBuilder.AppendLine($"LogsDirectory={serviceName}");
        commandBuilder.AppendLine($"StandardOutput=file:/var/log/{serviceName}.stdout.log");
        commandBuilder.AppendLine($"StandardError=file:/var/log/{serviceName}.stderr.log");
        commandBuilder.AppendLine(); // Empty line separator
        commandBuilder.AppendLine("[Install]");
        commandBuilder.AppendLine("WantedBy=multi-user.target");
        // 3. Close the Heredoc
        commandBuilder.AppendLine("EOF");

        // Convert to final string variable
        return commandBuilder.ToString();
    }

    private async Task DownloadInstallDep(string serviceName, ServiceComposePayload serviceComposePayload)
    {
        await RunAsync(_deployEnvironmentName, $"wget '{serviceComposePayload.DebUrl!}' -O '/tmp/{serviceName}.deb");
        await RunAsync(_deployEnvironmentName, $"apt install -y '/tmp/{serviceName}.deb");
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
    }

}

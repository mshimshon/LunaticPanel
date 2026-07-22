using LunaticPanel.DebugTool.Payloads;
using System.Text;
using static LunaticPanel.DebugTool.Extensions.SubsystemExt;
namespace LunaticPanel.DebugTool.Services;

internal sealed class DeploymentService
{

    private readonly string _operatingSystemImgFile;
    private readonly string _serviceInstalledFile;
    private readonly string _wslDataFolder;
    private readonly string _deployEnvironmentName = "lpcli_deploy";

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


    public async Task DeployAsync(bool installDebian, bool installServices, CancellationToken ct = default)
    {
        bool isFresh = false;

        bool debianExist = await WslDistroExists("Debian");
        if (debianExist)
            await DestroyAsync("Debian");

        bool deployExist = await WslDistroExists(_deployEnvironmentName);
        if (deployExist)
            await DestroyAsync(_deployEnvironmentName);

        bool distroInstallRequired = installDebian || !File.Exists(_operatingSystemImgFile) || !deployExist;
        if (distroInstallRequired)
        {
            await InstallOfficialDebian();
            await SubsystemConfigure(ct);
            await ShudownAsync("Debian");
            await ExportAsync("Debian", _operatingSystemImgFile);
            await ImportAsync(_deployEnvironmentName, _wslDataFolder, _operatingSystemImgFile);
            await StartAsync(_deployEnvironmentName);
            isFresh = true;
        }

        bool serviceInstallRequired = installServices || !File.Exists(_serviceInstalledFile) || isFresh || !deployExist;
        if (installServices || !File.Exists(_serviceInstalledFile) || isFresh)
        {
            await DeployServicesAsync(ct);
            await ShudownAsync(_deployEnvironmentName);
            await ExportAsync(_deployEnvironmentName, _serviceInstalledFile);
        }
        else
        {
            await DestroyAsync(_deployEnvironmentName);
            await ImportAsync(_deployEnvironmentName, _wslDataFolder, _operatingSystemImgFile);
        }
    }

    private async Task SubsystemConfigure(CancellationToken ct = default)
    {
        await RunAsync("Debian", "echo -e '[boot]\nsystemd=true' > /etc/wsl.conf");
        await ShudownAsync("Debian");
        await StartAsync("Debian");
        if (Configuration.Compose.Apt.Count > 0)
            await RunAsync("Debian", $"apt-get update && apt-get install -y {string.Join(' ', Configuration.Compose.Apt)}");
        var username = Guid.NewGuid().ToString().Replace("-", string.Empty);
        var password = Guid.NewGuid().ToString().Replace("-", string.Empty);
        await RunAsync("Debian", "printf '%s:%s\n' '$Username' '$Password' | chpasswd");
        await RunAsync("Debian", "printf '[user]\ndefault=%s\n' '$Username' >> /etc/wsl.conf");
        await ShudownAsync("Debian");
    }
    private async Task DeployServicesAsync(CancellationToken ct = default)
    {
        foreach (var item in Configuration.Compose.Services.Where(p => File.Exists(p.Value.DotnetProject))
            await PublishDotnetService(item.Value);

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

    private async Task PublishDotnetService(ServiceComposePayload serviceComposePayload)
    {

    }

}

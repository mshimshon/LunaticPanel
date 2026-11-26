using LunaticPanel.Core.Extensions;
using NuGet.Versioning;
using System;
using System.Reflection;

namespace LunaticPanel.Engine.Bootstraper.Discovery;

public class UpdateDiscovery : IUpdateDiscovery
{
    private readonly IUpdateDiscoveryLocator _updateDiscoveryLocation;
    private readonly IPluginDiscoveryLocator _pluginDiscoveryLocation;
    internal string _upgradeMarkerFile;
    public string UpgradeMarkerFile => _upgradeMarkerFile;

    internal string _upgradePanelFile;
    public string UpgradePanelFile => _upgradePanelFile;

    internal string _failSafePanelFile;
    public string FailSafePanelFile => _failSafePanelFile;

    internal string _failSafePluginFile;
    public string FailSafePluginFile => _failSafePluginFile;
    internal string _accessBinMark;
    public string AccessBinMark => _accessBinMark;

    internal string _accessUpgradeRootAccessMark;
    public string AccessUpgradeRootAccessMark => _accessUpgradeRootAccessMark;

    internal string _accessPluginRootMark;
    private bool _disposedValue;

    public string AccessPluginRootMark => _accessPluginRootMark;
    public NuGetVersion? UpgradeTo { get; set; }

    public UpdateDiscovery(
        IUpdateDiscoveryLocator updateDiscoveryLocation,
        IPluginDiscoveryLocator pluginDiscoveryLocation)
    {
        _updateDiscoveryLocation = updateDiscoveryLocation;
        _pluginDiscoveryLocation = pluginDiscoveryLocation;
        _upgradePanelFile = Pathf.CombineReadyFile(_updateDiscoveryLocation.UpgradeReadyApply, "lunaticpanel_latest.tar.gz");
        _failSafePanelFile = Pathf.CombineReadyFile(_updateDiscoveryLocation.RuntimeBackupRoot, "panel.tar.gz");
        _failSafePluginFile = Pathf.CombineReadyFile(_updateDiscoveryLocation.RuntimeBackupRoot, "plugins.tar.gz");

        _upgradeMarkerFile = Pathf.CombineReadyFile(_updateDiscoveryLocation.UpgradeReadyApply, "version.txt");
        _accessBinMark = Pathf.CombineReadyFile(_updateDiscoveryLocation.BinaryPathRoot, "access.test");
        _accessUpgradeRootAccessMark = Pathf.CombineReadyFile(_updateDiscoveryLocation.UpgradeRoot, "access.test");
        _accessPluginRootMark = Pathf.CombineReadyFile(_pluginDiscoveryLocation.PluginLifecycleLocation, "access.test");
        LoadVersionFile();
    }
    private bool CheckAccessMark(string file, string name)
    {
        bool flag = File.Exists(file);
        if (flag)
        {
            System.Console.WriteLine($"Upgrade Mark Found for {name} ('{file}').".Green());
        }
        else if (!flag)
        {
            System.Console.WriteLine($"No Upgrade Possible, Missing {name} ('{file}').".Red());
        }
        return flag;
    }
    private bool ConsumeAccessMark(string file)
    {
        try
        {
            File.Delete(file);
            return true;
        }
        catch (System.Exception ex)
        {
            System.Console.Error.WriteLine($"Coul'd not consume access mark ('{file}') due to {ex.Message}.");
            return false;
        }
    }

    public bool CanUpgrade()
    {
        bool haveAllMarks = CheckAccessMark(_upgradeMarkerFile, "Upgrade Version Mark") &&
        CheckAccessMark(_accessBinMark, "Panel Bin Root") &&
        CheckAccessMark(_accessUpgradeRootAccessMark, "Panel Upgrade Staging Root") &&
        CheckAccessMark(_accessPluginRootMark, "Plugin Staging Root");
        bool hasNoUpdatePending = Directory.GetFiles(_pluginDiscoveryLocation.PluginLifecycleApplyLocation, "*.lpkg", SearchOption.TopDirectoryOnly).Count() <= 0;
        bool hasNoDeletePending = Directory.GetFiles(_pluginDiscoveryLocation.PluginLifecycleDeleteLocation, "*.lpkg", SearchOption.TopDirectoryOnly).Count() <= 0;
        return haveAllMarks && hasNoUpdatePending && hasNoDeletePending;
    }
    public void LoadVersionFile()
    {
        if (!File.Exists(_upgradeMarkerFile))
        {
            System.Console.WriteLine($"No Version Marked ('{_upgradeMarkerFile}').".Red());
            return;
        }
        UpgradeTo = NuGetVersion.Parse(File.ReadAllText(_upgradeMarkerFile));
    }

    public async Task StartUpgrade()
    {

        if (!CanUpgrade())
        {
            System.Console.WriteLine($"No Upgrade Available.".Red());
            return;
        }
        if (!ConsumeAccessMark(_accessBinMark) ||
            !ConsumeAccessMark(_accessUpgradeRootAccessMark) ||
            !ConsumeAccessMark(_accessPluginRootMark))
        {
            System.Console.WriteLine("Failed to Consume Upgrade Access Mark, Continue Booting.".Red());
            return;
        }
        System.Console.WriteLine($"Access Testing and Granted, Performing Upgrade Now.".Green());
        await BackUpCurrentPanel();
        await ApplyUpgrade();

    }
    /// <summary>
    /// This is to avoid .NET to resolve after directory cleared assemblies, it is very fragile and is only used to
    /// delete binary path and apply upgrade then full restart is required.
    /// </summary>
    private void LockAssemblies()
    {
        string binPath = AppDomain.CurrentDomain.BaseDirectory;
        foreach (string dllPath in Directory.GetFiles(binPath, "*.dll"))
        {
            try
            {
                Assembly assembly = Assembly.LoadFrom(dllPath);
                _ = assembly.GetTypes();
            }
            catch (Exception)
            {
                // Handle or ignore native/unmanaged DLLs that can't be loaded this way
            }
        }
    }

    public async Task ApplyUpgrade()
    {
        try
        {
            System.Console.WriteLine("Updating Binary...".Cyan());
            LockAssemblies();
            Pathf.ResetDir(_updateDiscoveryLocation.BinaryPathRoot);
            Pathf.ResetDir(_pluginDiscoveryLocation.PluginLifecycleApplyLocation);
            Pathf.ResetDir(_pluginDiscoveryLocation.PluginLifecycleDeleteLocation);
            Pathf.ResetDir(_pluginDiscoveryLocation.PluginLifecycleInstalledLocation);
            Pathf.ResetDir(_pluginDiscoveryLocation.PluginLifecycleRollbackLocation);
            Pathf.ResetDir(_pluginDiscoveryLocation.PluginLifecycleRuntimeLocation);
            Pathf.ResetDir(_pluginDiscoveryLocation.PluginLifecyclePreInstalledLocation);
            System.Console.WriteLine("Directories Resetted".Green());

            System.Console.WriteLine("Installing New Panel".Yellow());
            await ArchiveUtilityExt.ExtractTo(_upgradePanelFile, _updateDiscoveryLocation.BinaryPathRoot, ArchiveType.TarGz);
            System.Console.WriteLine($"Binary Successfully Upgraded to {UpgradeTo!}.".Green());
            UpgradePreInstalledPlugins();
            UpgradePlugins();


            System.Console.WriteLine("Upgrade Apply Completed... Restart Required.".Green());
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine($"Upgrade Failed (::{ex.Message}::)".Green());
            await FailSafeRestoring();
            throw;
        }
    }
    private void UpgradePlugins()
    {
        if (Directory.Exists(_updateDiscoveryLocation.UpgradePluginInstalled))
        {
            foreach (var item in Directory.GetFiles(_updateDiscoveryLocation.UpgradePluginInstalled, "*.lpkg", SearchOption.AllDirectories))
            {
                System.Console.WriteLine($"Restoring '{item}'.");
                string filename = Path.GetFileName(item);
                File.Move(item, Path.Combine(_pluginDiscoveryLocation.PluginLifecycleApplyLocation, filename));
                System.Console.WriteLine($"Restored '{item}'.");
            }
        }
        else
        {
            System.Console.WriteLine($"The panel had no user installed plugins.".Yellow());
        }
    }
    private void UpgradePreInstalledPlugins()
    {
        if (Directory.Exists(_updateDiscoveryLocation.UpgradePluginPreinstalled))
        {
            foreach (var item in Directory.GetFiles(_updateDiscoveryLocation.UpgradePluginPreinstalled, "*.lpkg", SearchOption.AllDirectories))
            {
                System.Console.WriteLine($"Restoring '{item}'.".Cyan());
                string filename = Path.GetFileName(item);
                File.Copy(item, Path.Combine(_pluginDiscoveryLocation.PluginLifecycleApplyLocation, filename));
                File.Move(item, Path.Combine(_pluginDiscoveryLocation.PluginLifecyclePreInstalledLocation, filename));
                System.Console.WriteLine($"Restored '{item}'.".Green());
            }
        }
        else
        {
            System.Console.WriteLine($"The panel was not shipped with preinstalled plugins.".Yellow());
        }
    }
    public async Task BackUpCurrentPanel()
    {
        // Clear PreExisting if Any
        ClearFile(_failSafePanelFile);
        ClearFile(_failSafePluginFile);
        System.Console.WriteLine("Creating Fail-Safe Backup.".Cyan());
        try
        {
            await ArchiveUtilityExt.PackTarGz(_updateDiscoveryLocation.BinaryPathRoot, _failSafePanelFile);
            await ArchiveUtilityExt.PackTarGz(_pluginDiscoveryLocation.PluginLifecycleLocation, _failSafePluginFile);

        }
        catch (System.Exception ex)
        {
            await FailSafeRestoring();
            System.Console.WriteLine($"Back up failure (:: {ex.Message} ::).".Red());
            throw;
        }
        System.Console.WriteLine($"Back Up Completed.".Green());

    }
    private void ClearFile(string file)
    {
        if (File.Exists(file))
            File.Delete(file);
    }


    public async Task FailSafeRestoring()
    {
        if (File.Exists(_failSafePanelFile))
        {
            System.Console.WriteLine("Restoring Panel.".Yellow());
            if (Directory.Exists(_updateDiscoveryLocation.BinaryPathRoot))
                Pathf.ResetDir(_updateDiscoveryLocation.BinaryPathRoot);
            await ArchiveUtilityExt.ExtractTo(_failSafePanelFile, _updateDiscoveryLocation.BinaryPathRoot, ArchiveType.TarGz);
            File.Delete(_failSafePanelFile);
        }

        if (File.Exists(_failSafePluginFile))
        {
            System.Console.WriteLine("Restoring PLugins.".Yellow());
            if (Directory.Exists(_pluginDiscoveryLocation.PluginLifecycleLocation))
                Pathf.ResetDir(_pluginDiscoveryLocation.PluginLifecycleLocation);
            await ArchiveUtilityExt.ExtractTo(_failSafePluginFile, _pluginDiscoveryLocation.PluginLifecycleLocation, ArchiveType.TarGz);
            File.Delete(_failSafePluginFile);
        }
        System.Console.WriteLine("Restore Success.".Green());

    }

    public Task ClearStageAsync()
    {
        Pathf.ResetDir(_updateDiscoveryLocation.UpgradePluginInstalled);
        Pathf.ResetDir(_updateDiscoveryLocation.UpgradePluginPreinstalled);
        Pathf.ResetDir(_updateDiscoveryLocation.UpgradeReadyApply);
        return Task.CompletedTask;
    }

}

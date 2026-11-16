using LunaticPanel.Core.Extensions;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Net;
using System.Reflection;

namespace LunaticPanel.Engine.Web.Boostrap;

public static class BootstrapPanelUpdates
{
    private static string _upgradeRootFolder = default!;
    private static string _binPanelPath = default!;
    private static string _binPreinstalledPlugins = default!;
    private static string _pluginStageLocationRootFolder = default!;
    private static string _pluginApplyLocationFolder = default!;
    private static string _pluginDeleteLocationFolder = default!;
    private static string _pluginRollbackLocationFolder = default!;
    private static string _pluginInstalledLocationFolder = default!;
    private static string _upgradeApplyFolder = default!;
    private static string _applyVersionCacheMark = default!;
    private static string _binAccessMark = default!;
    private static string _upgradeRootAccessMark = default!;
    private static string _pluginStagingRootAccessMark = default!;
    private static string _runtimeBackupRoot = default!;
    private static string _runtimeBackupPanel = default!;
    private static string _runtimeBackupPlugins = default!;
    private static string _upgradeApplyPanelFile = default!;
    private static string _upgradeApplyPreInstalledPluginFolder = default!;
    private static string _upgradeApplyUserInstalledPluginFolder = default!;
    private static string _varLibLunaticPanel = default!;
    private static string _varLib = default!;
    private static string _srv = default!;

    public static async Task ApplyPanelUpdate()
    {
        _srv = Path.Combine($"{Path.DirectorySeparatorChar}", "srv");
        _varLib = Path.Combine($"{Path.DirectorySeparatorChar}", "var", "lib");
        _upgradeRootFolder = Path.Combine(_varLib, "lunaticpanel", ".upgrade");
        _binPanelPath = Path.Combine(_srv, "lunaticpanel");
        _varLibLunaticPanel = Path.Combine(_varLib, "lunaticpanel");
        _binPreinstalledPlugins = Path.Combine(_binPanelPath, "preinstalled");
        _pluginStageLocationRootFolder = Path.Combine(_varLibLunaticPanel, ".plugins");
        _pluginApplyLocationFolder = Path.Combine(_pluginStageLocationRootFolder, "apply");
        _pluginDeleteLocationFolder = Path.Combine(_pluginStageLocationRootFolder, "delete");
        _pluginRollbackLocationFolder = Path.Combine(_pluginStageLocationRootFolder, "rollbacks");
        _pluginInstalledLocationFolder = Path.Combine(_pluginStageLocationRootFolder, "installed");
        _upgradeApplyFolder = Path.Combine(_upgradeRootFolder, "apply");
        _upgradeApplyPreInstalledPluginFolder = Path.Combine(_upgradeApplyFolder, "plugin_preinstalled");
        _upgradeApplyUserInstalledPluginFolder = Path.Combine(_upgradeApplyFolder, "plugin_user_installed");
        _upgradeApplyPanelFile = Path.Combine(_upgradeApplyFolder, "lunaticpanel_latest.tar.gz");
        _applyVersionCacheMark = Path.Combine(_upgradeApplyFolder, "version.txt");
        _binAccessMark = Path.Combine(_binPanelPath, "access.test");
        _upgradeRootAccessMark = Path.Combine(_upgradeRootFolder, "access.test");
        _pluginStagingRootAccessMark = Path.Combine(_pluginStageLocationRootFolder, "access.test");

        _runtimeBackupRoot = EnsureCreated(Path.Combine(Path.GetTempPath(), "lunaticpanel", "upgrade_backup"));
        _runtimeBackupPanel = Path.Combine(_runtimeBackupRoot, "panel.tar.gz");
        _runtimeBackupPlugins = Path.Combine(_runtimeBackupRoot, "plugins.tar.gz");
        // Any Mark Missing, Do not perform update and move
        if (!CheckAccessMark(_applyVersionCacheMark, "Upgrade Version Mark") ||
            !CheckAccessMark(_binAccessMark, "Panel Bin Root") ||
            !CheckAccessMark(_upgradeRootAccessMark, "Panel Upgrade Staging Root") ||
            !CheckAccessMark(_pluginStagingRootAccessMark, "Plugin Staging Root"))
        {
            System.Console.WriteLine("No Upgrade Found, Continue Booting.");
            return;
        }
        string upgradeCacheVersion = File.ReadAllText(_applyVersionCacheMark);
        System.Console.WriteLine($"Pending Upgrade Found to v{upgradeCacheVersion}, Proceed to Install.");
        /*
            File/Folder Control, This is very delicate process we must minimize possible issues during
            this vulnerable period of the panel, any failure could render the panel inaccessible
            or stuck in a deadlock upgrade loop.
        */
        if (!ConsumeAccessMark(_binAccessMark) ||
            !ConsumeAccessMark(_upgradeRootAccessMark) ||
            !ConsumeAccessMark(_pluginStagingRootAccessMark))
        {
            System.Console.WriteLine("Failed to Consume Upgrade Access Mark, Continue Booting.");

            return;
        }
        System.Console.WriteLine($"Access Testing and Granted, Performing Upgrade.");
        await PreApplyBackUp();
        await ApplyPatch();
        System.Console.WriteLine($"Panel v{upgradeCacheVersion} installed, restart.");
        Environment.Exit(0);

    }

    private static async Task ApplyPatch()
    {
        try
        {
            System.Console.WriteLine("Updating Binary...");
            LockAssemblies();
            ResetDirectory(_binPanelPath);
            ResetDirectory(_pluginStageLocationRootFolder);
            ResetDirectory(_pluginStageLocationRootFolder);
            ResetDirectory(_pluginApplyLocationFolder);
            ResetDirectory(_binPreinstalledPlugins);
            await ArchiveUtilityExt.ExtractTo(_upgradeApplyPanelFile, _binPanelPath, ArchiveType.TarGz);
            if (Directory.Exists(_upgradeApplyPreInstalledPluginFolder))
            {

                System.Console.WriteLine("Binary Successfully Upgraded.");

                foreach (var item in Directory.GetFiles(_upgradeApplyPreInstalledPluginFolder, "*.lpkg", SearchOption.AllDirectories))
                {
                    System.Console.WriteLine($"Restoring '{item}'.");
                    string filename = Path.GetFileName(item);
                    File.Copy(item, Path.Combine(_pluginApplyLocationFolder, filename));
                    File.Move(item, Path.Combine(_binPreinstalledPlugins, filename));
                    System.Console.WriteLine($"Restored '{item}'.");
                }

            }

            foreach (var item in Directory.GetFiles(_upgradeApplyUserInstalledPluginFolder, "*.lpkg", SearchOption.AllDirectories))
            {
                System.Console.WriteLine($"Restoring '{item}'.");
                string filename = Path.GetFileName(item);
                File.Move(item, Path.Combine(_pluginApplyLocationFolder, filename));
                System.Console.WriteLine($"Restored '{item}'.");
            }

            System.Console.WriteLine("Upgrade Apply Completed.");
        }
        catch (System.Exception)
        {
            await RestoreBackup();
            Environment.Exit(0);
            throw;
        }
    }
    public static void LockAssemblies()
    {
        // Get the directory where the application is executing
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

    private static void ResetDirectory(string path)
    {
        if (Directory.Exists(path))
            Directory.Delete(path, true);
        EnsureCreated(path);
    }
    private static async Task RestoreBackup()
    {
        if (File.Exists(_runtimeBackupPanel))
        {
            System.Console.WriteLine("Restoring Panel.");
            if (Directory.Exists(_binPanelPath))
                ResetDirectory(_binPanelPath);
            await ArchiveUtilityExt.ExtractTo(_runtimeBackupPanel, _binPanelPath, ArchiveType.TarGz);
            Directory.Exists(_runtimeBackupPanel);
        }

        if (File.Exists(_runtimeBackupPlugins))
        {
            System.Console.WriteLine("Restoring PLugins.");
            if (Directory.Exists(_pluginStageLocationRootFolder))
                ResetDirectory(_pluginStageLocationRootFolder);

            await ArchiveUtilityExt.ExtractTo(_runtimeBackupPlugins, _pluginStageLocationRootFolder, ArchiveType.TarGz);
            Directory.Exists(_runtimeBackupPlugins);
        }
        System.Console.WriteLine("Restore Success.");

    }

    private static async Task PreApplyBackUp()
    {

        System.Console.WriteLine("Runtime Upgrade Fail-Safe Backup.");
        System.Console.WriteLine("Backing Up Panel Binary...");
        await ArchiveUtilityExt.PackTarGz(_binPanelPath, _runtimeBackupPanel);
        try
        {
            System.Console.WriteLine("Backing Up Plugin Staging...");
            await ArchiveUtilityExt.PackTarGz(_pluginStageLocationRootFolder, _runtimeBackupPlugins);

        }
        catch (System.Exception)
        {
            System.Console.WriteLine("Failed to Back up plugin staging, Restoring Binary.");
            await RestoreBackup();
            Environment.Exit(1); // Failed Restart
            throw;
        }
        System.Console.WriteLine("Back Up Completed, Continue.");


    }
    private static bool ConsumeAccessMark(string file)
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

    private static bool CheckAccessMark(string file, string name)
    {
        bool flag = File.Exists(file);
        if (flag)
        {
            System.Console.WriteLine($"Upgrade Mark Found for {name} ('{file}').");
        }
        else if (!flag)
        {
            System.Console.WriteLine($"No Upgrade Possible, Missing {name} ('{file}').");
        }
        return flag;
    }
    private static string EnsureCreated(string path)
    {

        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        return path;
    }
    private static bool CheckFlagPrint(bool flag, string? positive, string? negative)
    {
        if (flag && !string.IsNullOrWhiteSpace(positive))
        {
            System.Console.WriteLine(positive);
        }
        else if (!flag && !string.IsNullOrWhiteSpace(negative))
        {
            System.Console.WriteLine(negative);
        }
        return flag;
    }
}

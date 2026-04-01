using LunaticPanel.Core.Abstraction;

using static LunaticPanel.Core.Abstraction.IPluginConfiguration;
namespace LunaticPanel.Core;

public interface IPluginUserConfiguration
{
    public string GetUserDownloadBase(string moduleName);
    public string GetUserDownloadBase(string moduleName, string username);

    public string GetUserDownloadFor(string moduleName, string[] subFolders, string filename);
    public string GetUserDownloadFor(string moduleName, string[] subFolders, string filename, string username);

    public string GetUserConfigBase(string moduleName);
    public string GetUserConfigBase(string moduleName, string username);

    public string GetUserConfigBase(string moduleName, string[] subFolders);
    public string GetUserConfigBase(string moduleName, string[] subFolders, string username);

    public string GetUserBashBase(string moduleName);
    public string GetUserBashBase(string moduleName, string username);

    public string GetUserConfigFor(string moduleName, string filename, string username);
    public string GetUserBashFor(string moduleName, string filename, string username);
    public string GetUserBashFor(string moduleName, string[] subFolders, string filename);
    public string GetUserBashFor(string moduleName, string filename, params string[] args);
    public string GetUserBashFor(string moduleName, string filename, string username, params string[] args);
    public string GetUserBashFor(string moduleName, string[] subFolders, string filename, params string[] args);
    public string GetUserBashFor(string moduleName, string[] subFolders, string filename, string username, params string[] args);
}
public class GlobalUserRequiredException : Exception
{

}
internal class PluginConfiguration : IPluginConfiguration, IPluginUserConfiguration
{

    public string PluginFolder { get; }
    public string DotnetAssemblyName { get; }

    public string LinuxAssemblyName { get; }

    public string PluginEtcFolder { get; }

    public string PluginVarFolder { get; }


    private string GlobalUser { get; set; }
    private string BashFolder { get; }
    private string ReposFolder { get; }
    private string DownloadFolder { get; }
    private string ConfigFolder { get; }
    private string UserPluginFolder { get; }
    private string UserConfigFolder { get; }
    private string UserBashFolder { get; }

    public const string BASH_FOLDER_NAME = "bash";
    public const string CONFIG_FOLDER_NAME = "config";
    public const string HOME_FOLDER_NAME = "home";
    public const string REPOS_FOLDER_NAME = "repos";
    public const string DOWNLOAD_FOLDER_NAME = "download";
    public PluginConfiguration(string assemblyName)
    {

        DotnetAssemblyName = assemblyName;
        LinuxAssemblyName = assemblyName.Replace('.', '_').ToLower();
        PluginFolder = EnsureCreated(Path.Combine("/", LinuxUsrFolderName, LinuxLibFolderName, LunaticPanelFolderName, LunaticPanelPluginsFolderName, LinuxAssemblyName));
        PluginEtcFolder = EnsureCreated(Path.Combine("/", LinuxEtcFolderName, LunaticPanelFolderName, LunaticPanelPluginsFolderName, LinuxAssemblyName));
        PluginVarFolder = EnsureCreated(Path.Combine("/", LinuxVarFolderName, LunaticPanelFolderName, LunaticPanelPluginsFolderName, LinuxAssemblyName));


        BashFolder = Path.Combine(PluginFolder, BASH_FOLDER_NAME);
        ConfigFolder = Path.Combine(PluginEtcFolder, CONFIG_FOLDER_NAME);
        ReposFolder = Path.Combine(PluginEtcFolder, REPOS_FOLDER_NAME);
        DownloadFolder = Path.Combine(UserPluginFolder, DOWNLOAD_FOLDER_NAME);


        UserPluginFolder = Path.Combine(UserFolder, LunaticPanelFolderName, LunaticPanelPluginsFolderName, LinuxAssemblyName);
        UserConfigFolder = Path.Combine(UserPluginFolder, CONFIG_FOLDER_NAME);
        UserBashFolder = Path.Combine(UserPluginFolder, BASH_FOLDER_NAME);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
    public string EnsureCreated(string path)
    {
        var dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Console.WriteLine($"Created (755): {dir}");
            Directory.CreateDirectory(dir!,
                UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute |
                UnixFileMode.GroupRead | UnixFileMode.GroupExecute |
                UnixFileMode.OtherRead | UnixFileMode.OtherExecute);
        }
        return path;
    }




    public string GetReposBase(string moduleName)
        => EnsureCreated(Path.Combine(ReposFolder, moduleName.ToLower()));

    public string GetReposFor(string moduleName, string repos)
        => EnsureCreated(Path.Combine(GetReposBase(moduleName), repos.ToLower()));
    public string GetReposFor(string moduleName, string[] subFolders, string repos)
        => GetReposFor(Path.Combine([moduleName, .. subFolders]), repos);

    public string GetConfigBase(string moduleName)
        => EnsureCreated(Path.Combine(ConfigFolder, moduleName.ToLower()));
    public string GetConfigBase(string moduleName, string[] subFolders)
        => EnsureCreated(Path.Combine(ConfigFolder, Path.Combine([moduleName.ToLower(), .. subFolders])));

    public string GetBashBase(string moduleName)
        => EnsureCreated(Path.Combine(BashFolder, moduleName.ToLower()));


    public string GetBashFor(string moduleName, string filename)
        => Path.Combine(GetBashBase(moduleName), filename);

    public string GetBashFor(string moduleName, string filename, params string[] args)
        => GetBashFor(moduleName, filename + " " + _stringifyArguments(args));

    public string GetBashFor(string moduleName, string[] subFolders, string filename, params string[] args)
        => GetBashFor(Path.Combine([moduleName, .. subFolders]), filename, args);


    private void RequiresGlobalUser()
    {
        if (string.IsNullOrWhiteSpace(GlobalUser))
            throw new GlobalUserRequiredException();
    }

    public string GetUserDownloadFor(string moduleName, string[] subFolders, string filename)
    {
        RequiresGlobalUser();
        return Path.Combine(GetUserDownloadBase(Path.Combine([moduleName, .. subFolders])), filename);
    }

    public string GetUserDownloadFor(string moduleName, string[] subFolders)
    {
        RequiresGlobalUser();
        return GetUserDownloadBase(Path.Combine([moduleName, .. subFolders]), GlobalUser);
    }
    public string GetUserDownloadBase(string moduleName)
    {
        RequiresGlobalUser();
        return GetUserDownloadBase(moduleName, GlobalUser);
    }

    public string GetUserDownloadBase(string moduleName, string username)
        => EnsureCreated(Path.Combine(DownloadFolder, moduleName.ToLower()));
    public string GetUserDownloadFor(string moduleName, string[] subFolders, string filename, string username)
        => throw new NotImplementedException();


    public string GetUserConfigBase(string moduleName, string? username = default)
        => EnsureCreated(Path.Combine(UserConfigFolder, moduleName.ToLower()));

    public string GetUserConfigBase(string moduleName, string[] subFolders, string? username = default)
    => EnsureCreated(Path.Combine(UserConfigFolder, Path.Combine([moduleName.ToLower(), .. subFolders])));

    public string GetUserBashBase(string moduleName, string? username = default)
        => EnsureCreated(Path.Combine(UserBashFolder, moduleName.ToLower()));

    public string GetConfigFor(string moduleName, string filename)
        => Path.Combine(GetConfigBase(moduleName), filename);

    public string GetUserConfigFor(string moduleName, string filename, string? username = default)
        => Path.Combine(GetUserConfigBase(moduleName), filename);

    public string GetUserConfigFor(string moduleName, string[] subFolders, string filename, string? username = default)
    => GetUserConfigFor(Path.Combine([moduleName, .. subFolders]), filename);
    public string GetUserBashFor(string moduleName, string filename, string? username = default)
        => Path.Combine(GetUserBashBase(moduleName), filename);
    public string GetUserBashFor(string moduleName, string[] subFolders, string filename, string? username = default)
        => Path.Combine(GetUserBashBase(Path.Combine([moduleName, .. subFolders])), filename);

    private Func<string[], string> _stringifyArguments = (args) => string.Join(' ', args.Select(p => $"\\\"{p}\\\""));
    public string GetUserBashFor(string moduleName, string filename, params string[] args)
        => GetUserBashFor(moduleName, filename + " " + _stringifyArguments(args));
    public string GetUserBashFor(string moduleName, string[] subFolders, string filename, params string[] args)
        => GetUserBashFor(Path.Combine([moduleName, .. subFolders]), filename, args);

    public string GetUserConfigBase(string moduleName) => throw new NotImplementedException();
    public string GetUserConfigBase(string moduleName, string[] subFolders) => throw new NotImplementedException();
    public string GetUserBashBase(string moduleName) => throw new NotImplementedException();
    public string GetUserBashFor(string moduleName, string[] subFolders, string filename) => throw new NotImplementedException();
    public string GetUserBashFor(string moduleName, string filename, string username, params string[] args) => throw new NotImplementedException();
    public string GetUserBashFor(string moduleName, string[] subFolders, string filename, string username, params string[] args) => throw new NotImplementedException();
}
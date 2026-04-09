namespace LunaticPanel.Core.Utils.Abstraction.LinuxCommand.Helper.Common.Packages;

public static class LinuxPackageCommandExt
{
    const string CHECK_DEP_INSTALLED = "dpkg -s {0} >/dev/null 2>&1 && echo true || echo false";
    const string INSTALL_DEP = "apt-get install -y {0} >/dev/null 2>&1";

    public static async Task<bool> InstalledAsync(this LinuxPackageRegionContext context, string name, Action<LinuxCommandBuilderContext>? options = default, CancellationToken ct = default)
        => await context.LinuxCommand.GetAsync<bool>(string.Format(CHECK_DEP_INSTALLED, name), options, ct);

    public static async Task<bool> InstalledOrDefaultAsync(this LinuxPackageRegionContext context, string name, bool defaultValue, Action<LinuxCommandBuilderContext>? options = default, CancellationToken ct = default)
        => await context.LinuxCommand.GetOrDefaultAsync<bool>(string.Format(CHECK_DEP_INSTALLED, name), defaultValue, options, ct);

    public static async Task<bool> InstalledAsync(this LinuxPackageRegionContext context, string[] names, Action<LinuxCommandBuilderContext>? options = default, CancellationToken ct = default)
    => await context.LinuxCommand.GetAsync<bool>(string.Format(CHECK_DEP_INSTALLED, string.Join(' ', names)), options, ct);

    public static async Task<bool> InstalledOrDefaultAsync(this LinuxPackageRegionContext context, string[] names, bool defaultValue, Action<LinuxCommandBuilderContext>? options = default, CancellationToken ct = default)
        => await context.LinuxCommand.GetOrDefaultAsync<bool>(string.Format(CHECK_DEP_INSTALLED, string.Join(' ', names)), defaultValue, options, ct);

    public static async Task<bool> InstallAsync(this LinuxPackageRegionContext context, string name, Action<LinuxCommandBuilderContext>? options = default, CancellationToken ct = default)
        => await context.LinuxCommand.GetAsync<bool>(string.Format(string.Join("&&", [INSTALL_DEP, CHECK_DEP_INSTALLED]), name), options, ct);

    public static async Task<bool> InstallOrDefaultAsync(this LinuxPackageRegionContext context, string name, bool defaultValue, Action<LinuxCommandBuilderContext>? options = default, CancellationToken ct = default)
        => await context.LinuxCommand.GetOrDefaultAsync<bool>(string.Format(string.Join("&&", [INSTALL_DEP, CHECK_DEP_INSTALLED]), name), defaultValue, options, ct);

    public static async Task<bool> InstallAsync(this LinuxPackageRegionContext context, string[] names, Action<LinuxCommandBuilderContext>? options = default, CancellationToken ct = default)
    => await context.LinuxCommand.GetAsync<bool>(string.Format(string.Join("&&", [INSTALL_DEP, CHECK_DEP_INSTALLED]), string.Join(' ', names)), options, ct);

    public static async Task<bool> InstallOrDefaultAsync(this LinuxPackageRegionContext context, string[] names, bool defaultValue, Action<LinuxCommandBuilderContext>? options = default, CancellationToken ct = default)
        => await context.LinuxCommand.GetOrDefaultAsync<bool>(string.Format(string.Join("&&", [INSTALL_DEP, CHECK_DEP_INSTALLED]), string.Join(' ', names)), defaultValue, options, ct);
}

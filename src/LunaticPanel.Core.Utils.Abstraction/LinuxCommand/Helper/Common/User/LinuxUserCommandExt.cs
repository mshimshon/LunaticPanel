using LunaticPanel.Core.Utils.Abstraction.LinuxCommand.Helper.Common;

namespace LunaticPanel.Core.Utils.Abstraction.LinuxCommand.Extensions.Common.User;
/*
 * ILinuxCommand.Common.User.ExistAsync(username, o=> o.Sudo().RunAs(user));
 * 
 * 
 */
public static class LinuxUserCommandExt
{
    const string CHECK_USER_EXIST = "id -u {0} >/dev/null 2>&1 && echo true || echo false";
    public static async Task<bool> ExistAsync(this LinuxUserRegionContext context, string username, Action<LinuxCommandBuilderContext>? options = default, CancellationToken ct = default)
        => await context.LinuxCommand.GetAsync<bool>(string.Format(CHECK_USER_EXIST, username), options, ct);

    public static async Task<bool> ExistOrDefaultAsync(this LinuxUserRegionContext context, string username, bool defaultValue, Action<LinuxCommandBuilderContext>? options = default, CancellationToken ct = default)
        => await context.LinuxCommand.GetOrDefaultAsync(string.Format(CHECK_USER_EXIST, username), defaultValue, options, ct);


}

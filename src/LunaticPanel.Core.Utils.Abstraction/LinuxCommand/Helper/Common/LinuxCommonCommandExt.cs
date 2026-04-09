using LunaticPanel.Core.Utils.Abstraction.LinuxCommand.Helper.Common.Packages;

namespace LunaticPanel.Core.Utils.Abstraction.LinuxCommand.Extensions;

public static class LinuxCommonUserCommandExt
{
    public static LinuxCommonRegionContext Common(this ILinuxCommand linuxCommand)
        => LinuxRegionContext.Create<LinuxCommonRegionContext>(linuxCommand);

    public static LinuxUserRegionContext User(this LinuxCommonRegionContext context)
        => context.CreateFrom<LinuxUserRegionContext>(context);

    public static LinuxPackageRegionContext Package(this LinuxCommonRegionContext context)
    => context.CreateFrom<LinuxPackageRegionContext>(context);
}

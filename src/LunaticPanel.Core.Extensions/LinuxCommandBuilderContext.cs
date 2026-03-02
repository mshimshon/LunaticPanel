using LunaticPanel.Core.Abstraction.Tools.LinuxCommand;

namespace LunaticPanel.Core.Extensions;


public sealed class LinuxCommandBuilderContext
{
    public LinuxCommandBuilder CommandBuilder { get; set; } = default!;
    public ILinuxCommand LinuxCommand { get; init; } = default!;
    public LinuxCommandBuilderContext UpdateBuilder(Func<LinuxCommandBuilder, LinuxCommandBuilder> update)
    {
        CommandBuilder = update(CommandBuilder);
        return this;
    }
}
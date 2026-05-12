namespace LunaticPanel.Core.Utils.Abstraction.LinuxCommand;


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
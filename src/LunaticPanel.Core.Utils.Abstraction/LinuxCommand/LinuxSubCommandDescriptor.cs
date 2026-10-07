using LunaticPanel.Core.Utils.Abstraction.LinuxCommand.Enums;

namespace LunaticPanel.Core.Utils.Abstraction.LinuxCommand;

public sealed record LinuxSubCommandDescriptor(string Command, CommandOperand Operand)
{
    public override string ToString()
    {
        var operand = Operand switch
        {
            CommandOperand.Or => "||",
            CommandOperand.PipeIn => "|",
            CommandOperand.PipeAll => "|&",
            _ => "&&"
        };
        return $"{operand} {Command}";
    }
}

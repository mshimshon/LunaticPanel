namespace LuncaticPanel.Package.Server.Application.Exceptions;

public class MediatorCommandNotFoundException : ApplicationException
{
    public MediatorCommandNotFoundException() :
        base(nameof(MediatorCommandNotFoundException), "Requested command was not found.")
    {
    }
}

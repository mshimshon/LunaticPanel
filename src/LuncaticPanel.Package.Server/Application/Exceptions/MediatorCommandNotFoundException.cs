namespace LuncaticPanel.Package.Server.Application.Exceptions;

public class MediatorCommandNotFoundException : AppLayerException
{
    public MediatorCommandNotFoundException() :
        base(nameof(MediatorCommandNotFoundException), "Requested command was not found.")
    {
    }
}

namespace LuncaticPanel.Package.Server.Application.Exceptions;

public class AppLayerException : Exception
{
    public AppLayerException(string code, string message) : base(message)
    {
        Code = code;
    }

    public string Code { get; }
}

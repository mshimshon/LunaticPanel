namespace LuncaticPanel.Package.Server.Application.Exceptions;

public class ApplicationException : Exception
{
    public ApplicationException(string code, string message) : base(message)
    {
        Code = code;
    }

    public string Code { get; }
}

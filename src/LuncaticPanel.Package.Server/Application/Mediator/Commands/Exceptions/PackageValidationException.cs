namespace LuncaticPanel.Package.Server.Application.Mediator.Commands.Exceptions;

public sealed class PackageValidationException
{
    public PackageValidationException(string code, string message, object? validationResult) : base(code, message)
    {
        ValidationResult = validationResult;
    }

    public object? ValidationResult { get; }
}

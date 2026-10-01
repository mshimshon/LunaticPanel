using LuncaticPanel.Package.Server.Application.Exceptions;

namespace LuncaticPanel.Package.Server.Application.Mediator.Commands.Exceptions;

public sealed class PackageValidationException : AppLayerException
{

    public PackageValidationException(string code, string message, object? validationResult) : base(code, message)
    {
        ValidationResult = validationResult;
    }

    public object? ValidationResult { get; }
}

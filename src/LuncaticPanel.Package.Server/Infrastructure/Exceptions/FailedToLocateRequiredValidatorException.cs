namespace LuncaticPanel.Package.Server.Infrastructure.Exceptions;

public sealed class FailedToLocateRequiredValidatorException : InfrastructureCodedException
{
    public FailedToLocateRequiredValidatorException(string panelVersion) :
        base(nameof(FailedToLocateRequiredValidatorException), $"No viable tool location to validate v{panelVersion}.")
    {
    }
}

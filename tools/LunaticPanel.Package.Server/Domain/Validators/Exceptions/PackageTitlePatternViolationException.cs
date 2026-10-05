using LunaticPanel.Package.Server.Domain.Exceptions;

namespace LunaticPanel.Package.Server.Domain.Validators.Exceptions;

public sealed class PackageTitlePatternViolationException : DomainCodedException
{
    public PackageTitlePatternViolationException(string title) :
        base(nameof(PackageTitlePatternViolationException), $"'{title}' package title must only contain a-Z 0-9.")
    {
    }
}

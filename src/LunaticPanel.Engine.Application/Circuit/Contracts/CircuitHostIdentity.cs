using LunaticPanel.Core.Abstraction.Circuit;

namespace LunaticPanel.Engine.Application.Circuit.Contracts;

public sealed record CircuitHostIdentity : CircuitIdentity
{
    public object? LayoutComponent { get; set; }
    public bool Master { get; set; }
}

using LunaticPanel.Core.Abstraction.Circuit;

namespace LunaticPanel.Engine.Models.Circuit;

public sealed record CircuitHostIdentityModel : CircuitIdentity
{
    public object? LayoutComponent { get; set; }
}

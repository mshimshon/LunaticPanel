namespace LunaticPanel.PackageManager.Application.Mediator.Queries;

public sealed record SearchPackageQuery
{
    public string Keywords { get; set; } = string.Empty;
}

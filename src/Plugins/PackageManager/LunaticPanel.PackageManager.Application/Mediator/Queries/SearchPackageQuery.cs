namespace LunaticPanel.PackageManager.Application.Mediator.Queries;

internal sealed record SearchPackageQuery
{
    public string Keywords { get; set; } = string.Empty;
}

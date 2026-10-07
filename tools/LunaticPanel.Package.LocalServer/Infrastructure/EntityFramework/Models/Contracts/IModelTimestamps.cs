namespace LunaticPanel.Package.LocalServer.Infrastructure.EntityFramework.Models.Contracts;

public interface IModelTimestamps
{
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
}

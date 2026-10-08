using LunaticPanel.Package.LocalServer.Infrastructure;
using LunaticPanel.Package.LocalServer.Infrastructure.Extensions;
using LunaticPanel.Package.LocalServer.Infrastructure.Services.ValidationQueueSystem;

namespace LunaticPanel.Package.LocalServer.Web;

public static class RegisterServiceExt
{
    public static void AddLocalServerServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddLocalServerInfrastructure(configuration);
    }
    public static void AddLocalServerAPIEndpoints(this WebApplication app)
    {

        app.MapPost("/v1/upload/{uploadId}", async (string uploadId, HttpRequest request, IValidationQueueHandlerService validationQueue) =>
        {
            await validationQueue.ValidateUploadIdAsync(uploadId);
            string pathBase = "/var/lib/lunaticpanel_lpkg_localserver/lpkgs";

            var serveLocation = Path.Combine(pathBase, "available");
            var uploadLocation = Path.Combine(pathBase, "awaiting");
            if (!request.HasFormContentType)
                return Results.BadRequest("Form content expected");

            var form = await request.ReadFormAsync();
            var file = form.Files.FirstOrDefault();

            if (file == null || file.Length == 0)
                return Results.BadRequest("No file");

            var path = Path.Combine(uploadLocation, $"{file.FileName.ToBase32()}.lpkg");
            using var stream = File.Create(path);
            await file.CopyToAsync(stream);
            await validationQueue.AssociateUploadIdToFileAsync(uploadId, path);

            return Results.Ok();
        });
    }

}

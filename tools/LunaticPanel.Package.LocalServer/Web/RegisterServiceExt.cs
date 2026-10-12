using LunaticPanel.Package.LocalServer.Infrastructure;
using LunaticPanel.Package.LocalServer.Infrastructure.Exceptions;
using LunaticPanel.Package.LocalServer.Infrastructure.Extensions;
using LunaticPanel.Package.LocalServer.Infrastructure.Services.ValidationQueueSystem;
using LunaticPanel.Package.Server;
using LunaticPanel.Package.Server.Web.Payloads.Enums;
using Microsoft.AspNetCore.Mvc;

namespace LunaticPanel.Package.LocalServer.Web;

public static class RegisterServiceExt
{

    public static void AddLocalServerServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddLocalServerInfrastructure(configuration);
    }
    public static async Task UseLocalServerAPIEndpoints(this WebApplication app)
    {
        app.UseLunaPackage();
        await app.UseLocalServerInfrastructure();
        app.EnableLunaPackageCodedError();
        app.UseLunaPackageCodedErrorFor<InfrastructureException>(p => new(p.Code, p.Message, ExceptionProvenencePayload.Infrastructure));

        app.MapPost("/v1/upload/{uploadId:regex(^[a-zA-Z0-9-]+$)}", async ([FromRoute] string uploadId, HttpRequest request, [FromServices] IValidationQueueHandlerService validationQueue) =>
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

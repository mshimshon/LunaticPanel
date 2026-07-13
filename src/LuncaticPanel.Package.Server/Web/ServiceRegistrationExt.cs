using LuncaticPanel.Package.Server.Application.Exceptions;
using LuncaticPanel.Package.Server.Application.Mediator.Commands;
using LuncaticPanel.Package.Server.Application.Mediator.Engine;
using LuncaticPanel.Package.Server.Application.Mediator.Queries;
using LuncaticPanel.Package.Server.Application.Payloads;
using LuncaticPanel.Package.Server.Application.Payloads.Requests;
using LuncaticPanel.Package.Server.Domain.Exceptions;
using LuncaticPanel.Package.Server.Infrastructure;
using LuncaticPanel.Package.Server.Web.Payloads;
using LuncaticPanel.Package.Server.Web.Payloads.Enums;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Net;

namespace LuncaticPanel.Package.Server.Web;

public static class ServiceRegistrationExt
{
    internal static void AddWebLayerServices(this WebApplicationBuilder app)
    {
        app.Services.AddInfrastructureLayerServices();
    }

    internal static HashSet<string> APIs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);


    private static void AddEndpointsVersion1(this WebApplication app)
    {

        var version = "v1";
        if (APIs.Contains(version)) return;
        APIs.Add(version);
        var v1Group = app.MapGroup($"/lpkg/{version}");
        v1Group.MapPost("/package/push", (ManifestPayload data, IMediator mediator)
            => mediator.ExecuteAsync(new CreateManifestCommand(data)));

        v1Group.MapPost("/package/validate", (ManifestPayload data, IMediator mediator)
            => mediator.ExecuteAsync(new CreateManifestCommand(data)));

        v1Group.MapPost("/package/hide/{id}/{version}", (string id, string version, IMediator mediator)
            => mediator.ExecuteAsync(new HideManifestVersionCommand(id, version)));
        v1Group.MapPost("/package/endlife", (EndOfLifeRequest data, IMediator mediator)
            => mediator.ExecuteAsync(new EndManifestLifeCommand(data)));
        v1Group.MapPost("/package/search", (ManifestSearchRequest data, IMediator mediator)
            => mediator.ExecuteAsync(new SearchManifestQuery(data)));
        v1Group.MapGet("/package/info/{id}", (string id, IMediator mediator) =>
            mediator.ExecuteAsync(new GetAllPackageVersionsQuery(id)));
        v1Group.MapGet("/package/info/{id}/{version}", (string id, string version, IMediator mediator)
            => mediator.ExecuteAsync(new GetSpecificPackageVersionQuery(id, version)));
    }
    internal static void AddEndpoints(this WebApplication app)
    {
        app.AddEndpointsVersion1();
        app.MapGet("/lpkg/versions", () => APIs);
    }


    public static Dictionary<Type, Func<object, CodedException>> _customException = new();
    internal static void UseExceptionHandlerFor<TException>(this WebApplication app, Func<TException, CodedException> onErrorFound)
    {
        Type exceptionType = typeof(TException);
        if (_customException.ContainsKey(exceptionType))
            return;

        _customException[exceptionType] = (e) =>
        {
            return onErrorFound.Invoke((TException)e);
        };

    }
    private static bool _exceptionHandlingEnable = false;
    private static void HandleSelfExceptions(this WebApplication app)
    {
        app.UseExceptionHandlerFor<DomainCodedException>(a => new CodedException(a.Code, a.Message, ExceptionProvenencePayload.Domain));
        app.UseExceptionHandlerFor<AppLayerException>(a => new CodedException(a.Code, a.Message, ExceptionProvenencePayload.Application));
    }

    private static CodedException GenerateCodedException(Exception ex)
    {
        var exType = ex.GetType();
        if (!_customException.ContainsKey(exType))
            return new CodedException("UnknownException", "Internal system unknown exception occured.", ExceptionProvenencePayload.Unknown);
        return _customException[exType].Invoke(exType);

    }
    internal static void UseLayerExceptionHandler(this WebApplication app)
    {
        if (_exceptionHandlingEnable) return;
        _exceptionHandlingEnable = true;

        app.HandleSelfExceptions();
        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                var feature = context.Features.Get<IExceptionHandlerFeature>();
                var ex = feature?.Error;
                if (ex == null)
                    return;
                var error = GenerateCodedException(ex);
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(error);
            });
        });
    }
}

using LunaticPanel.Package.Server.Web;
using LunaticPanel.Package.Server.Web.Payloads;
using Microsoft.AspNetCore.Builder;

namespace LunaticPanel.Package.Server;

public static class ServiceRegistrationExt
{
    public static void AddLunaPackage(this WebApplicationBuilder app)
    {
        app.AddWebLayerServices();
    }
    public static void UseLunaPackage(this WebApplication app)
    {
        app.AddEndpoints();
    }

    public static void UseLunaPackageCodedErrorFor<TException>(this WebApplication app, Func<TException, CodedExceptionPayload> onErrorFound)
        where TException : Exception
    {
        app.EnableLunaPackageCodedError();
        app.UseExceptionHandlerFor(onErrorFound);
    }


    public static void EnableLunaPackageCodedError(this WebApplication app)
    {
        app.UseLayerExceptionHandler();
    }

}

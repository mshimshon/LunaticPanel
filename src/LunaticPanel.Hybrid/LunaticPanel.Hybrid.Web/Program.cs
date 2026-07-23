using LunaticPanel.Core.Abstraction.Plugin;
using LunaticPanel.Engine.Web;
using LunaticPanel.Engine.Web.Boostrap;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.ComponentModel;
using System.Diagnostics;
using System.Security.Claims;
using App = LunaticPanel.Hybrid.Web.App;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorComponents().AddInteractiveServerComponents().AddCircuitOptions(o => o.DetailedErrors = true);
builder.WebHost.UseKestrel();

builder
            .AddSharedType<IPlugin>()
            // ==========================================
            // 2. MudBlazor & UI Interop Libraries
            // ==========================================
            .AddSharedType<MudBlazor.MudButton>()                                // Blocks MudBlazor.dll
            .AddSharedType<HtmlRenderer>()   // Blocks Microsoft.AspNetCore.Components.Web.dll
            .AddSharedType<ComponentBase>()      // Blocks Microsoft.AspNetCore.Components.dll
            .AddSharedType<Microsoft.JSInterop.IJSRuntime>()                     // Blocks Microsoft.JSInterop.dll

            // ==========================================
            // 3. ASP.NET Core Framework Foundations
            // ==========================================
            .AddSharedType<WebApplication>()        // Blocks Microsoft.AspNetCore.dll
            .AddSharedType<HttpContext>()              // Blocks Microsoft.AspNetCore.Http.Abstractions.dll
            .AddSharedType<IServiceCollection>() // Blocks Microsoft.Extensions.DependencyInjection.Abstractions.dll
            .AddSharedType<ILogger>()                // Blocks Microsoft.Extensions.Logging.Abstractions.dll
            .AddSharedType<IConfiguration>()  // Blocks Microsoft.Extensions.Configuration.Abstractions.dll

            // ==========================================
            // 4. Vulnerable "System" Extended Utilities
            // ==========================================
            .AddSharedType<HttpClient>()                         // Blocks System.Net.Http.dll
            .AddSharedType<DiagnosticSource>()                 // Blocks System.Diagnostics.DiagnosticSource.dll
            .AddSharedType<ClaimsPrincipal>()             // Blocks System.Security.Claims.dll
            .AddSharedType<Component>();
Bootstrap.BootstrapBuilder(builder.Services, builder.Configuration);

WebApplication app = builder.Build();

app.UseStaticFiles();
app.MapStaticAssets();
await Bootstrap.BootstrapRunAsync(app, app.Services, app.Configuration);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
    app.UseHttpsRedirection();
}
app.UseAntiforgery();

app.MapRazorComponents<App>()
.AddInteractiveServerRenderMode()
.AddAdditionalAssemblies([.. Bootstrap.AdditionalAssemblies]);
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);





app.Lifetime.ApplicationStarted.Register(() =>
{
    var sources = app.Services.GetServices<EndpointDataSource>();

    foreach (var source in sources)
    {
        Console.WriteLine($"SOURCE: {source.GetType().FullName}");

        foreach (var endpoint in source.Endpoints)
        {
            //Console.WriteLine($"  ENDPOINT TYPE : {endpoint.GetType().FullName}");
            //Console.WriteLine($"  DISPLAY NAME  : {endpoint.DisplayName}");
            bool isComponentRoute = endpoint.Metadata
            .Any(p => string.Equals(p.GetType().FullName, "Microsoft.AspNetCore.Components.Endpoints.ComponentTypeMetadata", StringComparison.OrdinalIgnoreCase));
            if (!isComponentRoute) continue;
            if (endpoint is RouteEndpoint routeEndpoint)
            {
                Console.WriteLine($"  ROUTE         : {routeEndpoint.RoutePattern.RawText}");
            }

            Console.WriteLine();
        }
    }
});


await app.RunAsync();

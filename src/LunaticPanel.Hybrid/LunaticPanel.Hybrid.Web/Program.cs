using LunaticPanel.Engine.Web.Boostrap;
using App = LunaticPanel.Hybrid.Web.App;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorComponents().AddInteractiveServerComponents().AddCircuitOptions(o => o.DetailedErrors = true);
builder.WebHost.UseKestrel();

Bootstrap.BootstrapBuilder(builder.Services, builder.Configuration);
WebApplication app = builder.Build();

app.UseStaticFiles();
app.MapStaticAssets();
await Bootstrap.BootstrapRunAsync(app, app.Services, app.Configuration);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.MapRazorComponents<App>()
.AddInteractiveServerRenderMode()
.AddAdditionalAssemblies([.. Bootstrap.AdditionalAssemblies]);
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();




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

using LunaticPanel.Engine;
using LunaticPanel.Engine.Boostrap;

var app = Bootstrap.Load(() =>
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
    return builder;
});

await Bootstrap.RunAsync(() =>
{
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error", createScopeForErrors: true);
        app.UseHsts();
    }

    app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
    app.UseHttpsRedirection();

    app.UseAntiforgery();

    app.UseStaticFiles();
    app.MapStaticAssets();

    app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies([.. Bootstrap.AdditionalAssemblies]);
    return app;
});

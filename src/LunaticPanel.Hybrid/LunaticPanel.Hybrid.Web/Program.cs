using LunaticPanel.Engine.Web.Boostrap;
using App = LunaticPanel.Hybrid.Web.App;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorComponents().AddInteractiveServerComponents().AddCircuitOptions(o => o.DetailedErrors = true);
builder.WebHost.UseKestrel();
Bootstrap.BootstrapBuilder(builder.Services, builder.Configuration);
WebApplication app = builder.Build();

app.UseStaticFiles();
app.MapStaticAssets();
Bootstrap.BootstrapRun(app, app.Services, app.Configuration);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();



app.MapRazorComponents<App>()
.AddInteractiveServerRenderMode()
.AddAdditionalAssemblies([.. Bootstrap.AdditionalAssemblies]);

await app.RunAsync();

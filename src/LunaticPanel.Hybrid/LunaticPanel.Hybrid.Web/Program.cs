using LunaticPanel.Engine.Presentation;
using LunaticPanel.Engine.Presentation.Boostrap;
using App = LunaticPanel.Hybrid.Web.App;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.WebHost.UseKestrel();
Bootstrap.BootstrapBuilder(builder.Services, builder.Configuration);
WebApplication app = builder.Build();
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
.AddAdditionalAssemblies([.. Bootstrap.AdditionalAssemblies, typeof(RegisterServicesExt).Assembly]);

Bootstrap.BootstrapRun(app.Services, app.Configuration);
await app.RunAsync();

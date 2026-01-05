using LunaticPanel.Engine.Presentation.Boostrap;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace LunaticPanel.Hybrid;

public static class MauiProgram
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0007:Use implicit type", Justification = "<Pending>")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
    public static MauiApp CreateMauiApp()
    {
        MauiAppBuilder? builder = MauiApp.CreateBuilder();
        try
        {
            using var stream = FileSystem.OpenAppPackageFileAsync("appsettings.json")
                .GetAwaiter()
                .GetResult();
            builder.Configuration.AddJsonStream(stream);

        }
        catch (Exception)
        {
            // Optional if no file or file issue just use default.
        }

        Bootstrap.BootstrapBuilder(builder.Services, builder.Configuration);
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif
        MauiApp app = builder.Build();
        Bootstrap.BootstrapRun(app.Services, app.Configuration);
        return app;
    }
}

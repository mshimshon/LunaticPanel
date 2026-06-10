using LunaticPanel.Core.Abstraction.Widgets.Exceptions;
using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.Engine.Web.Services.Circuit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;

namespace LunaticPanel.Engine.Web.ExceptionHandlers;

public class GlobalErrorCodedBoundary : ErrorBoundary
{
    [Inject] ISnackbar Snackbar { get; set; } = default!;
    [Inject] CircuitRegistry CircuitRegistry { get; set; } = default!;
    [Inject] ICrazyReport<GlobalErrorCodedBoundary> CrazyReport { get; set; } = default!;
    protected override Task OnErrorAsync(Exception exception)
    {
        if (exception is HostCodedException coded)
        {
            Snackbar.Add(coded.Message, Severity.Error);
            CrazyReport.ReportErrorException(coded.Message, coded);
            Recover();
            return Task.CompletedTask;
        }

        return base.OnErrorAsync(exception);
    }
}
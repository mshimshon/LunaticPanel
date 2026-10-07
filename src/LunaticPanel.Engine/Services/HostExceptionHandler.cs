using LunaticPanel.Core.Abstraction.Exceptions;
using LunaticPanel.Core.Utils.Abstraction.Logging;
using MudBlazor;

namespace LunaticPanel.Engine.Web.Services;

internal class HostExceptionHandler : IHostExceptionHandler
{
    private readonly ICrazyReport? _crazyReport;
    private ISnackbar? _snackbar;
    public HostExceptionHandler(IServiceProvider serviceProvider, ICrazyReport<HostExceptionHandler> crazyReport)
    {
        _snackbar = serviceProvider.GetService<ISnackbar>();
        _crazyReport = crazyReport;
    }
    public void Throw(Exception ex)
    {
        _crazyReport.SafeReportErrorException(ex.Message, ex);
        Throw(new HostCodedException("Unknown", "Unknown Internal Error Occurs."));
    }
    public void Throw(HostCodedException ex)
    {
        _crazyReport.SafeReportErrorException(ex.Message, ex);
        _snackbar?.Add(ex.Message, Severity.Error);
    }
    public Task ThrowAsync(Exception ex, CancellationToken ct = default)
    {
        Throw(ex);

        return Task.CompletedTask;
    }
    public Task ThrowAsync(HostCodedException ex, CancellationToken ct = default)
    {
        Throw(ex);
        return Task.CompletedTask;
    }
}

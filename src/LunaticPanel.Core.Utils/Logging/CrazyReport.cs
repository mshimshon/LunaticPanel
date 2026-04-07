using LunaticPanel.Core.Utils.Abstraction.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using static LunaticPanel.Core.Utils.Abstraction.Logging.ICrazyReport;
namespace LunaticPanel.Core.Utils.Logging;

internal class CrazyReport<TClass> : CrazyReport, ICrazyReport<TClass> where TClass : class
{
    public CrazyReport(ICrazyReportCircuit crazyReportCircuit, IServiceProvider serviceProvider) : base(crazyReportCircuit, serviceProvider)
    {
        SetClass<TClass>();
    }
}

internal class CrazyReport : ICrazyReport
{
    private string? _moduleName = "[System]";
    private string? _className = "[Object]";
    private readonly IServiceProvider _serviceProvider;
    private ILogger? _logger;
    private Guid _circuitId;

    public CrazyReport(ICrazyReportCircuit crazyReportCircuit, IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _circuitId = crazyReportCircuit.CircuitId;
    }


    public void Report(string line)
    {
        Console.WriteLine(line);
        Log(line, LogLevel.Trace);
    }
    public void Report(string format, params object[] arg)
    {
        var line = string.Format(format, arg);
        Console.WriteLine(line, arg);
        Log(line, LogLevel.Trace);
    }
    public void ReportError(string line)
    {
        Console.WriteLine($"{Red}{GetPrefix()}: {line}{Reset}");
        Log(line, LogLevel.Error);
    }
    public void ReportError(string format, params object[] arg)
    {
        var line = string.Format(format, arg);
        Console.WriteLine($"{Red}{GetPrefix()}: {line}{Reset}");
        Log(line, LogLevel.Error);
    }

    public void ReportInfo(string line)
    {
        Console.WriteLine($"{DarkCyan}{GetPrefix()}: {line}{Reset}");
        Log(line, LogLevel.Information);
    }

    public void ReportInfo(string format, params object[] arg)
    {
        var line = string.Format(format, arg);
        Console.WriteLine($"{DarkCyan}{GetPrefix()}: {line}{Reset}");
        Log(line, LogLevel.Information);
    }

    public void ReportSuccess(string line)
    {
        Console.WriteLine($"{DarkGreen}{GetPrefix()}: {line}{Reset}");
        Log(line, LogLevel.Debug);
    }

    public void ReportSuccess(string format, params object[] arg)
    {
        var line = string.Format(format, arg);
        Console.WriteLine($"{DarkGreen}{GetPrefix()}: {line}{Reset}");
        Log(line, LogLevel.Debug);
    }

    public void ReportWarning(string line)
    {
        Console.WriteLine($"{DarkYellow}{GetPrefix()}: {line}{Reset}");
        Log(line, LogLevel.Warning);

    }
    public void ReportWarning(string format, params object[] arg)
    {
        var line = string.Format(format, arg);
        Console.WriteLine($"{DarkYellow}{GetPrefix()}: {line}{Reset}", arg);
        Log(line, LogLevel.Warning);
    }
    private string GetPrefix()
    {
        List<string> prefixes = new() {
            $"[{DateTime.Now.ToString()}]"
        };
        if (_circuitId != Guid.Empty)
            prefixes.Add($"[{_circuitId}]");
        if (!string.IsNullOrWhiteSpace(_moduleName))
            prefixes.Add($"[{_moduleName}]");
        if (!string.IsNullOrWhiteSpace(_className))
            prefixes.Add($"[{_className}]");
        return string.Join("::", prefixes);
    }

    public void SetModule(string moduleName) => _moduleName = moduleName;
    public void SetModule<TClass>(string moduleName) where TClass : class
    {
        SetModule(moduleName);
        SetClass<TClass>();
    }

    public void SetClass<TClass>()
        where TClass : class
    {
        _className = typeof(TClass).Name;
        _logger = _serviceProvider.GetService<ILogger<TClass>>();
    }
    private void HandleException(Exception ex, LogLevel logLevel, string line, Action<string> toReport)
    {
        toReport(line);
        Log(ex, logLevel);
    }
    private void HandleException(Exception ex, LogLevel logLevel, string format, object?[]? arg, Action<string, object?[]?> toReport)
    {
        toReport(format, arg);
        Log(ex, logLevel);
    }

    private void Log(string line, LogLevel logLevel)
    {
        _logger?.Log(LogLevel.Error, line);
    }

    private void Log(Exception ex, LogLevel logLevel)
    {
        _logger?.Log(LogLevel.Error, ex, ex.Message);
    }


    public void ReportException(string line, Exception ex)
        => HandleException(ex, LogLevel.Information, line, Report);

    public void ReportException(string format, Exception ex, params object[] arg)
        => HandleException(ex, LogLevel.Information, format, arg, Report);

    public void ReportErrorException(string line, Exception ex)
        => HandleException(ex, LogLevel.Error, line, Report);

    public void ReportErrorException(string format, Exception ex, params object[] arg)
        => HandleException(ex, LogLevel.Error, format, arg, Report);

    public void ReportWarningException(string line, Exception ex)
        => HandleException(ex, LogLevel.Warning, line, Report);

    public void ReportWarningException(string format, Exception ex, params object[] arg)
        => HandleException(ex, LogLevel.Warning, format, arg, Report);

    public void ReportInfoException(string line, Exception ex)
        => HandleException(ex, LogLevel.Information, line, Report);

    public void ReportInfoException(string format, Exception ex, params object[] arg)
        => HandleException(ex, LogLevel.Information, format, arg, Report);
}
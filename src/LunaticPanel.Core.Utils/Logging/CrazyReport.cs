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
    private string? _moduleName = "System";
    private string? _className = "Object";
    private readonly IServiceProvider _serviceProvider;
    private ILogger? _logger;
    private Guid _circuitId;

    public CrazyReport(ICrazyReportCircuit crazyReportCircuit, IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _circuitId = crazyReportCircuit.CircuitId;
    }

    private void ErrorCatch(Action action)
    {
        try
        {
            action();
        }
        catch (Exception ex)
        {
            ReportErrorException("CRAZY REPORT INTERNAL ERROR: {0}", ex, ex.Message);
        }
    }
    public void Report(string line)
    {
        ErrorCatch(() =>
        {
            Console.WriteLine(line);
            Log(line, LogLevel.Trace);
        });
    }
    public void Report(string format, params object[] arg)
    {
        ErrorCatch(() =>
        {
            var line = string.Format(format, arg);
            Console.WriteLine(line);
            Log(line, LogLevel.Trace);
        });
    }
    public void ReportError(string line)
    {
        ErrorCatch(() =>
        {
            Console.WriteLine($"{Red}{GetPrefix()}: {line}{Reset}");
            Log(line, LogLevel.Error);
        });
    }
    public void ReportError(string format, params object[] arg)
    {
        ErrorCatch(() =>
        {
            var line = string.Format(format, arg);
            Console.WriteLine($"{Red}{GetPrefix()}: {line}{Reset}");
            Log(line, LogLevel.Error);
        });
    }

    public void ReportInfo(string line)
    {
        ErrorCatch(() =>
        {
            Console.WriteLine($"{DarkCyan}{GetPrefix()}: {line}{Reset}");
            Log(line, LogLevel.Information);
        });
    }
    public void ReportInfo(string format, params object[] arg)
    {
        ErrorCatch(() =>
        {
            var line = string.Format(format, arg);
            Console.WriteLine($"{DarkCyan}{GetPrefix()}: {line}{Reset}");
            Log(line, LogLevel.Information);
        });
    }
    public void ReportSuccess(string line)
    {
        ErrorCatch(() =>
        {
            Console.WriteLine($"{DarkGreen}{GetPrefix()}: {line}{Reset}");
            Log(line, LogLevel.Debug);
        });
    }
    public void ReportSuccess(string format, params object[] arg)
    {
        ErrorCatch(() =>
        {
            var line = string.Format(format, arg);
            Console.WriteLine($"{DarkGreen}{GetPrefix()}: {line}{Reset}");
            Log(line, LogLevel.Debug);
        });
    }
    public void ReportWarning(string line)
    {
        ErrorCatch(() =>
        {
            Console.WriteLine($"{DarkYellow}{GetPrefix()}: {line}{Reset}");
            Log(line, LogLevel.Warning);
        });
    }
    public void ReportWarning(string format, params object[] arg)
    {
        ErrorCatch(() =>
        {
            var line = string.Format(format, arg);
            Console.WriteLine($"{DarkYellow}{GetPrefix()}: {line}{Reset}", arg);
            Log(line, LogLevel.Warning);
        });

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
        var lineprocess = string.Format("[{1}] {0}", line, ex.GetType());
        toReport(lineprocess);
        Log(ex, logLevel);
    }
    private void HandleException(Exception ex, LogLevel logLevel, string format, object[] arg, Action<string, object[]> toReport)
    {
        var lineprocess = string.Format("[{1}] {0}", format, ex.GetType());
        toReport(lineprocess, arg);
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
        => HandleException(ex, LogLevel.Error, line, ReportError);

    public void ReportErrorException(string format, Exception ex, params object[] arg)
        => HandleException(ex, LogLevel.Error, format, arg, ReportError);

    public void ReportWarningException(string line, Exception ex)
        => HandleException(ex, LogLevel.Warning, line, ReportWarning);

    public void ReportWarningException(string format, Exception ex, params object[] arg)
        => HandleException(ex, LogLevel.Warning, format, arg, ReportWarning);

    public void ReportInfoException(string line, Exception ex)
        => HandleException(ex, LogLevel.Information, line, ReportInfo);

    public void ReportInfoException(string format, Exception ex, params object[] arg)
        => HandleException(ex, LogLevel.Information, format, arg, ReportInfo);
}
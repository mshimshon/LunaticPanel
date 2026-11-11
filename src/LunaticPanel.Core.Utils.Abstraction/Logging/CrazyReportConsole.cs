namespace LunaticPanel.Core.Utils.Abstraction.Logging;
/// <summary>
/// Quick Global Coloring Console Logging.<br/>
/// Note: Has limited features and configuration compared to ICrazyReport service but can be useful where services aren't available.
/// Report Info and Report Success = STDOUT anything else STDERR
/// </summary>
public static class CrazyReportConsole
{
    private const string RESET = "\u001b[0m";
    private const string RED = "\u001b[31m";
    private const string GREEN = "\u001b[32m";
    private const string YELLOW = "\u001b[33m";
    private const string MAGENTA = "\u001b[35m";
    private const string CYAN = "\u001b[36m";
    private const string WHITE = "\u001b[37m";
    public static CrazyReportLevel[] LogLevel { get; set; } = [
            CrazyReportLevel.Errors,
            CrazyReportLevel.Highlights,
            CrazyReportLevel.Info,
            CrazyReportLevel.Reports,
            CrazyReportLevel.Success,
            CrazyReportLevel.Warnings
        ];

    public static void Report(string line)
    {
        if (LogLevel.Contains(CrazyReportLevel.Reports))
            Console.Error.WriteLine($"{WHITE}{line}{RESET}");
    }
    public static void Report(string format, params object[] arg)
    {
        if (LogLevel.Contains(CrazyReportLevel.Reports))
            Console.Error.WriteLine($"{WHITE}{string.Format(format, arg)}{RESET}");
    }
    public static void ReportError(string line)
    {
        if (LogLevel.Contains(CrazyReportLevel.Errors))
            Console.Error.WriteLine($"{RED}{line}{RESET}");
    }
    public static void ReportError(string format, params object[] arg)
    {
        if (LogLevel.Contains(CrazyReportLevel.Errors))
            Console.Error.WriteLine($"{RED}{string.Format(format, arg)}{RESET}");
    }
    public static void ReportWarning(string line)
    {
        if (LogLevel.Contains(CrazyReportLevel.Warnings))
            Console.Error.WriteLine($"{YELLOW}{line}{RESET}");
    }
    public static void ReportWarning(string format, params object[] arg)
    {
        if (LogLevel.Contains(CrazyReportLevel.Warnings))
            Console.Error.WriteLine($"{YELLOW}{string.Format(format, arg)}{RESET}");
    }
    public static void ReportInfo(string line)
    {
        if (LogLevel.Contains(CrazyReportLevel.Info))
            Console.WriteLine($"{CYAN}{line}{RESET}");
    }
    public static void ReportInfo(string format, params object[] arg)
    {
        if (LogLevel.Contains(CrazyReportLevel.Info))
            Console.WriteLine($"{CYAN}{string.Format(format, arg)}{RESET}");
    }
    public static void ReportSuccess(string line)
    {
        if (LogLevel.Contains(CrazyReportLevel.Success))
            Console.WriteLine($"{GREEN}{line}{RESET}");
    }
    public static void ReportSuccess(string format, params object[] arg)
    {
        if (LogLevel.Contains(CrazyReportLevel.Success))
            Console.WriteLine($"{GREEN}{string.Format(format, arg)}{RESET}");
    }
    public static void ReportHighlight(string line)
    {
        if (LogLevel.Contains(CrazyReportLevel.Highlights))
            Console.Error.WriteLine($"{MAGENTA}{line}{RESET}");
    }
    public static void ReportHighlight(string format, params object[] arg)
    {
        if (LogLevel.Contains(CrazyReportLevel.Highlights))
            Console.Error.WriteLine($"{MAGENTA}{string.Format(format, arg)}{RESET}");
    }

    public static void SafeReport(this ICrazyReport? crazyReport, string line)
    {
        if (crazyReport != default)
            crazyReport?.Report(line);
        else Report(line);
    }
    public static void SafeReportException(this ICrazyReport? crazyReport, string line, Exception ex)
    {
        if (crazyReport != default)
            crazyReport?.ReportException(line, ex);
        else ReportError(line);
    }
    public static void SafeReport(this ICrazyReport? crazyReport, string format, params object[] arg)
    {
        if (crazyReport != default)
            crazyReport?.Report(format, arg);
        else Report(format, arg);
    }
    public static void SafeReportException(this ICrazyReport? crazyReport, string format, Exception ex, params object[] arg)
    {
        if (crazyReport != default)
            crazyReport?.ReportException(format, ex, arg);
        else Report(format, arg);
    }
    public static void SafeReportError(this ICrazyReport? crazyReport, string line)
    {
        if (crazyReport != default)
            crazyReport?.ReportError(line);
        else ReportError(line);
    }
    public static void SafeReportErrorException(this ICrazyReport? crazyReport, string line, Exception ex)
    {
        if (crazyReport != default)
            crazyReport?.ReportErrorException(line, ex);
        else ReportError(line);
    }
    public static void SafeReportError(this ICrazyReport? crazyReport, string format, params object[] arg)
    {
        if (crazyReport != default)
            crazyReport?.ReportError(format, arg);
        else ReportError(format, arg);
    }
    public static void SafeReportErrorException(this ICrazyReport? crazyReport, string format, Exception ex, params object[] arg)
    {
        if (crazyReport != default)
            crazyReport?.ReportErrorException(format, ex, arg);
        else ReportError(string.Format(format, arg));
    }
    public static void SafeReportWarning(this ICrazyReport? crazyReport, string line)
    {
        if (crazyReport != default)
            crazyReport?.ReportWarning(line);
        else ReportWarning(line);
    }
    public static void SafeReportWarningException(this ICrazyReport? crazyReport, string line, Exception ex)
    {
        if (crazyReport != default)
            crazyReport?.ReportWarningException(line, ex);
        else ReportWarning(line);
    }
    public static void SafeReportWarning(this ICrazyReport? crazyReport, string format, params object[] arg)
    {
        if (crazyReport != default)
            crazyReport?.Report(format, arg);
        else ReportWarning(format, arg);
    }
    public static void SafeReportWarningException(this ICrazyReport? crazyReport, string format, Exception ex, params object[] arg)
    {
        if (crazyReport != default)
            crazyReport?.ReportWarningException(format, ex, arg);
        else ReportWarning(format, arg);
    }
    public static void SafeReportInfo(this ICrazyReport? crazyReport, string line)
    {
        if (crazyReport != default)
            crazyReport?.ReportInfo(line);
        else ReportInfo(line);
    }
    public static void SafeReportInfoException(this ICrazyReport? crazyReport, string line, Exception ex)
    {
        if (crazyReport != default)
            crazyReport?.ReportInfoException(line, ex);
        else ReportInfo(line);
    }
    public static void SafeReportInfo(this ICrazyReport? crazyReport, string format, params object[] arg)
    {
        if (crazyReport != default)
            crazyReport?.ReportInfo(format, arg);
        else ReportInfo(format, arg);
    }
    public static void SafeReportInfoException(this ICrazyReport? crazyReport, string format, Exception ex, params object[] arg)
    {
        if (crazyReport != default)
            crazyReport?.ReportInfoException(format, ex, arg);
        else ReportInfo(format, arg);
    }

    public static void SafeReportSuccess(this ICrazyReport? crazyReport, string line)
    {
        if (crazyReport != default)
            crazyReport?.ReportSuccess(line);
        else ReportSuccess(line);
    }
    public static void SafeReportSuccess(this ICrazyReport? crazyReport, string format, params object[] arg)
    {
        if (crazyReport != default)
            crazyReport?.ReportSuccess(format, arg);
        else ReportSuccess(format, arg);
    }
}

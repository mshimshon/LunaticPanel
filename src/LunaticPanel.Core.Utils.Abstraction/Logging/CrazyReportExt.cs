namespace LunaticPanel.Core.Utils.Abstraction.Logging;

public static class CrazyReportExt
{
    public static void SafeReport(this ICrazyReport? crazyReport, string line)
    {
        if (crazyReport != default)
            crazyReport?.Report(line);
        else Console.Out.WriteLine(line);
    }
    public static void SafeReportException(this ICrazyReport? crazyReport, string line, Exception ex)
    {
        if (crazyReport != default)
            crazyReport?.ReportException(line, ex);
        else Console.Error.WriteLine(line);
    }
    public static void SafeReport(this ICrazyReport? crazyReport, string format, params object[] arg)
    {
        if (crazyReport != default)
            crazyReport?.Report(format, arg);
        else Console.Error.WriteLine(string.Format(format, arg));
    }
    public static void SafeReportException(this ICrazyReport? crazyReport, string format, Exception ex, params object[] arg)
    {
        if (crazyReport != default)
            crazyReport?.ReportException(format, ex, arg);
        else Console.Error.WriteLine(string.Format(format, arg));
    }
    public static void SafeReportError(this ICrazyReport? crazyReport, string line)
    {
        if (crazyReport != default)
            crazyReport?.ReportError(line);
        else Console.Error.WriteLine(line);
    }
    public static void SafeReportErrorException(this ICrazyReport? crazyReport, string line, Exception ex)
    {
        if (crazyReport != default)
            crazyReport?.ReportErrorException(line, ex);
        else Console.Error.WriteLine(line);
    }
    public static void SafeReportError(this ICrazyReport? crazyReport, string format, params object[] arg)
    {
        if (crazyReport != default)
            crazyReport?.ReportError(format, arg);
        else Console.Error.WriteLine(string.Format(format, arg));
    }
    public static void SafeReportErrorException(this ICrazyReport? crazyReport, string format, Exception ex, params object[] arg)
    {
        if (crazyReport != default)
            crazyReport?.ReportErrorException(format, ex, arg);
        else Console.Error.WriteLine(string.Format(format, arg));
    }
    public static void SafeReportWarning(this ICrazyReport? crazyReport, string line)
    {
        if (crazyReport != default)
            crazyReport?.ReportWarning(line);
        else Console.Error.WriteLine(line);
    }
    public static void SafeReportWarningException(this ICrazyReport? crazyReport, string line, Exception ex)
    {
        if (crazyReport != default)
            crazyReport?.ReportWarningException(line, ex);
        else Console.Error.WriteLine(line);
    }
    public static void SafeReportWarning(this ICrazyReport? crazyReport, string format, params object[] arg)
    {
        if (crazyReport != default)
            crazyReport?.Report(format, arg);
        else Console.Error.WriteLine(string.Format(format, arg));
    }
    public static void SafeReportWarningException(this ICrazyReport? crazyReport, string format, Exception ex, params object[] arg)
    {
        if (crazyReport != default)
            crazyReport?.ReportWarningException(format, ex, arg);
        else Console.Error.WriteLine(string.Format(format, arg));
    }
    public static void SafeReportInfo(this ICrazyReport? crazyReport, string line)
    {
        if (crazyReport != default)
            crazyReport?.ReportInfo(line);
        else Console.Out.WriteLine(line);
    }
    public static void SafeReportInfoException(this ICrazyReport? crazyReport, string line, Exception ex)
    {
        if (crazyReport != default)
            crazyReport?.ReportInfoException(line, ex);
        else Console.Error.WriteLine(line);
    }
    public static void SafeReportInfo(this ICrazyReport? crazyReport, string format, params object[] arg)
    {
        if (crazyReport != default)
            crazyReport?.ReportInfo(format, arg);
        else Console.Out.WriteLine(string.Format(format, arg));
    }
    public static void SafeReportInfoException(this ICrazyReport? crazyReport, string format, Exception ex, params object[] arg)
    {
        if (crazyReport != default)
            crazyReport?.ReportInfoException(format, ex, arg);
        else Console.Error.WriteLine(string.Format(format, arg));
    }

    public static void SafeReportSuccess(this ICrazyReport? crazyReport, string line)
    {
        if (crazyReport != default)
            crazyReport?.ReportSuccess(line);
        else Console.Out.WriteLine(line);
    }
    public static void SafeReportSuccess(this ICrazyReport? crazyReport, string format, params object[] arg)
    {
        if (crazyReport != default)
            crazyReport?.ReportSuccess(format, arg);
        else Console.Out.WriteLine(string.Format(format, arg));
    }
}

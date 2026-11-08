namespace LunaticPanel.Core.Utils.Abstraction.Logging;


public interface ICrazyReport
{
    const string RESET = "\u001b[0m";
    const string DARK_GREEN = "\u001b[32m";
    const string RED = "\u001b[31m";
    const string DARK_CYAN = "\u001b[36m";
    const string DARK_YELLOW = "\u001b[33m";
    void SetModule(string moduleName);
    void SetModule<TClass>(string moduleName) where TClass : class;
    void SetClass<TClass>() where TClass : class;
    void SetClass(string className);

    void Report(string line);
    void ReportException(string line, Exception ex);
    void Report(string format, params object[] arg);
    void ReportException(string format, Exception ex, params object[] arg);
    void ReportError(string line);
    void ReportErrorException(string line, Exception ex);
    void ReportError(string format, params object[] arg);
    void ReportErrorException(string format, Exception ex, params object[] arg);
    void ReportWarning(string line);
    void ReportWarningException(string line, Exception ex);
    void ReportWarning(string format, params object[] arg);
    void ReportWarningException(string format, Exception ex, params object[] arg);
    void ReportInfo(string line);
    void ReportInfoException(string line, Exception ex);
    void ReportInfo(string format, params object[] arg);
    void ReportInfoException(string format, Exception ex, params object[] arg);

    void ReportSuccess(string line);
    void ReportSuccess(string format, params object[] arg);
}

public interface ICrazyReport<T> : ICrazyReport { }

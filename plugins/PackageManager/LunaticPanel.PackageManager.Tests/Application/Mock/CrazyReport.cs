using LunaticPanel.Core.Utils.Abstraction.Logging;

namespace LunaticPanel.PackageManager....LunaticPanel.PackageManager.Tests.Application.Mock;

public class CrazyReport<T> : ICrazyReport<T>
{
    public void Report(string line) { }
    public void Report(string format, params object[] arg) { }
    public void ReportError(string line) { }
    public void ReportError(string format, params object[] arg) { }
    public void ReportErrorException(string line, Exception ex) { }
    public void ReportErrorException(string format, Exception ex, params object[] arg) { }
    public void ReportException(string line, Exception ex) { }
    public void ReportException(string format, Exception ex, params object[] arg) { }
    public void ReportInfo(string line) { }
    public void ReportInfo(string format, params object[] arg) { }
    public void ReportInfoException(string line, Exception ex) { }
    public void ReportInfoException(string format, Exception ex, params object[] arg) { }
    public void ReportSuccess(string line) { }
    public void ReportSuccess(string format, params object[] arg) { }
    public void ReportWarning(string line) { }
    public void ReportWarning(string format, params object[] arg) { }
    public void ReportWarningException(string line, Exception ex) { }
    public void ReportWarningException(string format, Exception ex, params object[] arg) { }
    public void SetClass<TClass>() where TClass : class { }
    public void SetClass(string className) { }
    public void SetModule(string moduleName) { }
    public void SetModule<TClass>(string moduleName) where TClass : class { }

}

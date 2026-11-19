using LunaticPanel.Core.Utils.Abstraction.Logging;
using NSubstitute;
using System;

namespace LunaticPanel.PackageManager....LunaticPanel.PackageManager.Tests.Web.Pulses.Utils;

public class CrazyReportMock<T> : ICrazyReport<T> where T : class
{
    public ICrazyReport<T> Mock { get; } = Substitute.For<ICrazyReport<T>>();

    public void Report(string line) => Mock.Report(line);
    public void Report(string format, params object[] arg) => Mock.Report(format, arg);

    public void ReportError(string line) => Mock.ReportError(line);
    public void ReportError(string format, params object[] arg) => Mock.ReportError(format, arg);
    public void ReportErrorException(string line, Exception ex) => Mock.ReportErrorException(line, ex);
    public void ReportErrorException(string format, Exception ex, params object[] arg) => Mock.ReportErrorException(format, ex, arg);

    public void ReportException(string line, Exception ex) => Mock.ReportException(line, ex);
    public void ReportException(string format, Exception ex, params object[] arg) => Mock.ReportException(format, ex, arg);

    public void ReportInfo(string line) => Mock.ReportInfo(line);
    public void ReportInfo(string format, params object[] arg) => Mock.ReportInfo(format, arg);
    public void ReportInfoException(string line, Exception ex) => Mock.ReportInfoException(line, ex);
    public void ReportInfoException(string format, Exception ex, params object[] arg) => Mock.ReportInfoException(format, ex, arg);

    public void ReportSuccess(string line) => Mock.ReportSuccess(line);
    public void ReportSuccess(string format, params object[] arg) => Mock.ReportSuccess(format, arg);

    public void ReportWarning(string line) => Mock.ReportWarning(line);
    public void ReportWarning(string format, params object[] arg) => Mock.ReportWarning(format, arg);
    public void ReportWarningException(string line, Exception ex) => Mock.ReportWarningException(line, ex);
    public void ReportWarningException(string format, Exception ex, params object[] arg) => Mock.ReportWarningException(format, ex, arg);

    public void SetClass<TClass>() where TClass : class => Mock.SetClass<TClass>();
    public void SetClass(string className) => Mock.SetClass(className);

    public void SetModule(string moduleName) => Mock.SetModule(moduleName);
    public void SetModule<TClass>(string moduleName) where TClass : class => Mock.SetModule<TClass>(moduleName);
}

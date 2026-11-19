using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using System;
using System.Reflection;

namespace LunaticPanel.Engine.Web.Bootstraper;

public interface IBootstrap
{
    IReadOnlyList<Assembly> AdditionalAssemblies { get; }
    Task SetupAsync(WebApplication webApp, IServiceProvider masterSp);
    Task RunAsync(IServiceProvider serviceProvider);
}

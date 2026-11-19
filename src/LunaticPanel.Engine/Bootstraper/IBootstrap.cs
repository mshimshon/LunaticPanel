using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using System;

namespace LunaticPanel.Engine.Web.Bootstraper;

public interface IBootstrap
{
    Task SetupAsync(WebApplication webApp, IServiceProvider masterSp);
    Task RunAsync(IServiceProvider serviceProvider);
}

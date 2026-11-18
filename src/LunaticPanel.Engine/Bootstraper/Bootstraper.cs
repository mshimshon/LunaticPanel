using Microsoft.Extensions.Configuration;
using System;

namespace LunaticPanel.Engine.Web.Bootstraper;

public class Bootstraper
{
    public static Bootstraper Create() => new();
    public async Task Setup(IServiceCollection services, IConfiguration configuration)
    {

    }
}

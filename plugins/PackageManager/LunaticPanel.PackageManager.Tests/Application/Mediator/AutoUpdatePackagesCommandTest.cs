using LunaticPanel.Core.Abstraction.Tools;
using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.PackageManager.Application.Mediator.Commands;
using LunaticPanel.PackageManager.Application.Mediator.Commands.Handlers;
using LunaticPanel.PackageManager.Application.Pulses.States;
using LunaticPanel.PackageManager.Tests.Application.Mock;
using StatePulse.Net;
using System;
using LunaticPanel.Core.Utils.Logging;
using Microsoft.Extensions.DependencyInjection;
using MaksimShimshon.Mediator;
namespace LunaticPanel.PackageManager.Tests.Application.Mediator;

public class AutoUpdatePackagesCommandTest
{
    private readonly IPanelControl _panelControl = new PanelControl();
    private readonly IStateAccessor<PackageUpdateState> _packageUpdateState = StateAccessor<PackageUpdateState>.CreateState();
    private readonly IStateAccessor<PackageManagerDiskState> _packageManagerDiskState = StateAccessor<PackageManagerDiskState>.CreateState();
    private readonly IServiceProvider _serviceProvider;
    private readonly ICrazyReport _crazyReport = new CrazyReport<AutoUpdatePackagesCommandTest>();
    public AutoUpdatePackagesCommandTest()
    {
        IServiceCollection services = new ServiceCollection();
        services.AddMediatorService();
        services.AddScoped((sp) => _panelControl);
        services.AddScoped((sp) => _packageUpdateState);
        services.AddScoped((sp) => _packageManagerDiskState);
        services.AddScoped((sp) => _crazyReport);
        services.BuildMediatorRouting();
        _serviceProvider = services.BuildServiceProvider().CreateScope().ServiceProvider;
    }

    [Fact]
    public void Command_MapsCorrectly()
    {
        new AutoUpdatePackagesCommandHandler(_panelControl, );
        Assert.Collection(respositorySourceQueryModel.RepositorySourceTypes,
            p => Assert.True(p == RepositorySourceType.Local),
            p => Assert.True(p == RepositorySourceType.Remote)
        );
    }
}

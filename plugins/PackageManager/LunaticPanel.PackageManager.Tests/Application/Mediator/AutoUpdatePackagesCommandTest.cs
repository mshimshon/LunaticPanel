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
    private readonly ICrazyReport<AutoUpdatePackagesCommandHandler> _crazyReport = new CrazyReport<AutoUpdatePackagesCommandHandler>();
    private readonly IMediator _mediator;
    public AutoUpdatePackagesCommandTest()
    {
        IServiceCollection services = new ServiceCollection();
        services.AddMediatorService();
        services.AddScoped((sp) => _panelControl);
        services.AddScoped((sp) => _packageUpdateState);
        services.AddScoped((sp) => _packageManagerDiskState);
        services.AddScoped((sp) => _crazyReport);
        services.BuildMediatorRouting();
        var sp = services.BuildServiceProvider().CreateScope().ServiceProvider;
        _mediator = sp.GetRequiredService<IMediator>();

    }

    [Fact]
    public async Task Command_MapsCorrectly()
    {
        var handler = new AutoUpdatePackagesCommandHandler(_panelControl, _mediator, _packageUpdateState, _packageManagerDiskState, _crazyReport);
        await handler.HandleAsync();
        Assert.Collection(respositorySourceQueryModel.RepositorySourceTypes,
            p => Assert.True(p == RepositorySourceType.Local),
            p => Assert.True(p == RepositorySourceType.Remote)
        );
    }
}

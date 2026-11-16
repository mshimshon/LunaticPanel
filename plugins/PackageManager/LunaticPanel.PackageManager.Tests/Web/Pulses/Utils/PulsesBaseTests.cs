using LunaticPanel.Core.Abstraction.Tools;
using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.PackageManager.Application.Mediator.Commands;
using LunaticPanel.PackageManager.Application.Mediator.Commands.Handlers;
using LunaticPanel.PackageManager.Pulses.States;
using LunaticPanel.PackageManager.Tests.Application.Mock;
using StatePulse.Net;
using System;
using LunaticPanel.Core.Utils.Logging;
using Microsoft.Extensions.DependencyInjection;
using MaksimShimshon.Mediator;
using NuGet.Versioning;
using LunaticPanel.PackageManager.Application.Payloads;
using NSubstitute;
using NSubstitute.Extensions;
using LunaticPanel.Core.Abstraction.Exceptions;
using LunaticPanel.PackageManager.Tests.Application.Mediator.Extensions;
using NSubstitute.ExceptionExtensions;
using LunaticPanel.PackageManager.Pulses.Effects;
using LunaticPanel.PackageManager.Pulses.Actions;
using LunaticPanel.PackageManager.Application.Payloads.Enums;
using LunaticPanel.PackageManager.Pulses;
namespace LunaticPanel.PackageManager.Tests.Application.Pulses;

[CollectionDefinition("NoParallel", DisableParallelization = true)]
public class NoParallelCollection { }

[Collection("NoParallel")]
public abstract class PulsesBaseTests
{
    protected readonly IMediator _mediator = Substitute.For<IMediator>();
    protected readonly IServiceCollection _services = new ServiceCollection();
    public ICrazyReport<T> CreateStateMock<T>() where T : class
    {
        return Substitute.For<ICrazyReport<T>>();
    }
    public PulsesBaseTests()
    {
        _mediator = Substitute.For<IMediator>();
        var crazyReport = Substitute.For<ICrazyReport>();
        _services.AddStatePulseServices();
        _services.AddPulseServices();
        _services.AddScoped((sp) => _mediator);
        _services.AddScoped((sp) => crazyReport);
        _services.AddScoped(typeof(ICrazyReport<>), typeof(CrazyReportMock<>));
    }



}

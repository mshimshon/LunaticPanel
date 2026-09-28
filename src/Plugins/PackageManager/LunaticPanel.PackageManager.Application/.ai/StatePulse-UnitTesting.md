
# Instructions
# THIS WORKFLOW TAKES TARGET CLASS FOR TESTING FROM LunaticPanel.PackageManager.Application
# THIS WORKFLOW ONLY CREATES UNIT TESTS WITHIN THE PROJECT LunaticPanel.PackageManager.Tests
# Workspace is inside Application folder of the 'LunaticPanel.PackageManager.Tests' project.
# UnitTest Engine is xunit.v3.
# Replace [FeatureName] with the target statepulse action name we are generating unit test for.
# Replace [FeatureState] with the featured reducer bound state.
# Replace [FeatureMediator] with Command if mediator bound action is a command and Query if it's a query which you can fing by inspecting the effect class of the statepulse action and extract Command/Query from the line _mediator like such '_medihater.Send(new RepositorySourceAddCommand(action.Source), dispatcher.CancelToken);'.
# Always Read as Instruction {AI_DO}{/AI_DO} and Remove from final output.
# INGORE SYNTAX ERROR YOUR ONLY AND ONLY JOB IS TO FILL THE TEMPLATE LEAVE ERRORS AS IS
# DO NOT BUILD
# DO NOT DEVIATE FROM THE TEMPLATE!

---


### 1. Application/Pulses/[FeatureName]SPTests.cs
```csharp
using Microsoft.Extensions.DependencyInjection;
using LunaticPanel.PackageManager.Application.Pulses.Actions;
using StatePulse.Net;
using MedihatR;
using LunaticPanel.PackageManager.Application.Pulses;
using LunaticPanel.PackageManager.Application.Mediator;

namespace LunaticPanel.PackageManager.Tests.Application.Pulses;

public class [FeatureName]SPTests
{
    private readonly IServiceProvider _scope;
    private readonly IDispatcher _dispatcher;
    private readonly IStateAccessor<[FeatureState]> _state;
    public [FeatureName]SPTests()
    {
        IServiceCollection services = new ServiceCollection();
        // TODO: Register Mocked Services.
        services.AddStatePulseServices();
        services.AddMedihaterServices();
        services.AddApplicationPulses();
        services.AddApplicationMediator();
        _scope = services.BuildServiceProvider().CreateScope().ServiceProvider;
        _dispatcher = _scope.GetRequiredService<IDispatcher>();
        _state = _scope.GetRequiredService<IStateAccessor<[FeatureState]>>();
    }

    {AI_DO}
    Generate Methods preferrably using Inline data as theory if possible in a clean way.
    You must first check the reducer associated with action to know which state are we testing.
    {/AI_DO}

    [Theory]
    public async Task [FeatureName]_ShouldExecuteSuccessfully()
    {
         await dispatcher.Prepare<[FeatureName]Action>().Await().DispatchAsync();
         // TODO: VALIDATE RESULTS
    }

    [Theory]
    public async Task [FeatureName]_ShouldFailExecution()
    {
         await dispatcher.Prepare<[FeatureName]Action>().Await().DispatchAsync();
         // TODO: VALIDATE RESULTS
    }
}

```
[FeatureName]CQRSTests
{AI_DO}
If the mediator related to the StatePulse action is a Command
{/AI_DO}
### 2. Application/Mediator/[FeatureName]CQRSTests.cs
```csharp
using LunaticPanel.PackageManager.Application.Mediator;
using LunaticPanel.PackageManager.Application.Mediator.Commands;
using MedihatR;
using Microsoft.Extensions.DependencyInjection;

namespace LunaticPanel.PackageManager.Tests.Mediator;

public class [FeatureName][FeatureMediator]Tests
{
    private readonly IServiceProvider _scope;
    private readonly IMedihater _medihater;
    public [FeatureName][FeatureMediator]Tests()
    {
        IServiceCollection services = new ServiceCollection();
        // TODO: Register Mocked Services.
        // 
        services.AddMedihaterServices();
        services.AddApplicationMediator();
        _scope = services.BuildServiceProvider().CreateScope().ServiceProvider;
        _medihater = _scope.GetRequiredService<IMedihater>();
    }

    public async Task [FeatureName][FeatureMediator]_ShouldExecuteSuccessfully()
    {

        var action = new [FeatureName][FeatureMediator]();
        try
        {
            await _medihater.Send(action);
            Assert.True(true);
        }
        catch (Exception)
        {
            Assert.True(false);
            throw;
        }
    }

    public async Task  [FeatureName][FeatureMediator]_ErrorShouldFailExecution()
    {
        var action = new DisableSourceCommand();
        await Assert.ThrowsAsync<Exception>(async () => await _medihater.Send(action));
    }
}


```
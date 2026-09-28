
# Instructions
# THIS WORKFLOW TAKES TARGET CLASS FOR TESTING FROM LunaticPanel.PackageManager.Application
# THIS WORKFLOW ONLY CREATES UNIT TESTS WITHIN THE PROJECT LunaticPanel.PackageManager.Tests
# Workspace is inside Application folder of the 'LunaticPanel.PackageManager.Tests' project.
# UnitTest Engine is xunit.v3.
# Replace [FeatureName] with the target statepulse action name we are generating unit test for.
# Replace [FeatureState] with the featured reducer bound state.
# Always Read as Instruction {AI_DO}{/AI_DO} and Remove from final output.
# INGORE SYNTAX ERROR YOUR ONLY AND ONLY JOB IS TO FILL THE TEMPLATE LEAVE ERRORS AS IS
# DO NOT BUILD

---


### 1. Application/Pulses/[FeatureName].cs
```csharp
using Microsoft.Extensions.DependencyInjection;
using LunaticPanel.PackageManager.Application.Pulses.Actions;
namespace LunaticPanel.PackageManager.Tests.Application.Pulses;

internal class RepositorySourceTests
{
    private readonly IServiceProvider _scope;
    private readonly IDispatcher _dispatcher;
    private readonly IStateAccessor<[FeatureState]> _state;
    public RepositorySourceTests()
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
    public async Task [FeatureName]_ShouldNotExecuteSuccessfully()
    {
         await dispatcher.Prepare<[FeatureName]Action>().Await().DispatchAsync();
         // TODO: VALIDATE RESULTS
    }
}

```

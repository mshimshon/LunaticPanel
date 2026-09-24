

# Instructions
# Replace [FeatureName] with the target action name.
# Replace [KeyClass] with 'LPPackageManagerKeys'
# Replace [RootNamespace] with 'LunaticPanel.PackageManager'
# Replace [RootPulseNamespace] with 'LunaticPanel.PackageManager.Application.Pulses'
# Replace [RootMediatorNamespace] with 'LunaticPanel.PackageManager.Application.Mediator'
# Replace [MediatorRealm] with 'Query' if mediator query is mention in the prompt or 'Command' if command is mentioned in the prompt, If the prompt does not mention STOP and ask for it.
# Replace [MediatorRealmNamespace] with 'Queries' if mediator query is mention in the prompt or 'Commands' if command is mentioned in the prompt, If the prompt does not mention STOP and ask for it.
# Replace [ReturnType] the specified return type.
# Replace [BoundState] with the mentioned bound state , If the prompt does not mention STOP and ask for it.
# Always Read as Instruction {AI_DO}{/AI_DO} and Remove from final output.


---

### 1. Pulses/Actions/[FeatureName]Action.cs
```csharp
using StatePulse.Net;

namespace [RootPulseNamespace].Actions;

public sealed record [FeatureName]Action : IAction
{
}

```

### 2. Pulses/Actions/[FeatureName]DoneAction.cs
```csharp
using StatePulse.Net;

namespace [RootPulseNamespace].Actions;

public sealed record [FeatureName]DoneAction : IAction
{
}

```

### 3. Pulses/Effects/[FeatureName]Effect.cs
```csharp

using [RootMediatorNamespace].[MediatorRealmNamespace];
using [RootPulseNamespace].Actions;
using MedihatR;
using StatePulse.Net;

namespace [RootPulseNamespace].Effects;

internal class [FeatureName]Effect : IEffect<[FeatureName]Action>
{
    private readonly IMedihater _medihater;

    public AddSourceEffect(IMedihater medihater)
    {
        _medihater = medihater;
    }
    public async Task EffectAsync([FeatureName]Action action, IDispatcher dispatcher)
    {

        try
        {
            {AI_DO}
                USE IF REQUESTED PROMPT DOES NOT SPECIFY A RETURN TYPE
            await _medihater.Send(new [FeatureName][MediatorRealm](), dispatcher.CancelToken);
            await dispatcher.Prepare<[FeatureName]DoneAction>().DispatchAsync();
            {/AI_DO}
            {AI_DO}
                USE IF REQUESTED PROMPT DOES SPECIFY A RETURN TYPE
            [ReturnType] result = await _medihater.Send(new [FeatureName][MediatorRealm](), dispatcher.CancelToken);
            await dispatcher.Prepare<[FeatureName]DoneAction>().With(p=>p.ReturnValue, result).DispatchAsync();
            {/AI_DO}
        }
        catch (Exception)
        {
            await dispatcher.Prepare<[FeatureName]DoneAction>().DispatchAsync();
            throw;
        }

    }
}

```

### 4. Pulses/Reducers/[FeatureName]Reducer.cs
```csharp
using [RootPulseNamespace].Actions;
using [RootPulseNamespace].States;
using StatePulse.Net;

namespace [RootPulseNamespace].Reducers;

internal class [FeatureName]Reducer : IReducer<[BoundState], [FeatureName]Action>
{
    public [BoundState] Reduce([BoundState] state, [FeatureName]Action action)
        => state with { 

        };
}

```

### 5. Pulses/Reducers/[FeatureName]DoneReducer.cs
```csharp
using [RootPulseNamespace].Actions;
using [RootPulseNamespace].States;
using StatePulse.Net;

namespace [RootPulseNamespace].Reducers;

internal class [FeatureName]DoneReducer : IReducer<[BoundState], [FeatureName]DoneAction>
{
    public [BoundState] Reduce([BoundState] state, [FeatureName]DoneAction action)
        => state with { 
            
        };
}

```

# Instructions
#  'Query' if mediator query is mention in the prompt or 'Command' if command is mentioned in the prompt, If the prompt does not mention STOP and ask for it.



### 6. Mediator/[MediatorRealmNamespace]/[FeatureName][MediatorRealm].cs
```csharp
using MedihatR;

namespace [RootMediatorNamespace].[MediatorRealmNamespace];

public sealed record [FeatureName][MediatorRealm]()
    : {AI_DO} USE IF REQUESTED PROMPT DOES NOT SPECIFY RETURN TYPE USE  'IRequest' otherwise use 'IRequest<[ReturnType]>'{/AI_DO}
{
}
```


### 7. Mediator/[MediatorRealmNamespace]/Handlers/[FeatureName][MediatorRealm]Handler.cs
```csharp
using LunaticPanel.Core.Abstraction.Exceptions;
using LunaticPanel.Core.Utils.Abstraction.Logging;
using [RootNamespace].Keys;
using MedihatR;
namespace [RootMediatorNamespace].[MediatorRealmNamespace].Handlers;

internal class [FeatureName][MediatorRealm]Handler : {AI_DO} USE IF REQUESTED PROMPT DOES NOT SPECIFY RETURN TYPE USE  'IRequestHandler<[FeatureName][MediatorRealm]>' otherwise use 'IRequestHandler<[FeatureName][MediatorRealm], [ReturnType]>'{/AI_DO} 
{
    private readonly ICrazyReport<[FeatureName][MediatorRealm]Handler> _crazyReport;

    public [FeatureName][MediatorRealm]Handler(ICrazyReport<[FeatureName][MediatorRealm]Handler> crazyReport)
    {
        _crazyReport = crazyReport;
        _crazyReport.SetModule([KeyClass].MODULE_NAME);
        _crazyReport.Report("Initialized Class");

    }

    public async {AI_DO} USE IF REQUESTED PROMPT DOES NOT SPECIFY RETURN TYPE USE  'Task' otherwise use 'Task<[ReturnType]>'{/AI_DO}  Handle([FeatureName][MediatorRealm] request, CancellationToken ct = default)
    {
        try
        {
             _crazyReport.Report("Handler Called");
            //TODO: HANDLE DOMAIN EXCEPTIONS
        }
        catch (Exception)
        {
            throw new HostUnkownException();
        }
    }
}


```
### 8. Open file Mediator/RegisterServiceExt.cs and Append before '// AI APPEND ABOVE' if the comment '// AI APPEND ABOVE' is missing STOP and notify.
DO NOT REMOVE OR CHANGE EXISTING CODE.
```csharp
services.AddMedihaterHandler<[FeatureName][MediatorRealm]Handler>();
```
### 9. Open file Pulses/RegisterServiceExt.cs and Append before '// AI APPEND ABOVE' if the comment '// AI APPEND ABOVE' is missing STOP and notify.
DO NOT REMOVE OR CHANGE EXISTING CODE.
```csharp
    services.AddStatePulseService<[FeatureName]Action>();
    services.AddStatePulseService<[FeatureName]DoneAction>();
    services.AddStatePulseService<[FeatureName]Effect>();
    services.AddStatePulseService<[FeatureName]Reducer>();
    services.AddStatePulseService<[FeatureName]DoneReducer>();
```

---

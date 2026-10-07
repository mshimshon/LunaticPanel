## Keys Package  
Message Bus Keys Provider

This package exposes all message‑bus keys used by the plugin. Every key is a plain `const string` with zero dependencies. The package exists so other plugins can communicate with this plugin’s internal API natively, without reflection or internal references. Importing this package gives full access to the plugin’s public message surface.

The keys are grouped by functional domain and follow the standard naming pattern:
enginekey.[{AssemblyName}].vX.<path>  
querykey.[{AssemblyName}].vX.<path>  
eventkey.[{AssemblyName}].vX.<path>  

## Queries
Keys used to request data or trigger synchronous operations inside the plugin. Other plugins can call these to fetch state, run logic, or perform controlled actions.

## Engine (UI)
Keys that drive UI‑facing engine operations. These define interactions, UI updates, and engine‑level commands exposed to external plugins.

## Event
Keys emitted by the plugin to signal state changes, lifecycle transitions, or domain events. Other plugins can subscribe to these to react to internal behavior.

## ScheduledEvents
Keys representing scheduled or delayed events. These allow external plugins to hook into timed operations or periodic tasks managed by the plugin.

This package is the authoritative list of all accessible keys. If a plugin needs to talk to this one, it does it through these constants.

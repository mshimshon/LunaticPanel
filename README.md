﻿
[![Build](https://github.com/mshimshon/MaksimShimshon.Webmin.Module.GameServerManager/actions/workflows/ci.yml/badge.svg)](https://github.com/mshimshon/MaksimShimshon.Webmin.Module.GameServerManager/actions/workflows/ci.yml)
[![Deploy](https://github.com/mshimshon/MaksimShimshon.Webmin.Module.GameServerManager/actions/workflows/deploy.yml/badge.svg)](https://github.com/mshimshon/MaksimShimshon.Webmin.Module.GameServerManager/actions/workflows/deploy.yml)

## What is Lunatic Panel?

Lunatic Panel is a modular Blazor Server control panel that works like a mix of Webmin, WordPress, and Docker. The core idea is simple: the panel itself does nothing. Everything is a plugin. Lunatic Panel loads plugins, isolates them, gives them a shared UI surface, and provides a messaging system so plugins can talk to each other cleanly.

The main achievement of Lunatic Panel is its plugin system. Blazor Server does not expose the APIs needed for true plugin isolation, so Lunatic Panel implements its own semi‑isolated plugin architecture. Each plugin has its own dependency scope while sharing host UI dependencies. This allows plugins to stay independent without breaking the dashboard.

Plugins can technically use third‑party UI frameworks, but this often causes conflicts when multiple plugins depend on the same framework. Developers should request UI components in the core or contribute them upstream instead of bundling external UI frameworks.

Clean Architecture principles guide the design even though the core contains early experimental foundations. The panel’s job is to orchestrate plugins, load them, and provide a messaging system that lets plugins communicate without tight coupling.

### Messaging System

Lunatic Panel provides three messaging layers:

**UI Messaging (Engine Layer)**  
Plugins can request UI fragments from other plugins. This allows the dashboard to be built from modular pieces contributed by multiple plugins.

**Query Messaging**  
Plugins can query each other directly. Example: “Plugin A, is feature X enabled?” → true or false.

**Event Messaging**  
Plugins can emit and listen to events. This enables reactive behavior across the entire plugin ecosystem.

### StatePulse and Circuit Isolation

Each plugin runs in its own Blazor circuit. A service registered as a singleton becomes a singleton only inside that plugin’s scope. Lunatic Panel provides a cross‑circuit registration method to override this when needed.

StatePulse uses this mechanism. Any `ISingletonState` becomes cross‑circuit automatically, allowing all clients to share the same state in real time.

## How Debugging and YML Works

Lunatic Panel includes a Dotnet NuGet CLI tool that makes plugin development easier. Since the panel cannot be run directly inside Visual Studio, the CLI provides a simple command that installs the panel, installs your plugins, and starts everything in one action. This gives you a fast feedback loop while developing.

The CLI uses a YAML file called `lpcli-compose.yml`. It works similarly to Docker Compose or CI/CD pipelines. You place this file in the root of your project, run the CLI command, and the tool reads the YAML as the starting point for the entire setup. The YAML supports cascading imports, allowing you to split configuration into multiple files and include them from related folders.

The YAML file controls the full workflow: installing the panel, installing plugins, skipping steps, loading dependencies, and preparing a clean environment for debugging. This lets you reproduce the same development environment consistently without manual setup.

You must install the tool globally using `dotnet tool install --global LunaticPanel.DebugTool` then use `lpcli command` to run it, you run it from the folder of the project you are targeting, you you clone the LP repos main branch and import its YML into yours so the panel is compiled directly and and publish to WSL for your testing environment.

## Development Tool

The development tool reads the YAML file and executes its steps. It can build a C# project into a Linux service, pack and validate a plugin, automatically install that plugin, and mark it as installed and active. It can also run post‑processing actions such as copying development files into the WSL environment or executing simple Linux commands.

The tool provides a live log stream for all services. It supports skipping steps, which is useful when you have already built the service and only want to repeatedly test a plugin. It can also open an interactive terminal so you can run commands directly inside the Linux WSL environment.


## Packing Tool

The packing tool follows the Microsoft versioning standard. If the panel version is 10.0.1, then a packing tool for 10.0.1 exists as well. When you pack a plugin, the tool embeds the plugin’s version into its metadata. This version is critical because a plugin can only run on the same panel version and must follow the same versioning rules used by NuGet packages 10.0.1+ the plugin cannot use greater core version than the panel.

The packing tool can also extract the plugin manifest and validate the plugin. Validation ensures that all rules and requirements are met so the plugin is safe and compatible. Validation rules may evolve between major versions, but they remain stable within the same major version until the next major release.


## Repository Source

The repository source library works similarly to the NuGet API standard. It defines all API versions that any Lunatic Panel marketplace server must implement. Lunatic Panel ships with its own marketplace, but the API is open enough that you can build your own server if needed.

By default, Lunatic Panel includes a local repository implementation called `LunaticPanel.Package.LocalServer`. This server is meant for internal company use, private plugin distribution, and environments where plugins should remain inside the organization. It follows the same validation rules that apply to the official marketplace. It is not optimized for global distrubition and has no CDN but it is able to receive online API calls.

The core library for building a marketplace is `LunaticPanel.Package.Server`. Any marketplace or serving server depends on this library and implements its API. It is the standard interface for publishing, versioning, validating, and serving Lunatic Panel plugins.

## Plugin First Approach

Lunatic Panel is built with a plugin‑first philosophy inspired by video game modding frameworks. The panel itself is a simple orchestrator of the plugin lifecycle. Every feature—whether built‑in or added later—is a plugin. There are no hidden internal exceptions or special‑case logic; the entire system is designed so plugins operate as first‑class citizens.

All behavior in Lunatic Panel is driven by plugins communicating with other plugins. The architecture avoids “magic” and keeps the core dumb by design. This ensures that every capability, UI element, service, or workflow is implemented through the same plugin system, making the platform predictable, modular, and fully extensible.

### Fault Tolerance

Lunatic Panel includes a fault‑tolerant plugin policy. Plugins are always informed about which features and messaging endpoints are available. When a plugin depends on another plugin that is missing, it can gracefully disable its own endpoints. For example, if `Plugin.AWS.Adapter` is not installed, any event, query, or feature that relies on it will explicitly disable at the load up entry level.

The entire pipeline is designed to reduce fatal crashes. Keeping plugin code clean and predictable is still important, but the system itself is built to handle unhandled exceptions without breaking the panel but it is not always possible. Fault tolerance is a core part of the plugin architecture.



# Technical Stuff
Information:
Project Application Layer /API/ = Bus Handlers/Request for Query/Events External Access
Project Application Layer /CQRS/ = Internal Medihater CQRS pattern.

Versioning:
- x. = Major Breaking Changes
- x.x = Minor (Small New Features without break) that means any Core package contracts remains unchanged.
- x.x.x = Fixes (non breaking fixes of issues internal only)

Updates:
- Panel will auto-update constantly with garantuees.
- Panel will auto-update from x.0.0 to x.9.9 then next update is an upgrade.

Upgrade:
- Panel will never auto-upgrade without manual action or mass automation action.

Plugin Version:
- Plugin version policies aren't important what is important is the filename oof the package:
	- All plugins must follow the panel's version sematic and all plugins will auto-update up to breaking changes.

The essence of idea is .NET versioning method where as all plugins are built against a specifric Panel version ie: 1.2/1.3 or 2.1, to upgrade a panel from 1.x to 2.x all plugins must have an available update to install for panel 2.x all major versions are bound to the plugins and thus plugins will likely update before you upgrade your panel.

# Plugin Compatibility and Core Versioning Policy

## 2. Core Principles

### 2.1 Host Owns the Runtime
The host application exclusively owns:
- ASP.NET Core runtime
- Microsoft.Extensions.*
- Rendering pipeline
- Dependency injection container
- Execution context and lifecycle
- LunaticPanel.Core is the only package reference required to have a plugin work.

Plugins must never attempt to own or replace these systems.

---

### 2.2 Plugins Are Isolated by Default
Each plugin:
- Is loaded into its own AssemblyLoadContext
- May include private dependencies
- Must not ship framework or host-owned assemblies

Isolation applies to all non-framework dependencies.

---

### 2.3 Shared Execution Model
Plugins may interact with host services, but:
- All execution occurs inside the host runtime
- Plugins never instantiate or control framework primitives
- Plugins may only consume host-provided services

---

## 3. Framework Ownership Rules

### 3.1 Host-Owned Assemblies
The following assemblies are owned by the host and must never be shipped by plugins:
- MudBlazor
- System.Runtime
- System.Collections
- System.Net.Http
- Microsoft.AspNetCore.Components
- Microsoft.AspNetCore.Components.Web
- Microsoft.AspNetCore.Components.Forms
- Microsoft.AspNetCore.Components.Authorization
- Microsoft.JSInterop

These assemblies host provides to the plugin they are safe ASP.NET/Blazor Assemblies more may be added as breaking changes releases or issues are found.

---

### 3.2 Plugin Restrictions
Plugins must NOT:
- Include framework DLLs in their package
- Load framework assemblies manually
- Use self-contained publishing
- Override assembly resolution for host-owned assemblies

Plugins MAY:
- Reference framework assemblies at compile time
- Consume host-provided services at runtime

---

## 4. Assembly Resolution Model

Resolution order:

1. Plugin AssemblyLoadContext
2. Host Default AssemblyLoadContext

If a framework assembly is not found in the plugin context, it is resolved from the host.

This guarantees:
- Single instance of framework assemblies
- Shared type identity
- Stable runtime behavior

---

## 5. Versioning Policy

### 5.1 LTS-Based Versioning

- The host targets a specific .NET LTS version.
- All plugins must target the same LTS.
- The host version remains fixed for the duration of the LTS lifecycle.

### 5.2 Transitional Policy
- Core preview version will be available for plugin to upgrade to new host version!

### 5.3 Upgrade Policy

- Host upgrades only on new LTS releases.
- Plugins must be rebuilt against the new LTS.
- Previous LTS plugins are not guaranteed compatibility.

### 5.4 Version Mismatch Behavior

- If plugin references newer APIs not present in host → runtime failure.
- If plugin references older APIs compatible with host → allowed.

Compatibility is determined by API availability, not package version numbers.

---

## 6. Shared Types Policy

Shared types are limited to:
- Host-defined interfaces
- DTOs
- Message contracts

Framework types are implicitly shared by host ownership and must not be duplicated.

---

## 7. Forbidden Behaviors

Plugins must never:
- Instantiate framework internals (Renderers, Dispatchers, etc.)
- Create or manage DI containers that overlap with host DI
- Invoke rendering or lifecycle methods directly
- Load framework assemblies manually
- Depend on side effects of runtime internals
- Inject services directly into Renderfragment instead of using ViewModel pattern.

---

## 8. Supported Interaction Model

Plugins may:
- Consume host services via dependency injection
- Return data structures or descriptors
- Register callbacks throught

---

## 9. Validation

Plugins:
- All Razor Component/Pages must inherit `WidgetComponentBase`.
- PluginEntry must be locate at root of entry project.
- Namespace base must be the same as assembly name.
- Cannot reference more than one IPlugin any other found mean's directly plugin reference.


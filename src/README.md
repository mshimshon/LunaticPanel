﻿
[![Build](https://github.com/mshimshon/MaksimShimshon.Webmin.Module.GameServerManager/actions/workflows/ci.yml/badge.svg)](https://github.com/mshimshon/MaksimShimshon.Webmin.Module.GameServerManager/actions/workflows/ci.yml)
[![Deploy](https://github.com/mshimshon/MaksimShimshon.Webmin.Module.GameServerManager/actions/workflows/deploy.yml/badge.svg)](https://github.com/mshimshon/MaksimShimshon.Webmin.Module.GameServerManager/actions/workflows/deploy.yml)


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

- Microsoft.AspNetCore.*
- Microsoft.Extensions.*
- System.* (runtime assemblies)
- Any host-defined shared contract assemblies

These assemblies are loaded once in the default AssemblyLoadContext.

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
- Key Pakage of plugin MUST be names Namspace of the plugin .Keys


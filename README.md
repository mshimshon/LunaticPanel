﻿
[![Discord](https://img.shields.io/badge/Discord-Join-6A0DAD?logo=discord&logoColor=white)](https://discord.gg/DMegAnGyJv)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://github.com/mshimshon/LunaticPanel/blob/main/LICENSE)
[![NuGet Version](https://img.shields.io/nuget/v/LunaticPanel.Core)](https://www.nuget.org/packages/LunaticPanel.Core)
[![](https://img.shields.io/nuget/dt/LunaticPanel.Core?label=Downloads)](https://www.nuget.org/packages/LunaticPanel.Core)
[![Build](https://github.com/mshimshon/LunaticPanel/actions/workflows/build.yml/badge.svg)](https://github.com/mshimshon/LunaticPanel/actions/workflows/build.yml)
[![Deploy](https://github.com/mshimshon/LunaticPanel/actions/workflows/deploy.yml/badge.svg)](https://github.com/mshimshon/LunaticPanel/actions/workflows/deploy.yml)

# Lunatic Panel

**Lunatic Panel** is an open-source, modular control panel template built on **Blazor Server** and **.NET LTS**. The core panel is entirely barebones, it contains zero built-in configuration tools. Instead, all features are discovered, downloaded, and managed directly through an integrated **Marketplace** system.

For developers, it provides a clean, decoupled ecosystem to build and distribute server management utilities without library version conflicts or dependency clashing.

---

## Key Features

* **Marketplace-Driven Ecosystem:** users can add custom repository sources, similar to Linux `apt`. You can deploy your own package-serving endpoints (either hosted remotely or run locally via `LunaticPanel.Package.LocalServer`) to distribute verified, version-locked plugins directly to the control panel dashboard.
* **Auto-Update Engine:** The system enforces strict versioning policies to keep modules current and secure. Any minor updates are applied completely automatically. If a plugin receives a major update that introduces major version shift, the panel safeguards your system by requiring a manual confirmation or a mass update action triggered via the CLI.
* **No Dependency Clashing:** Plugins maintain isolated library scopes. Two separate marketplace tools can run completely different versions of the same third-party NuGet package simultaneously without breaking each other or the core dashboard application.
* **Instant UI Responses:** Built on Blazor Server, the control panel communicates natively over persistent WebSockets. All system metrics, background logs, and tool screens update on the web browser instantly without manual page refreshes.
* **Decoupled Communications:** Built-in UI, Query, and Event messaging frameworks allow marketplace tools to securely communicate, exchange UI fragments, and broadcast events across the entire panel ecosystem.

---
# System Requirements

| Resource             | Minimum           | Recommended         |
| :------------------- | :---------------- | :------------------ |
| **Operating System** | Debian 12+        | Debian 12+          |
| **Processor (CPU)**  | **2 vCPU** / Core | **4 vCPUs** / Cores |
| **Memory (RAM)**     | **1 GB RAM**      | **4 GB RAM**        |
| **Storage**          | **5 GB**          | **20 GB**           |
| **Runtime**          | .NET 10.0 LTS     | .NET 10.0 LTS       |

* **Note:** Do not take these values at face value; they are highly dependent on which plugins you run in the system. These recommendations are based on an estimated expectation of running only a few plugins.

You can adjust the optimization after installation by editing the service file (`/etc/systemd/system/lunaticpanel.service`) directly. The script will never override the service if it already exists.

Change the values to match your desired optimization. See the [.NET GC Documentation](https://microsoft.com) for more details on tuning the garbage collector for your specific use case.

Our core policy is that the panel must always remain accessible, no matter what other services are running alongside it. The panel itself is lightweight, but its eventual footprint depends entirely on the purpose it serves for you.

```text
[Service]
OOMScoreAdjust=-900
CPUWeight=1000
CPUQuota=100%
MemoryMin=514M
MemoryHigh=4G
```


## Panel Installation (Debian / Ubuntu)

You can deploy the orchestrator and configure its background environment using the official setup script.

### 1. Download and Run the Script
Execute these commands using ROOT or sudo, this will setup the whole panel including all official basic plugins (mostly required features):

```bash
wget -qO /tmp/panel_setup.sh https://raw.githubusercontent.com/mshimshon/LunaticPanel/main/panel_setup.sh && /bin/bash /tmp/panel_setup.sh "LunaticPanel.PackageManager"
```

### 2. What the Script Does
* Installs required native system prerequisites and the official Microsoft .NET Runtime dependencies.
* Downloads the latest production binary release of the Lunatic Panel, you can specify a specific version using `/bin/bash /tmp/panel_setup.sh "LunaticPanel.PackageManager" 1.0.1`.
* Configures and registers a background system service (`lunaticpanel.service`) so your panel remains active and boots automatically on system startup.

### 3. Access the Dashboard
Once the setup script finishes, the core application is live. Open your browser and navigate to:

```text
http://localhost:10001/
```

## Package Server Installation (Debian / Ubuntu)

You can deploy a package server with the ability push upload locally using cli api similar to nuget push `lpkg-cli push`.

```bash
dotnet tool install --global LunaticPanel.Packages.Tool
```

### 1. Download and Run the Script
Execute these commands using ROOT or sudo, this will setup the whole server API:

```bash
wget -qO /tmp/serve_packages.sh https://raw.githubusercontent.com/mshimshon/LunaticPanel/main/serve_packages.sh && /bin/bash /tmp/serve_packages.sh
```

### 2. What the Script Does
* Installs required native system prerequisites and the official Microsoft .NET Runtime dependencies.
* Downloads the latest production binary release of the Lunatic Panel Package Server.
* Configures and registers a background system daemon (`lpkg_localserver.service`) so your server remains active and boots automatically on system startup.

### 3. API endpoint
Once the setup script finishes, the application is live. you can push or setup the new source on the panel point to:

```text
http://localhost:10002/
```
---

# Developers

[Documentation](https://doc.lunaticpanel.com) | [Official Discord](https://discord.gg/DMegAnGyJv)

## Packing Tool

To pack and test your own packages install the official global command-line utility:

```bash
dotnet tool install --global LunaticPanel.Packages.Tool
```

## Developing Plugins
Ww built a feature tool set to make it easy to live test our development pipeline locally.
first your install the global command-line utility:

```bash
dotnet tool install --global LunaticPanel.DebugTool
```

The CLI tool utilizes an `lpcli-compose.yml` file to automate compilation, run integrity validation rules against your components, package them, and publish them to your environment.

If you have built docker-composes in the past this shouldn't be hard to setup, the mechanism is the same and ours cascades.

Full Documentation will be live soon for the rest of this process.


## Licensing
MIT Free it will always be period! the core is 100% free including official plugins shipped with it in this repository.

Any monetization will come from paid plugins specialities but rest assure that our official marketplace will feature strict ecosystem with licensing and billing to make it as easy as managing application on Apple store or Google store.

## Community and Contribution
The job is massive and never ending! there will never be enough good folks to contribute feel free to connect with us on discord, the project is young fresh and we need anyone able to improve, clean up and develop stability features.


### Commit Convention
To Keep branch history clean you must use [DeployItSmart](https://github.com/mshimshon/DeployItSmart) Standard Convention.

| Token      | Description                                        | Version Change | Example          |
| ---------- | -------------------------------------------------- | -------------- | ---------------- |
| `feat!:`   | Breaking Change                                    | MAJOR          | `1.0.0 -> 2.0.0` |
| `feat:`    | Non Breaking Feature                               | MINOR          | `1.0.0 -> 1.1.0` |
| `fix:`     | Non Breaking Fix                                   | PATCH          | `1.0.0 -> 1.0.1` |
| `note:`    | Release Note                                       | PATCH          | `1.0.0 -> 1.0.1` |
| `sponsor:` | Release Sponsor `@username` to link username       | PATCH          | `1.0.0 -> 1.0.1` |
| `thank:`   | Release Thank `@username` to link username         | PATCH          | `1.0.0 -> 1.0.1` |
| `wip:`     | Dirty Work in progress, should not be pushed.      | PATCH          | `1.0.0 -> 1.0.1` |
| `trace:`   | Quick Update Undocumented Change (Non Breaking)    | PATCH          | `1.0.0 -> 1.0.1` |
| `trace!:`  | Push/Merge without triggering the release workflow | PATCH          | `1.0.0 -> 1.0.1` |

HIGHLY RECOMMENDED: USE POST-COMMIT SCRIPT LOCALLY

Create file `.git/hooks/post-commit`

```bash
#!/bin/sh
dotnet tool install -g DeployItSmart
depitsmart LocalTrashCommitCleanerRunner
```

Execute in shell to activate the git hook on that local project `git config core.hooksPath .git/hooks`.

Note: this only activate locally each user on each repos must activate it this way unless you setup a global hook which in not recommended.

Note: it is of your responsability to ensure clear branch history prior to merge whether or not you are using the post-commit.

Check out [DeployItSmart](https://github.com/mshimshon/DeployItSmart) for more details.

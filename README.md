# C-Sweet Work Management Contracts

`CSweet.WorkManagement.Contracts` is the dependency-light .NET wire contract shared by the
C-Sweet platform broker and `CSweet.Agent.SDK`.

It contains:

- canonical agent-facing `work.*` capability names;
- request and response records used by the typed work client and broker; and
- stable string constants for item kinds, priorities, automation operations, and triggers.

Keep platform domain entities, persistence types, authorization services, and SDK runtime code out
of this package. Adding or changing a wire member is a protocol change and should include
compatibility tests in both the SDK and platform repositories.

## Build and package

Run the packaging script from a Developer Command Prompt or PowerShell:

```powershell
.\Create-NuGetPackages.bat
```

Pass an optional package version and output root:

```powershell
.\Create-NuGetPackages.bat 1.0.1 C:\packages
```

Packages are written to `artifacts\packages\<version>` by default.

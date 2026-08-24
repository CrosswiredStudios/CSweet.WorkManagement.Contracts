# C-Sweet Work Management Contracts

Version 3.9 adds technical delegation recommendations, correlated work-item comments, and public
orchestration read/retry contracts for continuous agent collaboration. Version 3.7 adds grant-governed board metadata configuration through
`work.board.configure`, including optimistic concurrency and idempotency fields.

Version 3.5 adds durable in-progress waiting. The SDK can release a transient execution claim
while leaving the personal item in `Running`/Doing until an external event resumes it. Version 3.4
added sequenced personal work through `Backlog` and explicit activation. Personal items also expose
their non-secret correlation identifier.
The platform still enforces board, team, repository, item, and personal-board grants server-side.

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

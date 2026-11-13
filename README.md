# C-Sweet Work Management Contracts

Version 5.6.0 adds the browser-scenarios receipt (`WorkItemVerificationKinds.BrowserScenariosReceiptCommand`, `WorkItemVerifications.IsBrowserScenariosReceipt`, `ScenarioRecords`) that the author of a `browser-interaction` ticket must carry. Version 5.5.0 adds live browser-interaction verification (`browser-interaction` with `CriterionIndexes`) and `WorkExecutionDiagnostics` for routing Blocked outcomes. Version 5.4.0 adds per-ticket verification requirements (`WorkItemPlanningSpecification.Verifications`). Version 5.3.0 defines ticket-owned workflows, declared predecessors, assigned execution steps and explicit source branch initialization. Delivery plans require fresh workflow tickets; the platform does not synthesize aggregate review stages or migrate legacy executions.
Version 4.0.0 replaces the parallel execution APIs with one scope-aware
`WorkExecutionAssignment` and the `work.execution.run` capability. Task assignments
retain sprint authority; aggregate reviews retain delivery-plan authority. Upgrade
the platform, SDK and executing agents together. Old execution envelopes are not
translated or accepted as the new capability.

Version 3.29.0 added `WorkArtifactDelivery`, the shared exact-document author output.
Use `ToOutput()` to write canonical `artifactId`, `revisionId`, `sha256` and
`remainingRisks` fields. `TryRead()` accepts existing PascalCase and mixed-case
persisted outcomes, validates nonempty identities and hexadecimal SHA-256 digests,
and rejects duplicate identity fields. Unknown metadata remains forward compatible.

Version 3.11 adds platform-owned work-item types and board profiles, planning revisions, reusable
approval policies, proposal provenance, and architecture-review events. Version 3.9 adds technical delegation recommendations, correlated work-item comments, and public
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

Version 3.21.0 adds typed personal development plans on the existing Epic → Story → Task hierarchy. Personal tickets expose kind, parent, plan root, execution phase, and acceptance criteria. CreatePersonalWorkPlanRequest submits the entire bounded backlog atomically; ReportPersonalWorkPlanTaskRequest carries revision-checked task progress and evidence. PersonalWorkPlanLink fits the existing planning specification without changing existing assignment fields or requiring a new hierarchy model.

ConfigureWorkDeliveryPlanRequest.TaskAssignments (3.29.0) optionally carries draft task staffing proposals. Proposals are advisory draft input; activation still requires authorized finalized task assignments and delivery metadata. Legacy requests deserialize with an empty proposal collection.

Forecast-driven planning exposes additive completion ranges, elapsed-history sample counts, unestimated and blocked item counts, generation time and basis through `WorkSprintForecast`. Missing dates never prevent planning. Forecasts do not grant spending, acceptance or release authority.


5.6.0 adds `WorkItemVerificationKinds.BrowserScenariosReceiptCommand` (`node .csweet/browser-scenarios.cjs <build> [--record <path.md>]...`), `ScenarioQueryParameter` (`csweet-scenarios`), `WorkItemVerifications.IsBrowserScenariosReceipt` and `ScenarioRecords`. The author of a `browser-interaction` ticket runs the shared scenario runner (CSweet.Agent.SDK 5.25.0) on the built output. The runner, not the page, judges each scenario check from its expected and observed values, and regenerates the measured section of every named observation record. The technical reviewer requires the receipt and QA repeats the same run independently.

5.5.0 adds `WorkItemVerificationKinds.BrowserInteraction` (`browser-interaction`). Its `CriterionIndexes` name, by zero-based index, the acceptance criteria that need live interaction evidence; criteria not named need none. `WorkItemVerifications.ValidatePlanning(planning)` also checks the indexes against the ticket's criteria and requires a browser-smoke requirement alongside. `WorkExecutionDiagnostics` holds the cross-agent tokens on Blocked outcomes: `decision-required:v1` (only a person can clear it) and `agent-defect:v1` (the agent's own processing failed; a retry of the same version is not expected to help).

5.4.0 adds `WorkItemVerificationRequirement`, `WorkItemVerificationKinds` (`browser-smoke`) and `WorkItemVerifications`. `WorkItemPlanningSpecification.Verifications` is `null` on tickets planned before 5.4.0 and an explicit list afterwards; an empty list means planning declared no special verification. The author, the technical reviewer and QA read this one declaration instead of inferring browser evidence from criteria text.

5.3.0 adds `WorkDeliveryCorrection`, `CoordinateWorkDeliveryCorrectionRequest`, plan correction state, and the manager-scoped `work.delivery.correction.coordinate.v1` capability. Correction handoffs are independent of sprint closure and incident presentation.

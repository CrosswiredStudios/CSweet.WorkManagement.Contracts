# C-Sweet Work Management Contracts

Version 3.13 adds generic, profile-driven Workstreams, portfolio supervision, authority envelopes,
milestone gates, correlated decisions, structured review findings, authenticated work context, and
generic delivery-evidence capability and event names. Domain-specific project types remain in
separate vertical contract packages.

Version 3.11 adds platform-owned work-item types and board profiles, planning revisions, reusable
approval policies, proposal provenance, and architecture-review events. Version 3.9 adds technical delegation recommendations, correlated work-item comments, and public
orchestration read/retry contracts. Version 3.6 adds repository-independent work-item planning specifications and idempotent delivery finalization.

Canonical capability names and wire DTOs shared by the C-Sweet platform broker and
`CSweet.Agent.SDK`.

This package intentionally has no dependency on platform domain, persistence, authorization, or
agent runtime assemblies.

Version 3.5 includes durable in-progress waiting for personal work. Version 3.4 added sequenced
Backlog activation, while version 3.3 introduced personal queues and structured, field-scoped
work-item mention spans in the shared agent/platform protocol.

Version 3.16.0 adds optional validated assignment replacement to planning revisions, allowing existing backlog tickets to acquire an owner as staffing arrives. Omitted assignment data preserves existing ownership.

Version 3.17.0 adds AllowedOutcomeCodes to canonical execution input, so agents can select outcomes supported by their immutable stage policy. Older payloads deserialize with an empty list.

Version 3.18.0 adds the canonical orchestration approval decision capability for authorized agent board managers.

Version 3.19.0 adds optional LatestOutcome to orchestration stage reads for delivery review. Older responses omit it.

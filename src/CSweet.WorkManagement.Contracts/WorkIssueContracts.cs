namespace CSweet.WorkManagement.Contracts;

/// <summary>Cases coordinate responsibility; linked tasks, decisions and reviews retain their own authority.</summary>
public static class WorkIssueCapabilities
{
    public const string Read = "work.issues.read.v1";
    public const string Reconcile = "work.issues.reconcile.v1";
    public const string Commission = "work.issues.commission.v1";
    public const string Propose = "work.issues.propose.v1";
    public const string DecideProposal = "work.issues.proposal.decide.v1";
    public static IReadOnlyList<string> All { get; } = [Read, Reconcile, Commission, Propose, DecideProposal];
}
public static class WorkIssueEvents { public const string Changed = "work.issues.changed.v1"; }

public sealed record WorkIssueLink(string Kind, Guid Id, string? Digest = null);
public sealed record WorkIssueUpdate(string ObligationKey, string Statement, string Detail, IReadOnlyList<string> ScopeKeys)
{
    public Guid? IssueId { get; init; }
    public long? ExpectedRevision { get; init; }
    public string Status { get; init; } = "Open";
    public string Phase { get; init; } = "Working";
    public string NextAction { get; init; } = "Reconcile the planning responsibility";
    public IReadOnlyList<WorkIssueLink> Links { get; init; } = [];
    public string? ResolutionEvidence { get; init; }
    public WorkIssueLink? ReopeningEvidence { get; init; }
    public string? BriefTitle { get; init; }
    public string? BriefSummary { get; init; }
}
public sealed record ReconcileWorkIssuesRequest(Guid BoardId, WorkApprovedSource Source,
    IReadOnlyList<WorkIssueUpdate> Issues, string IdempotencyKey);
public sealed record ReadWorkIssuesRequest(Guid BoardId, bool IncludeHistory = false, int Page = 1, int PageSize = 50);
public sealed record WorkIssueCaseResponse(Guid Id, Guid BoardId, Guid WorkstreamId, string ObligationKey,
    string Statement, string Detail, string Status, string Phase, Guid AccountableEmployeeId, string NextAction,
    WorkApprovedSource Source, IReadOnlyList<string> ScopeKeys, IReadOnlyList<WorkIssueLink> Links,
    string? ResolutionEvidence, long Revision, DateTimeOffset UpdatedAt)
{
    public int Episode { get; init; } = 1;
    public Guid? PreviousIssueId { get; init; }
    public string? BriefTitle { get; init; }
    public string? BriefSummary { get; init; }
}
public sealed record WorkIssueCommissionResponse(Guid Id, Guid BoardId, Guid TargetEmployeeId, IReadOnlyList<Guid> IssueIds,
    WorkApprovedSource Source, string Status, Guid? SessionId, string? Failure, long Revision)
{
    public Guid? RequestedByEmployeeId { get; init; }
    public string? DecisionReason { get; init; }
    /// <summary>Private consultation provenance, returned only to its original contributing participant.</summary>
    public IReadOnlyList<Guid>? OriginSessionIds { get; init; }
    public IReadOnlyList<WorkIssueUpdate>? ProposedChanges { get; init; }
}
public sealed record WorkIssueDirectory(IReadOnlyList<WorkIssueCaseResponse> Issues,
    IReadOnlyList<WorkIssueCommissionResponse> Commissions, bool HasMore = false)
{
    public bool? CanReconcile { get; init; }
    public Guid? AccountableEmployeeId { get; init; }
}
public sealed record WorkIssueRevisionReference(string ObligationKey, Guid? IssueId, long? Revision);
public sealed record DecideWorkIssueProposalRequest(Guid ProposalId, long ExpectedRevision, bool Accept, string Reason, string IdempotencyKey)
{
    /// <summary>Current records shown to the owner for this judgment. Omit only when using the original proposal's revision bindings.</summary>
    public IReadOnlyList<WorkIssueRevisionReference>? ReviewedIssues { get; init; }
}
public sealed record CommissionWorkIssuesRequest(Guid BoardId, Guid TargetEmployeeId, IReadOnlyList<Guid> IssueIds,
    WorkApprovedSource Source, string Question, string IdempotencyKey)
{
    /// <summary>Prior source-bound consultations whose responsibility this commission carries forward. History is retained.</summary>
    public IReadOnlyList<Guid> SupersededSessionIds { get; init; } = [];
}

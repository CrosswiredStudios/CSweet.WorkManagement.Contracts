namespace CSweet.WorkManagement.Contracts;

public static class DelegationCapabilities
{
    public static readonly IReadOnlyList<string> All = [Create, Read, Respond, Publish, Cancel, Follow, Unfollow, ReadTracking, Progress, Pending, Processed];
    public const string Create = "work.delegation.create.v1";
    public const string Read = "work.delegation.read.v1";
    public const string Respond = "work.delegation.respond.v1";
    public const string Publish = "work.delegation.publish.v1";
    public const string Cancel = "work.delegation.cancel.v1";
    public const string Follow = "work.tracking.follow.v1";
    public const string Unfollow = "work.tracking.unfollow.v1";
    public const string ReadTracking = "work.tracking.read.v1";
    public const string Progress = "work.tracking.progress.v1";
    public const string Pending = "work.tracking.pending.v1";
    public const string Processed = "work.tracking.processed.v1";
    public const string Received = "com.csweet.work.delegation.received.v1";
    public const string Changed = "com.csweet.work.tracking.changed.v1";
    public const string Execution = "com.csweet.work.delegation.execute.v1";
}

public sealed record DelegationContext(Guid? WorkstreamId = null, Guid? TeamId = null, Guid? WorkItemId = null);
public sealed record DelegateWithTrackingRequest(Guid RecipientOrganizationUserId, string Instruction,
    string IntendedOutcome, Guid SourceConversationId, Guid SourceMessageId, string IdempotencyKey,
    IReadOnlyList<Guid>? InterestedParties = null, DelegationContext? WorkContext = null,
    Guid? ParentDelegationId = null);
public sealed record ReadDelegationRequest(Guid? DelegationId = null, Guid? ExecutionMessageId = null);
public sealed record DelegationResponseRequest(Guid DelegationId, long ExpectedRevision, string Disposition,
    string Summary, string IdempotencyKey, Guid? WorkItemId = null);
public sealed record PublishDelegationReportRequest(Guid DelegationId, Guid UpdateId, string Summary, string IdempotencyKey);
public sealed record CancelDelegationRequest(Guid DelegationId, long ExpectedRevision, string IdempotencyKey);
public sealed record FollowWorkRequest(Guid WorkItemId, string IdempotencyKey);
public sealed record ReadWorkTrackingRequest(Guid WorkItemId);
public sealed record WorkProgressRequest(Guid WorkItemId, long ExpectedRevision, string Kind, string Summary,
    string IdempotencyKey, Guid? DelegationId = null);
public sealed record PendingTrackingRequest(int Limit = 20);
public sealed record ProcessTrackingUpdateRequest(Guid WorkItemId, Guid UpdateId);
public sealed record DelegationWake(Guid DelegationId);
public sealed record WorkTrackingWake(Guid WorkItemId, Guid? DelegationId = null);
public sealed record DelegationSnapshot(Guid Id, long Revision, Guid OriginalRequesterId, Guid CoordinatorId,
    Guid RecipientId, string CoordinatorName, string RecipientName, string Instruction, string IntendedOutcome,
    string Status, bool IsDirective, Guid ReportingConversationId, Guid RecipientConversationId,
    Guid RecipientMessageId, Guid? WorkItemId, string? WorkItemUrl, Guid CommitmentItemId,
    Guid? ParentDelegationId, DelegationContext? WorkContext, DateTimeOffset CreatedAt,
    IReadOnlyList<Guid> InterestedParties, IReadOnlyList<TrackingUpdate> Updates)
{
    public Guid? ExecutionTurnId { get; init; }
    public Guid? ExecutionMessageId { get; init; }
    public Guid? WorkBoardId { get; init; }
    public bool ExecutionHandled { get; init; }
    /// <summary>Stopping reporting does not withdraw the execution request.</summary>
    public bool TrackingCancelled { get; init; }
    public string? ExecutionStatus { get; init; }
    public Guid AuthoredByOrganizationUserId { get; init; }
    public Guid InstructionAuthorId { get; init; }
    public Guid SourceConversationId { get; init; }
    public Guid SourceMessageId { get; init; }
    public string? SourceRevisionHash { get; init; }
    public DelegationExecutionEvidence? ExecutionEvidence { get; init; }
}
public sealed record DelegationExecutionEvidence(Guid TurnId, Guid ResponseMessageId, string Status, string Content);

/// <summary>Typed recipient decision. Acceptance binds work without claiming that the outcome is achieved.</summary>
public abstract record DelegationResponse(string Summary)
{
    public sealed record Acceptance(string IntendedAction, Guid? WorkItemId = null) : DelegationResponse(IntendedAction);
    public sealed record Clarification(string Question) : DelegationResponse(Question);
    public sealed record Decline(string Reason) : DelegationResponse(Reason);
    public sealed record Progress(string Update, Guid? WorkItemId = null) : DelegationResponse(Update);
    public sealed record Blocker(string RequiredDecision) : DelegationResponse(RequiredDecision);
    public sealed record FinalOutcome(string Result, bool Succeeded) : DelegationResponse(Result);
}
public sealed record TrackingUpdate(Guid Id, Guid WorkItemId, Guid? DelegationId, long Revision,
    string Kind, string Summary, DateTimeOffset OccurredAt, DateTimeOffset AvailableAt, bool Reported = false);
public sealed record WorkTrackingSnapshot(Guid WorkItemId, bool Following, bool Limited,
    string? WorkItemUrl, IReadOnlyList<TrackingUpdate> Updates);
public sealed record PendingTrackingUpdate(Guid? DelegationId, Guid WorkItemId, TrackingUpdate Update);

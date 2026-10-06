using System.Text.Json;

namespace CSweet.WorkManagement.Contracts;

public static class WorkDeliveryCapabilities
{
    public const string Read = "work.delivery.read.v1";
    public const string Configure = "work.delivery.configure.v1";
    public const string Control = "work.delivery.control.v1";
    public const string Accept = "work.delivery.accept.v1";
    public const string Recover = "work.delivery.recover.v1";
    public const string Evidence = "work.delivery.evidence.read.v1";
    public const string Review = "work.delivery.review.complete.v1";
    public const string Changed = "com.csweet.work-delivery.changed.v1";
    public static IReadOnlyList<string> All { get; } = [Read, Configure, Control, Accept, Recover, Evidence, Review];
}

public static class WorkExecutionScopes
{
    public const string Task = "Task";
    public const string Story = "Story";
    public const string Epic = "Epic";
    public const string Release = "Release";
}

/// <summary>Repository branches are owned by a delivery scope, never by a caller's workspace.</summary>
public sealed record WorkDeliveryBranchBinding(Guid RepositoryId, string Scope, Guid? ItemId,
    string SourceBranch, string TargetBranch)
{
    public WorkDeliveryBuildSpecification? Build { get; init; }
}
public sealed record WorkDeliveryBuildSpecification(Guid ToolchainDefinitionId, Guid ProviderInstallationId,
    string RecipeKey, string TargetKey, JsonElement Configuration);
public sealed record WorkDeliveryScopeAssignment(string Scope, Guid? ItemId,
    Guid BoardId, IReadOnlyList<WorkStageAssignment> Stages);
public sealed record WorkDeliveryScopeSnapshot(string Scope, Guid? ItemId, Guid BoardId,
    long PlanningRevision, IReadOnlyList<Guid> ChildIds, WorkItemPlanningSpecification Planning,
    IReadOnlyList<WorkStageAssignment> Stages)
{
    public IReadOnlyDictionary<Guid, long> ChildPlanningRevisions { get; init; } = new Dictionary<Guid, long>();
    public IReadOnlyDictionary<Guid, string> ChildDeliveryDigests { get; init; } = new Dictionary<Guid, string>();
}
public sealed record ConfigureWorkDeliveryPlanRequest(Guid WorkstreamId, string Name,
    Guid ManagerOrganizationUserId, IReadOnlyList<Guid> EpicItemIds,
    IReadOnlyList<WorkDeliveryBranchBinding> Branches,
    IReadOnlyList<WorkDeliveryScopeAssignment> Assignments, string IdempotencyKey)
{
    public Guid? PlanId { get; init; }
    public long ExpectedRevision { get; init; }
    public WorkItemPlanningSpecification? ReleasePlanning { get; init; }
}
public sealed record ReadWorkDeliveryPlansRequest(Guid WorkstreamId, Guid? PlanId = null,
    int Limit = 50, Guid? AfterId = null);
public sealed record ControlWorkDeliveryPlanRequest(Guid PlanId, long ExpectedRevision,
    string Action, string IdempotencyKey, string? Reason = null);
public sealed record DecideWorkDeliveryAcceptanceRequest(Guid PlanId, Guid ExecutionId,
    long ExpectedRevision, string CandidateDigest, bool Approved, string Summary,
    IReadOnlyList<WorkDeliveryCriterionResult> Criteria, IReadOnlyList<string> Findings,
    string IdempotencyKey);
public sealed record RecoverWorkDeliveryRequest(Guid PlanId, Guid ExecutionId,
    long ExpectedRevision, string IdempotencyKey, string Reason)
{
    public IReadOnlyList<WorkDeliveryFindingResolution> Resolutions { get; init; } = [];
}
public sealed record WorkDeliveryFindingResolution(Guid FindingId, IReadOnlyList<Guid> RemediationTaskIds, string Evidence);
public sealed record WorkDeliveryFindingResponse(Guid Id, Guid ExecutionId, string CandidateDigest, string Summary,
    string Status, IReadOnlyList<Guid> RemediationTaskIds, string? ResolutionEvidence);
public sealed record CompleteWorkDeliveryReviewRequest(Guid PlanId, Guid ExecutionId,
    Guid StageExecutionId, long ExpectedRevision, WorkDeliveryReviewResult Result, string IdempotencyKey);
public sealed record WorkDeliveryCriterionResult(string Criterion, bool Satisfied, string Evidence);
public sealed record WorkArtifactQualityResult(Guid ArtifactId, Guid RevisionId, string Sha256,
    bool Passed, string Summary, IReadOnlyList<WorkDeliveryCriterionResult> Criteria, IReadOnlyList<string> Findings);
public sealed record WorkDeliveryRepositoryCandidate(Guid RepositoryId, string SourceBranch,
    string TargetBranch, string SourceCommitSha, string TargetCommitSha,
    string CandidateCommitSha, Guid? BuildId = null);
public sealed record WorkDeliveryDocumentCandidate(Guid ItemId, Guid ArtifactId,
    Guid RevisionId, string Sha256);
public sealed record WorkDeliveryCandidate(string Digest, long ScopeRevision,
    IReadOnlyList<WorkDeliveryRepositoryCandidate> Repositories,
    IReadOnlyList<WorkDeliveryDocumentCandidate> Documents);
public sealed record WorkDeliveryReviewResult(string CandidateDigest, bool Approved, string Summary,
    IReadOnlyList<WorkDeliveryCriterionResult> Criteria, IReadOnlyList<string> Findings)
{
    public IReadOnlyList<WorkDeliveryValidationEvidence> Validations { get; init; } = [];
    public IReadOnlyList<Guid> BuildIds { get; init; } = [];
}
public sealed record WorkDeliveryBuildPending(string CandidateDigest, IReadOnlyList<Guid> BuildIds);
public sealed record WorkDeliveryValidationEvidence(Guid RepositoryId, string CommitSha, string Command,
    int ExitCode, bool Succeeded, string Output);
public sealed record ReadWorkDeliveryEvidenceRequest(Guid PlanId, Guid ExecutionId, Guid? RepositoryId = null);
public sealed record WorkDeliveryDocumentContent(Guid ArtifactId, Guid RevisionId, string Sha256, string Content);
public sealed record WorkDeliveryEvidenceResponse(WorkDeliveryCandidate Candidate,
    IReadOnlyList<WorkDeliveryDocumentContent> Documents, byte[]? Archive = null,
    string? ArchiveSha256 = null, string? CommitSha = null)
{
    public IReadOnlyList<DeliveryBuildV2> Builds { get; init; } = [];
    public IReadOnlyList<WorkDeliveryChildEvidence> Children { get; init; } = [];
}
public sealed record WorkDeliveryChildEvidence(Guid ItemId, long PlanningRevision, string DeliveryKind,
    WorkExecutionOutcomeV1? TechnicalReview, WorkExecutionOutcomeV1? Quality, string? IntegratedCommitSha);
public sealed record WorkDeliveryPromotionReceipt(Guid RepositoryId, string SourceCommitSha,
    string TargetCommitSha, string Status, string? MergeCommitSha, string? Error);
public sealed record WorkDeliveryExecutionResponse(Guid Id, string Scope, Guid? ItemId,
    Guid BoardId, string Status, string CurrentStageKey, long Revision,
    WorkDeliveryCandidate? Candidate, string? BlockedReason,
    IReadOnlyList<WorkStageExecutionResponse> Stages,
    IReadOnlyList<WorkDeliveryPromotionReceipt> Promotions);
public sealed record WorkDeliveryPlanResponse(Guid Id, Guid WorkstreamId, string Name,
    Guid ManagerOrganizationUserId, string Status, long Revision, long ScopeRevision,
    IReadOnlyList<Guid> EpicItemIds, IReadOnlyList<WorkDeliveryBranchBinding> Branches,
    IReadOnlyList<WorkDeliveryScopeSnapshot> Scopes,
    IReadOnlyList<WorkDeliveryExecutionResponse> Executions,
    DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt)
{
    public IReadOnlyList<WorkDeliveryFindingResponse> Findings { get; init; } = [];
}

/// <summary>A real sprint identity exists only for Task scope. Aggregate reviews use DeliveryExecutionId.</summary>
public sealed record WorkExecutionAssignmentV2(Guid ExecutionId, Guid StageExecutionId,
    Guid AttemptId, Guid OrganizationId, Guid WorkstreamId, Guid BoardId, Guid? ItemId,
    string Scope, Guid? DeliveryPlanId, long ScopeRevision, Guid? SprintExecutionId,
    Guid? SprintId, long AssignmentRevision, string Identifier, Guid? PolicyRevisionId,
    string StageKey, int Traversal, int Attempt, DateTimeOffset Deadline,
    string Instructions, JsonElement Item, JsonElement Input,
    IReadOnlyList<WorkExecutionOutcomeV1> PriorOutcomes,
    IReadOnlyList<WorkExecutionEvidence> Evidence, WorkDeliveryCandidate? Candidate = null)
{
    public Guid OrganizationUserId { get; init; }
    public Guid? AgentInstallationId { get; init; }
    public long PlanningRevision { get; init; }
    public IReadOnlyList<string> PermittedOutcomes { get; init; } = [];
    public static WorkExecutionAssignmentV2 FromTask(WorkExecutionAssignmentV1 task,
        Guid projectId, Guid? planId, long scopeRevision) => new(task.ItemExecutionId,
        task.StageExecutionId, task.AttemptId, task.OrganizationId, projectId, task.BoardId,
        task.ItemId, WorkExecutionScopes.Task, planId, scopeRevision, task.SprintExecutionId,
        task.SprintId, task.AssignmentRevision, task.ItemIdentifier, task.PolicyRevisionId,
        task.StageKey, task.Traversal, task.Attempt, task.Deadline, task.Instructions,
        task.Item, task.Input, task.PriorOutcomes, task.Evidence);

    public WorkExecutionAssignmentV1 ToTaskAssignment()
    {
        if (Scope != WorkExecutionScopes.Task || SprintExecutionId is not { } execution ||
            SprintId is not { } sprint || ItemId is not { } item || PolicyRevisionId is not { } policy)
            throw new InvalidOperationException("An aggregate assignment is not sprint work.");
        return new(execution, ExecutionId, StageExecutionId, AttemptId, OrganizationId,
            BoardId, sprint, item, AssignmentRevision, Identifier.Split('-')[0], Identifier,
            policy, StageKey, Traversal, Attempt, Deadline, Instructions, Item,
            Input, PriorOutcomes, Evidence);
    }
}

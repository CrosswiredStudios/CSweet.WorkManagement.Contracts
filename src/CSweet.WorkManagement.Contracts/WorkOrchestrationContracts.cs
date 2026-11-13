using System.Text.Json;

namespace CSweet.WorkManagement.Contracts;

public static class WorkOrchestrationStageTypes
{
    public const string Queue = "Queue";
    public const string AgentExecution = "AgentExecution";
    public const string ManualWork = "ManualWork";
    public const string ManagerApproval = "ManagerApproval";
    public const string TrustedPlatformAction = "TrustedPlatformAction";
    public const string Terminal = "Terminal";
}

public static class WorkExecutionDispositions
{
    public const string Completed = "Completed";
    public const string Blocked = "Blocked";
    public const string Failed = "Failed";
}

/// <summary>
/// Cross-agent diagnostics on a <see cref="WorkExecutionOutcomeV1"/>. Each token says who can clear a Blocked outcome,
/// so a manager agent routes it without parsing localized summary text.
/// </summary>
public static class WorkExecutionDiagnostics
{
    /// <summary>
    /// Only a person can clear this: a scope, criteria, environment or tooling decision. A plain retry changes nothing
    /// until the decision is made; the summary states the decision needed.
    /// </summary>
    public const string DecisionRequired = "decision-required:v1";

    /// <summary>
    /// The executing agent hit a defect in its own processing (an internal invariant, contract or state error), not a
    /// problem with the work product. A retry of the same agent version is not expected to help; the agent needs an
    /// update or an operator's investigation. The summary carries the agent's exact reason.
    /// </summary>
    public const string AgentDefect = "agent-defect:v1";
}

public static class WorkMergeModes
{
    public const string ManagerApproval = "ManagerApproval";
    public const string Automatic = "Automatic";
}

public static class WorkOrchestrationPrincipalKinds
{
    public const string Unassigned = "Unassigned";
    public const string Human = "Human";
    public const string AgentInstallation = "AgentInstallation";
    public const string BoardManager = "BoardManager";
    public const string PlatformAction = "PlatformAction";
}

public sealed record WorkOrchestrationRetryPolicy(
    int MaximumAttempts = 5,
    int InitialDelaySeconds = 10,
    int MaximumDelaySeconds = 300);

public sealed record WorkOrchestrationStageDefinition(
    string Key,
    string Name,
    string StageType,
    Guid? ColumnId,
    string Instructions,
    string InputSchemaJson,
    string OutputSchemaJson,
    int TimeoutSeconds,
    int? ConcurrencyLimit,
    WorkOrchestrationRetryPolicy RetryPolicy,
    string? PlatformAction = null,
    bool IsSuccessfulTerminal = false);

public sealed record WorkOrchestrationTransitionDefinition(
    string FromStageKey,
    string OutcomeCode,
    string ToStageKey,
    int? MaximumTraversals = null);

public sealed record WorkOrchestrationConcurrencyLimits(
    int Global,
    int Organization,
    int Board,
    int DefaultStage,
    int DefaultAssignee);

public sealed record WorkOrchestrationPolicyRevision(
    Guid PolicyId,
    Guid RevisionId,
    Guid BoardId,
    int Revision,
    string Name,
    string InitialStageKey,
    string MergeMode,
    WorkOrchestrationConcurrencyLimits Concurrency,
    IReadOnlyList<WorkOrchestrationStageDefinition> Stages,
    IReadOnlyList<WorkOrchestrationTransitionDefinition> Transitions,
    bool IsPublished,
    DateTimeOffset CreatedAt,
    DateTimeOffset? PublishedAt);

public sealed record ConfigureSoftwareOrchestrationTemplateRequest(
    Guid BoardId,
    Guid ReadyColumnId,
    Guid DevelopmentColumnId,
    Guid DevCompleteColumnId,
    Guid QualityColumnId,
    Guid ReadyToMergeColumnId,
    Guid DoneColumnId,
    string MergeMode,
    int MaximumQualityCycles,
    string IdempotencyKey);

/// <summary>Applies the immutable workflow contributed by a pinned workstream profile.</summary>
public sealed record ConfigureProfileOrchestrationRequest(
    Guid WorkstreamId,
    Guid BoardId,
    long ExpectedBoardRevision,
    string ProfileDefinitionDigest,
    string IdempotencyKey);

public sealed record ConfigureProfileOrchestrationResponse(
    Guid WorkstreamId,
    Guid BoardId,
    long BoardRevision,
    WorkOrchestrationPolicyRevision Policy,
    IReadOnlyList<WorkBoardColumn> Columns);

public sealed record WorkStageAssignment(
    string StageKey,
    string PrincipalKind,
    Guid? OrganizationUserId = null,
    Guid? AgentInstallationId = null,
    string? PlatformAction = null)
{
    public WorkAssignmentRequirements? Requirements { get; init; }
    public WorkAssignmentSelectionEvidence? SelectionEvidence { get; init; }
}

public sealed record WorkExecutionEvidence(
    string Kind,
    string Name,
    string Value,
    string? ContentType = null);

/// <summary>Canonical, domain-neutral planning input supplied to an assigned worker.</summary>
public sealed record WorkExecutionInputV1(
    Guid? WorkstreamId,
    Guid? TeamId,
    long PlanningRevision,
    WorkItemPlanningSpecification? Planning)
{
    /// <summary>Outcome codes accepted by the immutable execution policy for this stage.</summary>
    public IReadOnlyList<string> AllowedOutcomeCodes { get; init; } = [];
    public WorkAssignmentRequirements? AssignmentRequirements { get; init; }
    public WorkAssignmentSelectionEvidence? AssignmentSelection { get; init; }
}

public sealed record WorkExecutionOutcomeV1(
    Guid StageExecutionId,
    Guid AttemptId,
    string Disposition,
    string OutcomeCode,
    string Summary,
    JsonElement Output,
    IReadOnlyList<WorkExecutionEvidence> Evidence,
    IReadOnlyList<string> Diagnostics);

public sealed record WorkOrchestrationValidationError(
    string Code,
    string Message,
    Guid? ItemId = null,
    string? StageKey = null,
    Guid? AssignmentId = null);

public sealed record WorkSprintPreflightResult(
    bool IsValid,
    Guid BoardId,
    Guid SprintId,
    Guid? PolicyRevisionId,
    IReadOnlyList<WorkOrchestrationValidationError> Errors);

public sealed record StartWorkSprintExecutionRequest(
    Guid BoardId,
    Guid SprintId,
    long ExpectedSprintRevision,
    string IdempotencyKey);

public sealed record ControlWorkSprintExecutionRequest(
    Guid BoardId,
    Guid SprintId,
    long ExpectedSprintRevision,
    string IdempotencyKey,
    string? Reason = null);

public sealed record RetryWorkStageExecutionRequest(
    Guid BoardId,
    Guid SprintExecutionId,
    Guid StageExecutionId,
    string IdempotencyKey,
    string? Reason = null)
{
    /// <summary>The immutable assignment revision observed by the caller.</summary>
    public long ExpectedAssignmentRevision { get; init; }
}

public sealed record ReadWorkOrchestrationRequest(
    Guid BoardId,
    Guid? SprintId = null,
    Guid? SprintExecutionId = null);

public sealed record WorkStageExecutionResponse(
    Guid Id,
    string StageKey,
    string StageType,
    int Traversal,
    string Status,
    string PrincipalKind,
    Guid? OrganizationUserId,
    Guid? AgentInstallationId,
    string? PlatformAction,
    int AttemptCount,
    string? LastOutcomeCode,
    string? LastSummary,
    string? LastError,
    DateTimeOffset? RetryAt,
    DateTimeOffset UpdatedAt)
{
    public long AssignmentRevision { get; init; }
    public int MaximumAttempts { get; init; }
    /// <summary>Validated result of this completed stage's latest attempt, if available.</summary>
    public WorkExecutionOutcomeV1? LatestOutcome { get; init; }
}

public sealed record WorkItemExecutionResponse(
    Guid Id,
    Guid WorkItemId,
    string ItemIdentifier,
    string CurrentStageKey,
    int Traversal,
    string Status,
    string? BlockedReason,
    IReadOnlyList<WorkStageExecutionResponse> Stages,
    DateTimeOffset UpdatedAt);

public sealed record WorkSprintExecutionResponse(
    Guid Id,
    Guid BoardId,
    Guid SprintId,
    Guid PolicyRevisionId,
    Guid StartedByOrganizationUserId,
    string Status,
    long Revision,
    DateTimeOffset StartedAt,
    DateTimeOffset UpdatedAt,
    DateTimeOffset? CompletedAt,
    IReadOnlyList<WorkItemExecutionResponse> Items);

public sealed record CompleteManualWorkStageRequest(
    Guid BoardId,
    Guid SprintExecutionId,
    Guid StageExecutionId,
    string OutcomeCode,
    string Summary,
    JsonElement Output,
    string IdempotencyKey)
{
    public IReadOnlyList<WorkExecutionEvidence> Evidence { get; init; } = [];
}

public sealed record DecideWorkApprovalStageRequest(
    Guid BoardId,
    Guid SprintExecutionId,
    Guid StageExecutionId,
    bool Approved,
    string Summary,
    string IdempotencyKey);

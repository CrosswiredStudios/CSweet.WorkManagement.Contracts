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

public static class WorkMergeModes
{
    public const string ManagerApproval = "ManagerApproval";
    public const string Automatic = "Automatic";
}

public static class WorkOrchestrationPrincipalKinds
{
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

public sealed record WorkStageAssignment(
    string StageKey,
    string PrincipalKind,
    Guid? OrganizationUserId = null,
    Guid? AgentInstallationId = null,
    string? PlatformAction = null);

public sealed record WorkExecutionEvidence(
    string Kind,
    string Name,
    string Value,
    string? ContentType = null);

public sealed record WorkExecutionAssignmentV1(
    Guid SprintExecutionId,
    Guid ItemExecutionId,
    Guid StageExecutionId,
    Guid AttemptId,
    Guid OrganizationId,
    Guid BoardId,
    Guid SprintId,
    Guid ItemId,
    long AssignmentRevision,
    string BoardKey,
    string ItemIdentifier,
    Guid PolicyRevisionId,
    string StageKey,
    int Traversal,
    int Attempt,
    DateTimeOffset Deadline,
    string Instructions,
    JsonElement Item,
    JsonElement Input,
    IReadOnlyList<WorkExecutionOutcomeV1> PriorOutcomes,
    IReadOnlyList<WorkExecutionEvidence> Evidence);

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
    string? Reason = null);

public sealed record CompleteManualWorkStageRequest(
    Guid BoardId,
    Guid SprintExecutionId,
    Guid StageExecutionId,
    string OutcomeCode,
    string Summary,
    JsonElement Output,
    string IdempotencyKey);

public sealed record DecideWorkApprovalStageRequest(
    Guid BoardId,
    Guid SprintExecutionId,
    Guid StageExecutionId,
    bool Approved,
    string Summary,
    string IdempotencyKey);

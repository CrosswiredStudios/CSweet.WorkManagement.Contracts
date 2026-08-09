namespace CSweet.WorkManagement.Contracts;

/// <summary>Canonical capability names for the agent-facing work-management protocol.</summary>
public static class WorkManagementCapabilityNames
{
    public const string ExecutionRunV1 = "work.execution.run.v1";
    public const string OrchestrationRead = "work.orchestration.read";
    public const string OrchestrationPreflight = "work.orchestration.preflight";
    public const string OrchestrationStart = "work.orchestration.start";
    public const string OrchestrationPause = "work.orchestration.pause";
    public const string OrchestrationResume = "work.orchestration.resume";
    public const string OrchestrationCancel = "work.orchestration.cancel";
    public const string OrchestrationRetry = "work.orchestration.retry";
    public const string BoardRead = "work.board.read";
    public const string BoardCreate = "work.board.create";
    public const string BoardConfigureColumns = "work.board.columns.configure";
    public const string OrchestrationConfigureSoftwareTemplate = "work.orchestration.software-template.configure";
    public const string ItemRead = "work.item.read";
    public const string ItemCreate = "work.item.create";
    public const string ItemComment = "work.item.comment";
    public const string ItemEstimate = "work.item.estimate";
    public const string ItemStart = "work.item.start";
    public const string ItemMove = "work.item.move";
    public const string ItemComplete = "work.item.complete";
    public const string ItemCancel = "work.item.cancel";
    public const string ItemReopen = "work.item.reopen";
    public const string ItemTransfer = "work.item.transfer";
    public const string ItemQualitySubmit = "work.item.quality.submit";
    public const string SprintRead = "work.sprint.read";
    public const string SprintCreate = "work.sprint.create";
    public const string SprintStart = "work.sprint.start";
    public const string SprintComplete = "work.sprint.complete";
    public const string SprintCancel = "work.sprint.cancel";
    public const string SprintManageScope = "work.sprint.scope.manage";
    public const string SprintManageCapacity = "work.sprint.capacity.manage";
    public const string SprintCarryOver = "work.sprint.carryover";
    public const string SprintReadReports = "work.sprint.report.read";
    public const string AutomationRead = "work.automation.read";
    public const string AutomationManage = "work.automation.manage";
    public const string PersonalTodoRead = "work.personal-todo.read.v1";
    public const string PersonalTodoAdd = "work.personal-todo.add.v1";
    public const string PersonalTodoReorder = "work.personal-todo.reorder.v1";
    public const string PersonalTodoRequeue = "work.personal-todo.requeue.v1";
    public const string PersonalTodoClaim = "work.personal-todo.claim.v1";
    public const string PersonalTodoComplete = "work.personal-todo.complete.v1";
    public const string PersonalTodoBlock = "work.personal-todo.block.v1";
    public const string PersonalTodoRelease = "work.personal-todo.release.v1";
    public const string PersonalTodoUpdate = "work.personal-todo.update.v1";
    public const string PersonalTodoArchive = "work.personal-todo.archive.v1";
    public const string PersonalTodoRestore = "work.personal-todo.restore.v1";

    public static IReadOnlySet<string> All { get; } = new HashSet<string>(
    [
        BoardRead, BoardCreate, BoardConfigureColumns,
        ItemRead, ItemCreate, ItemComment, ItemEstimate, ItemMove, ItemTransfer,
        SprintRead, SprintCreate,
        SprintManageScope, SprintManageCapacity, SprintCarryOver, SprintReadReports,
        OrchestrationRead, OrchestrationPreflight, OrchestrationStart, OrchestrationPause,
        OrchestrationResume, OrchestrationCancel, OrchestrationRetry,
        OrchestrationConfigureSoftwareTemplate, ExecutionRunV1,
        PersonalTodoRead, PersonalTodoAdd, PersonalTodoReorder, PersonalTodoRequeue,
        PersonalTodoClaim, PersonalTodoComplete, PersonalTodoBlock, PersonalTodoRelease,
        PersonalTodoUpdate, PersonalTodoArchive, PersonalTodoRestore
    ], StringComparer.Ordinal);
}

public static class WorkItemKinds
{
    public const string Initiative = "Initiative";
    public const string Epic = "Epic";
    public const string Story = "Story";
    public const string Task = "Task";
    public const string Bug = "Bug";
}

public static class WorkPriorities
{
    public const string Low = "Low";
    public const string Medium = "Medium";
    public const string High = "High";
    public const string Critical = "Critical";
}

public static class WorkStatuses
{
    public const string Backlog = "Backlog";
    public const string Ready = "Ready";
    public const string Assigned = "Assigned";
    public const string Running = "Running";
    public const string WaitingForApproval = "WaitingForApproval";
    public const string Completed = "Completed";
    public const string Failed = "Failed";
    public const string Blocked = "Blocked";
    public const string Cancelled = "Cancelled";
}

public static class WorkBoardKinds
{
    public const string Standard = "Standard";
    public const string Personal = "Personal";
}

public static class WorkAssignmentRelationships
{
    public const string DirectAssignee = "DirectAssignee";
    public const string AccountableOwner = "AccountableOwner";
    public const string StageAssignee = "StageAssignee";
    public const string StageAgent = "StageAgent";
}

/// <summary>
/// Canonical work-item shape shared by standard boards, personal boards, employee views,
/// and agent-facing compatibility adapters.
/// </summary>
public sealed record WorkItemResponse(
    Guid Id,
    Guid OrganizationId,
    Guid BoardId,
    Guid ColumnId,
    string Kind,
    string Title,
    string Description,
    string Status,
    string Priority,
    long Rank,
    long Revision,
    DateTimeOffset? DueDate,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    DateTimeOffset? ArchivedAt = null)
{
    public string? Identifier { get; init; }
    public Guid? SprintId { get; init; }
    public decimal? EstimatePoints { get; init; }
    public WorkItemProvenance? Provenance { get; init; }
    public WorkAssignmentMetadata? Assignment { get; init; }
    public WorkItemExecutionMetadata? Execution { get; init; }
}

public sealed record WorkItemProvenance(
    Guid? CreatedByOrganizationUserId,
    Guid? SourceConversationId,
    Guid? SourceMessageId,
    string? CorrelationId,
    string? CausationId,
    string? IdempotencyKey);

public sealed record WorkAssignmentMetadata(
    Guid? AssignedEmployeeId,
    Guid? AssignedAgentInstallationId,
    Guid? AccountableOrganizationUserId,
    IReadOnlyList<string> Relationships);

public sealed record WorkItemExecutionMetadata(
    string? ResultSummary,
    string? BlockReason,
    Guid? ClaimEventId,
    DateTimeOffset? ClaimExpiresAt);

public static class WorkAutomationOperations
{
    public const string Create = "Create";
    public const string Update = "Update";
    public const string Delete = "Delete";
}

public static class WorkAutomationTriggers
{
    public const string ItemCreated = "item.created";
    public const string ItemMoved = "item.moved";
    public const string ItemCompleted = "item.completed";
    public const string ItemCancelled = "item.cancelled";
    public const string ItemReopened = "item.reopened";
    public const string ItemSprintAssigned = "item.sprint.assigned";
    public const string ItemSprintRemoved = "item.sprint.removed";
    public const string ItemEstimateChanged = "item.estimate.changed";
    public const string CommentCreated = "comment.created";
}

public static class WorkItemEvents
{
    public const string Assigned = "work.item.assigned.v1";
}

public static class PersonalTodoEvents
{
    public const string Available = "com.csweet.work.personal-todo.available.v1";
}

public static class PersonalTodoCapabilities
{
    public const string Read = WorkManagementCapabilityNames.PersonalTodoRead;
    public const string Add = WorkManagementCapabilityNames.PersonalTodoAdd;
    public const string Reorder = WorkManagementCapabilityNames.PersonalTodoReorder;
    public const string Requeue = WorkManagementCapabilityNames.PersonalTodoRequeue;
    public const string Claim = WorkManagementCapabilityNames.PersonalTodoClaim;
    public const string Complete = WorkManagementCapabilityNames.PersonalTodoComplete;
    public const string Block = WorkManagementCapabilityNames.PersonalTodoBlock;
    public const string Release = WorkManagementCapabilityNames.PersonalTodoRelease;
    public const string Update = WorkManagementCapabilityNames.PersonalTodoUpdate;
    public const string Archive = WorkManagementCapabilityNames.PersonalTodoArchive;
    public const string Restore = WorkManagementCapabilityNames.PersonalTodoRestore;
}

public static class PersonalTodoStatuses
{
    public const string Ready = "Ready";
    public const string Running = "Running";
    public const string Completed = "Completed";
    public const string Blocked = "Blocked";
}

public sealed record PersonalTodoBoard(
    Guid BoardId,
    Guid OwnerOrganizationUserId,
    string OwnerDisplayName,
    Guid? ManagerOrganizationUserId,
    string? ManagerDisplayName,
    long Revision,
    IReadOnlyList<PersonalTodoItem> Items);

public sealed record PersonalTodoMention(
    Guid OrganizationUserId,
    string DisplayName,
    string EmployeeType);

public sealed record PersonalTodoItem(
    Guid Id,
    Guid BoardId,
    Guid OwnerOrganizationUserId,
    Guid CreatedByOrganizationUserId,
    string CreatedByDisplayName,
    string Title,
    string Description,
    string Status,
    string Priority,
    long Rank,
    long Revision,
    DateTimeOffset? DueDate,
    Guid? SourceConversationId,
    Guid? SourceMessageId,
    IReadOnlyList<PersonalTodoMention> Mentions,
    string? ResultSummary,
    string? BlockReason,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    DateTimeOffset? ArchivedAt = null);

public sealed record PersonalTodoDirectory(
    IReadOnlyList<PersonalTodoBoard> Boards,
    Guid? CurrentOrganizationUserId = null);

public sealed record AddPersonalTodoItemRequest(
    string Title,
    string? Description,
    string Priority,
    DateTimeOffset? DueDate,
    string IdempotencyKey,
    Guid? TargetOrganizationUserId = null,
    Guid? SourceConversationId = null,
    Guid? SourceMessageId = null,
    string? CorrelationId = null,
    string? CausationId = null);

public sealed record UpdatePersonalTodoItemRequest(
    Guid ItemId,
    string Title,
    string? Description,
    string Priority,
    DateTimeOffset? DueDate,
    long ExpectedRevision,
    string IdempotencyKey);

public sealed record ArchivePersonalTodoItemRequest(
    Guid ItemId,
    long ExpectedRevision,
    string IdempotencyKey);

public sealed record RestorePersonalTodoItemRequest(
    Guid ItemId,
    long ExpectedRevision,
    string IdempotencyKey);

public sealed record SetHumanPersonalTodoStatusRequest(
    Guid ItemId,
    string Status,
    long ExpectedRevision,
    string? Summary,
    string? Reason,
    string IdempotencyKey);

public sealed record ReorderPersonalTodoItemRequest(
    Guid ItemId,
    Guid? BeforeItemId,
    long ExpectedRevision,
    string IdempotencyKey);

public sealed record RequeuePersonalTodoItemRequest(
    Guid ItemId,
    long ExpectedRevision,
    string IdempotencyKey);

public sealed record ClaimPersonalTodoItemRequest(
    Guid EventId,
    string IdempotencyKey);

public sealed record PersonalTodoClaim(
    PersonalTodoItem? Item);

public sealed record CompletePersonalTodoItemRequest(
    Guid ItemId,
    Guid EventId,
    long ExpectedRevision,
    string? Summary,
    string IdempotencyKey);

public sealed record BlockPersonalTodoItemRequest(
    Guid ItemId,
    Guid EventId,
    long ExpectedRevision,
    string Reason,
    string IdempotencyKey);

public sealed record ReleasePersonalTodoItemRequest(
    Guid ItemId,
    Guid EventId,
    long ExpectedRevision,
    string IdempotencyKey);

public sealed record PersonalTodoAvailableEvent(
    Guid OwnerOrganizationUserId,
    Guid BoardId,
    Guid TriggerItemId);

public sealed record WorkBoardListRequest(string? Search = null, bool IncludeArchived = false);
public sealed record WorkBoardReference(Guid BoardId);
public sealed record WorkItemReference(Guid BoardId, Guid ItemId);
public sealed record CreateWorkBoardRequest(string Name, string? Description, string IdempotencyKey)
{
    public Guid? TeamId { get; init; }
    public string? Key { get; init; }
}
public sealed record WorkBoardSummary(
    Guid Id, string Name, string Description, bool IsDefault, bool IsArchived,
    long Revision, IReadOnlyList<string> AllowedActions)
{
    public Guid? TeamId { get; init; }
    public Guid? ManagerOrganizationUserId { get; init; }
    public string? Key { get; init; }
}
public sealed record WorkBoardColumn(
    Guid Id, string Name, string Category, int Position, string WipPolicy, int? WipLimit);
public sealed record WorkBoardColumnConfiguration(
    Guid? Id, string Name, string Category, string WipPolicy, int? WipLimit = null);
public sealed record ConfigureWorkBoardColumnsRequest(
    Guid BoardId,
    long ExpectedRevision,
    IReadOnlyList<WorkBoardColumnConfiguration> Columns,
    string IdempotencyKey);
public sealed record TeamRepositoryOptionsRequest(Guid TeamId);
public sealed record TeamRepositoryOption(
    Guid RepositoryId,
    string Name,
    string Provider,
    string CanonicalPath,
    string DefaultBranch,
    string DeliveryKind);
public sealed record SoftwareDevelopmentBrief(
    Guid RepositoryId,
    string EnvironmentProfile,
    IReadOnlyList<string> Requirements,
    IReadOnlyList<string> AcceptanceCriteria,
    IReadOnlyList<string>? Constraints = null)
{
    public Guid? QualityGateColumnId { get; init; }
    public IReadOnlyList<QualityFinding>? ReworkFindings { get; init; }
}
public sealed record SoftwareQualityBrief(
    Guid RepositoryId,
    string SourceCommitSha,
    string Provider,
    string DeliveryKind,
    Uri? PullRequestUrl,
    IReadOnlyList<string> Requirements,
    IReadOnlyList<string> AcceptanceCriteria,
    int QualityCycle,
    int MaximumReworkCycles,
    IReadOnlyList<string>? Constraints = null);
public sealed record WorkItemDeliverySpecification(
    Guid RepositoryId,
    IReadOnlyList<string> Requirements,
    IReadOnlyList<string> AcceptanceCriteria,
    IReadOnlyList<string>? Constraints = null)
{
    public Guid? QualityGateColumnId { get; init; }
    public IReadOnlyList<Guid> DependencyItemIds { get; init; } = [];
    public bool IsQaTrackingDefect { get; init; }
}
public sealed record WorkItem(
    Guid Id, Guid ColumnId, Guid? ParentItemId, Guid? SprintId, string Kind,
    string Title, string Description, string Status, string Priority,
    decimal? EstimatePoints, long Rank, long Revision, DateTimeOffset? DueDate,
    Guid? AssignedWorkerId = null,
    Guid? AssignedEmployeeId = null,
    Guid? AssignedInstallationId = null,
    string? AssignedDisplayName = null,
    SoftwareDevelopmentBrief? Development = null)
{
    public SoftwareQualityBrief? Quality { get; init; }
    public WorkItemDeliverySpecification? Delivery { get; init; }
    public string? Identifier { get; init; }
    public Guid? AccountableOrganizationUserId { get; init; }
    public IReadOnlyList<WorkStageAssignment> StageAssignments { get; init; } = [];
}
public sealed record WorkBoardDetail(
    WorkBoardSummary Board, IReadOnlyList<WorkBoardColumn> Columns, IReadOnlyList<WorkItem> Items);
public sealed record CreateWorkItemRequest(
    Guid BoardId, string Title, string? Description, string Kind, string Priority,
    Guid? ColumnId, Guid? ParentItemId, DateTimeOffset? DueDate, string IdempotencyKey)
{
    public WorkItemDeliverySpecification? Delivery { get; init; }
    public Guid? AccountableOrganizationUserId { get; init; }
    public IReadOnlyList<WorkStageAssignment> StageAssignments { get; init; } = [];
}
public sealed record AssignWorkItemRequest(
    Guid BoardId,
    Guid ItemId,
    Guid AssignedInstallationId,
    SoftwareDevelopmentBrief Development,
    long ExpectedRevision,
    string IdempotencyKey);
public sealed record WorkItemAssignedEvent(
    Guid BoardId,
    Guid ItemId,
    long AssignmentRevision,
    Guid AssignedInstallationId);
public sealed record CommentOnWorkItemRequest(
    Guid BoardId, Guid ItemId, string Body, string IdempotencyKey);
public sealed record WorkItemComment(
    Guid Id, Guid WorkItemId, string AuthorKind, Guid AuthorSubjectId,
    string AuthorDisplayName, string Body, long Revision,
    DateTimeOffset CreatedAt, DateTimeOffset? EditedAt);
public sealed record EstimateWorkItemRequest(
    Guid BoardId, Guid ItemId, decimal? EstimatePoints,
    long ExpectedItemRevision, string IdempotencyKey);
public sealed record MoveWorkItemRequest(
    Guid BoardId, Guid ItemId, Guid TargetColumnId,
    long ExpectedRevision, string IdempotencyKey);
public sealed record TransitionWorkItemRequest(
    Guid BoardId, Guid ItemId, long ExpectedRevision,
    string IdempotencyKey, Guid? TargetColumnId = null);
public sealed record TransferWorkItemRequest(
    Guid BoardId, Guid ItemId, Guid TargetBoardId, long ExpectedRevision,
    string IdempotencyKey, Guid? TargetColumnId = null);
public sealed record WorkItemTransfer(Guid SourceBoardId, Guid TargetBoardId, WorkItem Item);
public sealed record WorkSprint(
    Guid Id, Guid BoardId, string Name, string Goal, string Status,
    DateTimeOffset? StartsAt, DateTimeOffset? EndsAt, DateTimeOffset? StartedAt,
    DateTimeOffset? CompletedAt, decimal? CapacityPoints, int ItemCount,
    int CompletedItemCount, decimal PlannedPoints, decimal CompletedPoints, long Revision)
{
    public int? Sequence { get; init; }
}
public sealed record CreateWorkSprintRequest(
    Guid BoardId, string Name, string? Goal, DateTimeOffset? StartsAt,
    DateTimeOffset? EndsAt, string IdempotencyKey)
{
    public int? Sequence { get; init; }
}
public sealed record ChangeWorkSprintStateRequest(
    Guid BoardId, Guid SprintId, long ExpectedRevision, string IdempotencyKey);
public sealed record SetWorkItemSprintRequest(
    Guid BoardId, Guid ItemId, Guid? SprintId,
    long ExpectedItemRevision, string IdempotencyKey);
public sealed record SetWorkSprintCapacityRequest(
    Guid BoardId, Guid SprintId, decimal? CapacityPoints,
    long ExpectedSprintRevision, string IdempotencyKey);
public sealed record CarryOverWorkSprintRequest(
    Guid BoardId, Guid SourceSprintId, Guid TargetSprintId,
    IReadOnlyList<Guid>? ItemIds, long ExpectedSourceSprintRevision, string IdempotencyKey);
public sealed record WorkSprintCarryOver(
    Guid SourceSprintId, Guid TargetSprintId, IReadOnlyList<Guid> ItemIds, decimal CarriedPoints);
public sealed record WorkSprintSnapshotItem(
    Guid ItemId, string Kind, string Title, string Status,
    decimal? EstimatePoints, bool Completed);
public sealed record WorkSprintSnapshot(
    Guid Id, Guid SprintId, string SprintName, string Goal,
    DateTimeOffset? StartedAt, DateTimeOffset CompletedAt, decimal? CapacityPoints,
    int CommittedItemCount, int CompletedItemCount, decimal CommittedPoints,
    decimal CompletedPoints, IReadOnlyList<WorkSprintSnapshotItem> Items);
public sealed record WorkSprintMetricPoint(
    Guid Id, DateTimeOffset OccurredAt, string Reason, int ScopeItemCount,
    int CompletedItemCount, decimal ScopePoints, decimal CompletedPoints,
    decimal RemainingPoints);
public sealed record WorkSprintBurndownSeries(
    Guid SprintId, string SprintName, string Status, decimal? CapacityPoints,
    IReadOnlyList<WorkSprintMetricPoint> Points);
public sealed record WorkSprintForecast(
    Guid SprintId, string SprintName, decimal RemainingPoints,
    decimal AverageVelocity, decimal? ProjectedSprintsRequired, bool IsOverCapacity);
public sealed record WorkSprintReport(
    Guid BoardId, int CompletedSprintCount, decimal AverageVelocity,
    decimal TotalCompletedPoints, decimal? AverageCapacityUtilizationPercent,
    IReadOnlyList<WorkSprintSnapshot> Sprints,
    IReadOnlyList<WorkSprintBurndownSeries> Burndown,
    WorkSprintForecast? ActiveForecast);
public sealed record WorkAutomationRule(
    Guid Id, Guid BoardId, Guid AutomationIdentityId, string Name,
    string TriggerEventType, Guid? ConditionColumnId, string Action,
    Guid TargetColumnId, bool IsEnabled, bool HasExecutionGrant, long Revision,
    DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt);
public sealed record WorkAutomationExecution(
    Guid Id, Guid RuleId, Guid SourceActivityId, Guid WorkItemId, string Status,
    string RequiredAction, Guid? AuthorizingGrantId, long? AuthorizingGrantRevision,
    string? ErrorCode, string? ErrorMessage, DateTimeOffset CompletedAt);
public sealed record WorkAutomationDirectory(
    IReadOnlyList<WorkAutomationRule> Rules,
    IReadOnlyList<WorkAutomationExecution> RecentExecutions);
public sealed record ManageWorkAutomationRequest(
    Guid BoardId, string Operation, string IdempotencyKey, Guid? RuleId = null,
    string? Name = null, string? TriggerEventType = null,
    Guid? ConditionColumnId = null, string? Action = null,
    Guid? TargetColumnId = null, bool? IsEnabled = null, long? ExpectedRevision = null);

public static class QualityVerdicts
{
    public const string Passed = "Passed";
    public const string Failed = "Failed";
    public const string Blocked = "Blocked";
}

public static class QualityResultStatuses
{
    public const string Passed = "Passed";
    public const string Failed = "Failed";
    public const string Flaky = "Flaky";
    public const string NotRun = "NotRun";
    public const string Blocked = "Blocked";
}

public static class QualitySeverities
{
    public const string Low = "Low";
    public const string Medium = "Medium";
    public const string High = "High";
    public const string Critical = "Critical";
}

public sealed record QualityValidation(
    string Command, string Status, int ExitCode, string? DiagnosticExcerpt = null);

public sealed record QualityCriterionResult(
    string Criterion, string Status, string Evidence);

public sealed record QualityFinding(
    string Title,
    string Severity,
    string Description,
    IReadOnlyList<string> ReproductionSteps,
    string ExpectedBehavior,
    string ActualBehavior,
    string Evidence);

public sealed record SubmitQualityResultRequest(
    Guid BoardId,
    Guid ItemId,
    long AssignmentRevision,
    string SourceCommitSha,
    string Verdict,
    string Summary,
    IReadOnlyList<QualityCriterionResult> Criteria,
    IReadOnlyList<QualityValidation> Validations,
    IReadOnlyList<QualityFinding> Findings,
    IReadOnlyList<string> RemainingRisks,
    string IdempotencyKey);

public sealed record QualityRunResult(
    Guid QualityRunId,
    Guid WorkItemId,
    int QualityCycle,
    string Verdict,
    string PipelineStatus,
    string MergeStatus,
    IReadOnlyList<Guid> DefectItemIds,
    DateTimeOffset RecordedAt);

public static class DeliveryPipelineStatuses
{
    public const string Disabled = "Disabled";
    public const string Idle = "Idle";
    public const string Running = "Running";
    public const string Paused = "Paused";
    public const string Completed = "Completed";
}

public static class DeliveryMergeStatuses
{
    public const string None = "None";
    public const string Queued = "Queued";
    public const string Merged = "Merged";
    public const string Blocked = "Blocked";
}

public sealed record DeliveryPipelineConfiguration(
    Guid BoardId,
    Guid DeveloperInstallationId,
    Guid QualityInstallationId,
    Guid DevelopmentColumnId,
    Guid QualityColumnId,
    Guid DoneColumnId,
    Guid RepositoryId,
    string MergeStrategy,
    bool IsEnabled,
    string Status,
    long Revision)
{
    public Guid? ActiveSprintId { get; init; }
    public Guid? ActiveWorkItemId { get; init; }
    public string? LastError { get; init; }
}

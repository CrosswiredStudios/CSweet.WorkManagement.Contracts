namespace CSweet.WorkManagement.Contracts;

/// <summary>Canonical capability names for the agent-facing work-management protocol.</summary>
public static class WorkManagementCapabilityNames
{
    public const string BoardRead = "work.board.read";
    public const string BoardCreate = "work.board.create";
    public const string ItemRead = "work.item.read";
    public const string ItemCreate = "work.item.create";
    public const string ItemComment = "work.item.comment";
    public const string ItemEstimate = "work.item.estimate";
    public const string ItemMove = "work.item.move";
    public const string ItemComplete = "work.item.complete";
    public const string ItemCancel = "work.item.cancel";
    public const string ItemReopen = "work.item.reopen";
    public const string ItemTransfer = "work.item.transfer";
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

    public static IReadOnlySet<string> All { get; } = new HashSet<string>(
    [
        BoardRead, BoardCreate, ItemRead, ItemCreate, ItemComment, ItemEstimate,
        ItemMove, ItemComplete, ItemCancel, ItemReopen, ItemTransfer,
        SprintRead, SprintCreate, SprintStart, SprintComplete, SprintCancel,
        SprintManageScope, SprintManageCapacity, SprintCarryOver, SprintReadReports,
        AutomationRead, AutomationManage
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
    public const string Cancelled = "Cancelled";
}

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

public sealed record WorkBoardListRequest(string? Search = null, bool IncludeArchived = false);
public sealed record WorkBoardReference(Guid BoardId);
public sealed record CreateWorkBoardRequest(string Name, string? Description, string IdempotencyKey);
public sealed record WorkBoardSummary(
    Guid Id, string Name, string Description, bool IsDefault, bool IsArchived,
    long Revision, IReadOnlyList<string> AllowedActions);
public sealed record WorkBoardColumn(
    Guid Id, string Name, string Category, int Position, string WipPolicy, int? WipLimit);
public sealed record WorkItem(
    Guid Id, Guid ColumnId, Guid? ParentItemId, Guid? SprintId, string Kind,
    string Title, string Description, string Status, string Priority,
    decimal? EstimatePoints, long Rank, long Revision, DateTimeOffset? DueDate);
public sealed record WorkBoardDetail(
    WorkBoardSummary Board, IReadOnlyList<WorkBoardColumn> Columns, IReadOnlyList<WorkItem> Items);
public sealed record CreateWorkItemRequest(
    Guid BoardId, string Title, string? Description, string Kind, string Priority,
    Guid? ColumnId, Guid? ParentItemId, DateTimeOffset? DueDate, string IdempotencyKey);
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
    int CompletedItemCount, decimal PlannedPoints, decimal CompletedPoints, long Revision);
public sealed record CreateWorkSprintRequest(
    Guid BoardId, string Name, string? Goal, DateTimeOffset? StartsAt,
    DateTimeOffset? EndsAt, string IdempotencyKey);
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

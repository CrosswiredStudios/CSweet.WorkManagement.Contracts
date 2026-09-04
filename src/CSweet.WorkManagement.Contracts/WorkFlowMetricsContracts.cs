namespace CSweet.WorkManagement.Contracts;

public static class WorkFlowMetricConditionCodes
{
    public const string InsufficientCompletedSprints = "insufficient-completed-sprints";
    public const string InsufficientAttributedStages = "insufficient-attributed-stages";
    public const string SparseHistoricalBaseline = "sparse-historical-baseline";
}

public sealed record ReadWorkFlowMetricsRequest(Guid BoardId)
{
    public Guid? TeamId { get; init; }
    public Guid? WorkstreamId { get; init; }
    public DateTimeOffset? WindowStart { get; init; }
    public DateTimeOffset? WindowEnd { get; init; }
    public int CompletedSprintLimit { get; init; } = 6;
}

public sealed record WorkFlowTeamMetrics(
    int CompletedSprintCount,
    decimal AverageVelocity,
    decimal AverageCapacityUtilizationPercent,
    int CompletedItemCount,
    int CompletedStageCount,
    decimal ThroughputPerWeek,
    decimal MedianCycleTimeHours,
    decimal P85CycleTimeHours,
    int CurrentWorkInProgress,
    int PendingDemand,
    int BlockedCount,
    decimal BlockedDurationHours,
    int CarryoverItemCount,
    int ScopeChangeCount,
    int RetryCount,
    decimal ReworkRatePercent,
    decimal RemainingPoints,
    decimal AverageVelocityPoints,
    decimal ProjectedSprintCount,
    bool IsOverCapacity);

/// <summary>Per-principal work flow; deliberately not an individual point-velocity score.</summary>
public sealed record WorkFlowPrincipalMetrics(
    Guid OrganizationUserId,
    Guid? AgentInstallationId,
    string? RoleKey,
    int AssignedStageCount,
    int CompletedStageCount,
    decimal ThroughputPerWeek,
    decimal MedianCycleTimeHours,
    decimal P85CycleTimeHours,
    int CurrentWorkInProgress,
    int PendingDemand,
    int BlockedCount,
    decimal BlockedDurationHours,
    int RetryCount,
    decimal ReworkRatePercent,
    IReadOnlyList<string> ConditionCodes);

public sealed record WorkFlowMetricsReport(
    Guid BoardId,
    Guid? TeamId,
    Guid? WorkstreamId,
    DateTimeOffset WindowStart,
    DateTimeOffset WindowEnd,
    int CompletedSprintLimit,
    string SourceRevision,
    DateTimeOffset GeneratedAt,
    WorkFlowTeamMetrics Team,
    IReadOnlyList<WorkFlowPrincipalMetrics> Principals,
    IReadOnlyList<string> ConditionCodes);

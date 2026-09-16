namespace CSweet.WorkManagement.Contracts;

/// <summary>A complete, ordered decomposition. Story acceptance criteria describe observable phases.</summary>
public sealed record PersonalWorkPlanStory(string Key, string Title, string Description,
    IReadOnlyList<string> AcceptanceCriteria, IReadOnlyList<PersonalWorkPlanTask> Tasks);

public sealed record PersonalWorkPlanTask(string Key, string Title, string Description,
    IReadOnlyList<string> AcceptanceCriteria, string Execution = "Implementation");

public sealed record CreatePersonalWorkPlanRequest(Guid RootItemId, string EpicTitle,
    IReadOnlyList<PersonalWorkPlanStory> Stories, string IdempotencyKey)
{
    /// <summary>Fences planning performed before an executable personal item is claimed.</summary>
    public long? ExpectedRevision { get; init; }
}

public sealed record PersonalWorkPlan(Guid RootItemId, long RootRevision, IReadOnlyList<PersonalTodoItem> Items);

/// <summary>Report one small task while holding the owning request's execution claim.</summary>
public sealed record ReportPersonalWorkPlanTaskRequest(Guid RootItemId, Guid TaskItemId,
    long ExpectedRevision, string Status, string? Evidence, string IdempotencyKey);

public static class PersonalWorkPlanCapabilities
{
    public const string Create = "work.personal-plan.create.v1";
    public const string ReportTask = "work.personal-plan.report-task.v1";
}

public sealed record PersonalWorkPlanLink(Guid RootItemId, int Order, string Execution, string? Digest = null);

namespace CSweet.WorkManagement.Contracts;

/// <summary>New-install software workflow. Published policy revisions remain immutable.</summary>
public static class HierarchicalWorkflows
{
    public const string TaskIntegrationAction = "source-control.task.integrate.v1";
    public static (IReadOnlyList<WorkOrchestrationStageDefinition> Stages,
        IReadOnlyList<WorkOrchestrationTransitionDefinition> Transitions) Software(
        Guid ready, Guid development, Guid technicalReview, Guid quality, Guid done, int maximumQualityCycles = 3)
    {
        if (maximumQualityCycles is < 1 or > 10) throw new ArgumentOutOfRangeException(nameof(maximumQualityCycles));
        var retry = new WorkOrchestrationRetryPolicy();
        return ([
            new("ready", "Ready", WorkOrchestrationStageTypes.Queue, ready, "Wait for authorized scope and completed dependencies.", "{}", "{}", 30, null, retry),
            new("development", "Development", "MemberExecution", development,
                "Implement against the assigned story base and publish a PR to that story, or deliver an exact artifact revision.", "{}", "{}", 3600, null, retry),
            new("technical-review", "Technical Review", "MemberExecution", technicalReview,
                "Independently review the exact task commit and current story target. Approval authorizes trusted task integration.", "{}", "{}", 3600, null, retry),
            new("task-integration", "Technical Review", WorkOrchestrationStageTypes.TrustedPlatformAction, technicalReview,
                "Integrate only the independently approved task into its assigned story.", "{}", "{}", 300, 1, retry, TaskIntegrationAction),
            new("quality", "QA", "MemberExecution", quality,
                "Test the exact integrated story commit or delivered artifact revision against every task criterion. Every task requires QA.", "{}", "{}", 3600, null, retry),
            new("done", "Done", WorkOrchestrationStageTypes.Terminal, done, "Task QA passed.", "{}", "{}", 30, null, retry, null, true)
        ], [
            new("ready", "ready", "development"),
            new("development", "completed", "technical-review"),
            new("development", "artifact-delivered", "quality"),
            new("technical-review", "approved", "task-integration"),
            new("technical-review", "rejected", "development", maximumQualityCycles),
            new("task-integration", "merged", "quality"),
            new("quality", "passed", "done"),
            new("quality", "changes_requested", "development", maximumQualityCycles),
            new("quality", "failed", "development", maximumQualityCycles)
        ]);
    }
}

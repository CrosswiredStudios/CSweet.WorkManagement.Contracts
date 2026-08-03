using System.Reflection;

namespace CSweet.WorkManagement.Contracts.Tests;

public sealed class WorkManagementContractsTests
{
    [Fact]
    public void ActiveCapabilityNames_AreUniqueAndExcludeRetiredExecutionPaths()
    {
        var constants = typeof(WorkManagementCapabilityNames)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(field =>
                field.IsLiteral &&
                !field.IsInitOnly &&
                field.FieldType == typeof(string))
            .Select(field => (string)field.GetRawConstantValue()!)
            .ToHashSet(StringComparer.Ordinal);

        Assert.Equal(WorkManagementCapabilityNames.All.Count,
            WorkManagementCapabilityNames.All.Distinct(StringComparer.Ordinal).Count());
        Assert.All(WorkManagementCapabilityNames.All, capability => Assert.Contains(capability, constants));
        Assert.Contains(WorkManagementCapabilityNames.ItemMove, WorkManagementCapabilityNames.All);
        Assert.Contains(WorkManagementCapabilityNames.BoardConfigureColumns, WorkManagementCapabilityNames.All);
        Assert.Contains(WorkManagementCapabilityNames.OrchestrationConfigureSoftwareTemplate,
            WorkManagementCapabilityNames.All);
        Assert.DoesNotContain(WorkManagementCapabilityNames.SprintStart, WorkManagementCapabilityNames.All);
        Assert.DoesNotContain(WorkManagementCapabilityNames.AutomationManage, WorkManagementCapabilityNames.All);
    }

    [Theory]
    [InlineData(WorkManagementCapabilityNames.BoardRead)]
    [InlineData(WorkManagementCapabilityNames.OrchestrationStart)]
    [InlineData(WorkManagementCapabilityNames.OrchestrationRetry)]
    [InlineData(WorkManagementCapabilityNames.SprintCarryOver)]
    [InlineData(WorkManagementCapabilityNames.ExecutionRunV1)]
    public void CapabilityNames_UseWorkNamespace(string capability)
    {
        Assert.StartsWith("work.", capability, StringComparison.Ordinal);
    }

    [Fact]
    public void ExecutionContract_DoesNotLetWorkerChooseNextStage()
    {
        var properties = typeof(WorkExecutionOutcomeV1).GetProperties()
            .Select(property => property.Name)
            .ToHashSet(StringComparer.Ordinal);

        Assert.DoesNotContain("TargetStageKey", properties);
        Assert.Contains("OutcomeCode", properties);
        Assert.Contains("Disposition", properties);
    }

    [Fact]
    public void WorkVocabulary_UsesStableStringValues()
    {
        Assert.Equal("Task", WorkItemKinds.Task);
        Assert.Equal("Critical", WorkPriorities.Critical);
        Assert.Equal("Backlog", WorkStatuses.Backlog);
        Assert.Equal("WaitingForApproval", WorkStatuses.WaitingForApproval);
        Assert.Equal("Blocked", WorkStatuses.Blocked);
    }

    [Fact]
    public void DevelopmentAssignmentContracts_CarryStructuredAuthority()
    {
        var installationId = Guid.NewGuid();
        var repositoryId = Guid.NewGuid();
        var request = new AssignWorkItemRequest(
            Guid.NewGuid(),
            Guid.NewGuid(),
            installationId,
            new SoftwareDevelopmentBrief(
                repositoryId,
                "software-development-polyglot-v1",
                ["Implement the change."],
                ["All tests pass."]),
            4,
            "assignment-4");

        Assert.Equal(installationId, request.AssignedInstallationId);
        Assert.Equal(repositoryId, request.Development.RepositoryId);
        Assert.Equal("software-development-polyglot-v1", request.Development.EnvironmentProfile);
    }

    [Fact]
    public void QualityContracts_PinTheReviewedRevision()
    {
        var commit = new string('a', 40);
        var brief = new SoftwareQualityBrief(
            Guid.NewGuid(), commit, "GitHub", "PullRequest",
            new Uri("https://github.com/example/repo/pull/1"),
            ["Implement the behavior."], ["All checks pass."], 1, 3);
        var item = new WorkItem(
            Guid.NewGuid(), Guid.NewGuid(), null, null, WorkItemKinds.Story,
            "Ticket", "Description", WorkStatuses.Running, WorkPriorities.High,
            3, 1, 1, null)
        {
            Quality = brief
        };

        Assert.Equal(commit, item.Quality!.SourceCommitSha);
        Assert.Equal(3, item.Quality.MaximumReworkCycles);
    }
}

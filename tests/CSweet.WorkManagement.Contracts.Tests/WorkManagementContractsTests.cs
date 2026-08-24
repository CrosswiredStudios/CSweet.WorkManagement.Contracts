using System.Reflection;

namespace CSweet.WorkManagement.Contracts.Tests;

public sealed class WorkManagementContractsTests
{
    [Fact]
    public void UnassignedPrincipal_IsPublicForSafelyBlockedFutureStages()
    {
        Assert.Equal("Unassigned", WorkOrchestrationPrincipalKinds.Unassigned);
    }

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
        Assert.Contains(WorkManagementCapabilityNames.BoardConfigure, WorkManagementCapabilityNames.All);
        Assert.Contains(WorkManagementCapabilityNames.BoardConfigureColumns, WorkManagementCapabilityNames.All);
        Assert.Contains(WorkManagementCapabilityNames.OrchestrationConfigureSoftwareTemplate,
            WorkManagementCapabilityNames.All);
        Assert.DoesNotContain(WorkManagementCapabilityNames.SprintStart, WorkManagementCapabilityNames.All);
        Assert.DoesNotContain(WorkManagementCapabilityNames.AutomationManage, WorkManagementCapabilityNames.All);
        Assert.Contains(WorkManagementCapabilityNames.PersonalTodoAdd, WorkManagementCapabilityNames.All);
        Assert.Contains(WorkManagementCapabilityNames.PersonalTodoClaim, WorkManagementCapabilityNames.All);
        Assert.Contains(WorkManagementCapabilityNames.PersonalTodoActivate, WorkManagementCapabilityNames.All);
        Assert.Contains(WorkManagementCapabilityNames.PersonalTodoDefer, WorkManagementCapabilityNames.All);
        Assert.Contains(WorkManagementCapabilityNames.ItemCommentsRead, WorkManagementCapabilityNames.All);
    }

    [Fact]
    public void PersonalTodoContracts_KeepRuntimeClaimsPrivate()
    {
        Assert.Equal("com.csweet.work.personal-todo.available.v1", PersonalTodoEvents.Available);
        Assert.DoesNotContain(typeof(PersonalTodoItem).GetProperties(), property =>
            property.Name.Contains("Claim", StringComparison.Ordinal));
        Assert.DoesNotContain(typeof(PersonalTodoClaim).GetProperties(), property =>
            property.Name.Contains("Token", StringComparison.Ordinal));
        Assert.False(new ReleasePersonalTodoItemRequest(
            Guid.NewGuid(), Guid.NewGuid(), 1, "release").KeepInProgress);
        Assert.True(new ReleasePersonalTodoItemRequest(
            Guid.NewGuid(), Guid.NewGuid(), 1, "release") { KeepInProgress = true }.KeepInProgress);
        var reviewAt = DateTimeOffset.UtcNow.AddMinutes(5);
        var defer = new DeferPersonalTodoItemRequest(
            Guid.NewGuid(), Guid.NewGuid(), 1, reviewAt, "Awaiting a response.", Guid.NewGuid(), "defer");
        Assert.Equal(reviewAt, defer.NextReviewAt);
    }

    [Fact]
    public void WorkItemMentionsCarryFieldSpansAndAuthoritativeIdentity()
    {
        var personId = Guid.NewGuid();
        var input = new WorkItemMentionInput(
            personId, WorkItemMentionFields.Title, 5, 5);
        var span = new WorkItemMentionSpan(
            personId, "Matt", "Human", WorkItemMentionFields.Title, 5, 5, "@Matt");
        var request = new AddPersonalTodoItemRequest(
            "Tell @Matt a joke", null, WorkPriorities.Medium, null, "mention-1",
            Mentions: [input]) { StartInBacklog = true };

        Assert.Equal(personId, Assert.Single(request.Mentions!).OrganizationUserId);
        Assert.True(request.StartInBacklog);
        Assert.Equal("@Matt", span.DisplayText);
        Assert.Contains(span.Field, WorkItemMentionFields.All);
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
    public void TechnicalDelegation_IsAdviceWithoutPrincipalIdentity()
    {
        var recommendation = new WorkTechnicalDelegationRecommendation(
            "Development", "software-developer", ["repository.change.write"], "backend", false,
            "Requires implementation authority.");
        var properties = typeof(WorkTechnicalDelegationRecommendation).GetProperties()
            .Select(property => property.Name)
            .ToHashSet(StringComparer.Ordinal);

        Assert.Equal("software-developer", recommendation.RequiredRoleKey);
        Assert.DoesNotContain("EmployeeId", properties);
        Assert.DoesNotContain("AgentInstallationId", properties);
        var digest = new string('a', 64);
        var planning = new WorkItemPlanningSpecification(["Implement."], ["Tests pass."])
        {
            DelegationRecommendations = [recommendation],
            ArchitectureArtifactDigest = digest
        };
        Assert.Equal(digest, planning.ArchitectureArtifactDigest);
    }

    [Fact]
    public void SupportContracts_PinCommentsAndRetriesToAuthoritativeWork()
    {
        var sessionId = Guid.NewGuid();
        var comment = new CommentOnWorkItemRequest(Guid.NewGuid(), Guid.NewGuid(), "Guidance", "comment-1")
        {
            Kind = "ArchitectureGuidance",
            CoordinationSessionId = sessionId,
            CausationId = "support-1",
            ArtifactDigest = "sha256:abc"
        };
        var retry = new RetryWorkStageExecutionRequest(
            comment.BoardId, Guid.NewGuid(), Guid.NewGuid(), "retry-1")
        {
            ExpectedAssignmentRevision = 12
        };

        Assert.Equal(sessionId, comment.CoordinationSessionId);
        Assert.Equal(12, retry.ExpectedAssignmentRevision);
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

using System.Reflection;
using System.Text.Json;

namespace CSweet.WorkManagement.Contracts.Tests;

public sealed class WorkManagementContractsTests
{
    [Fact]
    public void TypedWorkCatalog_SeparatesHierarchyFromDomainPolicy()
    {
        var softwareStory = new WorkItemTypeDefinition(
            WorkItemTypeKeys.SoftwareStoryV1,
            "Software Story",
            WorkItemKinds.Story,
            [WorkBoardProfileKeys.SoftwareDeliveryV1],
            [WorkItemTypeKeys.SoftwareEpicV1],
            WorkItemTypeProviderKeys.Platform,
            [WorkItemApprovalPolicyKeys.SoftwareArchitectureReviewV1]);
        var generalStory = new WorkItemTypeDefinition(
            WorkItemTypeKeys.GeneralStoryV1,
            "Story",
            WorkItemKinds.Story,
            [WorkBoardProfileKeys.GeneralWorkV1],
            [WorkItemTypeKeys.GeneralEpicV1],
            WorkItemTypeProviderKeys.Platform,
            []);

        Assert.Equal(softwareStory.Kind, generalStory.Kind);
        Assert.NotEqual(softwareStory.Key, generalStory.Key);
        Assert.Equal(WorkItemApprovalPolicyKeys.SoftwareArchitectureReviewV1,
            Assert.Single(softwareStory.RequiredApprovalPolicyKeys));
        Assert.Empty(generalStory.RequiredApprovalPolicyKeys);
    }

    [Fact]
    public void WorkItemApproval_PinsTheExactPlanningRevisionAndProvenance()
    {
        var sessionId = Guid.NewGuid();
        var approval = new WorkItemApproval(
            Guid.NewGuid(),
            WorkItemApprovalPolicyKeys.SoftwareArchitectureReviewV1,
            WorkItemApprovalStatuses.Approved,
            7,
            Guid.NewGuid(),
            Guid.NewGuid(),
            "software-architect",
            new string('a', 64),
            sessionId,
            "The exact revision is technically executable.",
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow);
        var provenance = new WorkItemProposalProvenance(sessionId, new string('a', 64), "TASK-7");

        Assert.Equal(7, approval.PlanningRevision);
        Assert.Equal(approval.CoordinationSessionId, provenance.CoordinationSessionId);
        Assert.Equal(approval.ArtifactDigest, provenance.ArtifactDigest);
    }

    [Fact]
    public void ManagerLedPlanningContracts_RoundTripNextDirectiveAndClarifications()
    {
        var brief = new IncrementalProductBrief(
            Guid.NewGuid(), "plan-1", "Ship the first playable release",
            ["A player can complete the core loop."], ["The core loop is demonstrable."],
            new IncrementalEpic("EPIC-1", "Playable release", "Deliver a playable release", ["Playable"]),
            ArchitecturePlanningStages.Design)
        {
            ProductDecisions =
            [
                new ProductPlanningDecision(
                    "core-loop", "Three-lap arcade race", "PM product judgment",
                    new Dictionary<string, string> { ["charter"] = "3" })
            ],
            RespondsToArtifactDigest = new string('a', 64)
        };
        var decision = new ProductArchitectureDecision(
            "plan-1", new string('b', 64), "approved", "Complete and traceable", 0)
        {
            NextDirective = brief with
            {
                Stage = ArchitecturePlanningStages.Stories,
                ApprovedDesignDigest = new string('b', 64)
            }
        };

        var json = JsonSerializer.Serialize(decision);
        var roundTrip = JsonSerializer.Deserialize<ProductArchitectureDecision>(json);

        Assert.NotNull(roundTrip?.NextDirective);
        Assert.Equal(ArchitecturePlanningStages.Stories, roundTrip.NextDirective.Stage);
        Assert.Equal("core-loop", Assert.Single(roundTrip.NextDirective.ProductDecisions).QuestionId);
    }

    [Fact]
    public void ClarificationV2_SupportsMultipleStableQuestions()
    {
        var request = new SoftwareArchitectureClarificationRequest(
            "plan-1", ArchitecturePlanningStages.Design, "EPIC-1",
            [
                new("core-loop", "What is the core loop?", "Defines system boundaries.", "product-scope"),
                new("platform", "Which browsers are required?", "Defines compatibility.", "platform")
            ],
            new Dictionary<string, string> { ["board"] = "7" });

        Assert.Equal("software-architecture.question.v2", ArchitecturePlanningArtifactTypes.QuestionV2);
        Assert.Equal(2, request.Questions.Count);
        Assert.Equal(2, request.Questions.Select(x => x.Id).Distinct(StringComparer.Ordinal).Count());
    }

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

using System.Text.Json;
namespace CSweet.WorkManagement.Contracts.Tests;
public sealed class PlanningAssignmentCompatibilityTests
{
    [Fact]
    public void DeliveryDraftProposalsAreOptionalAndRoundTripWithoutGrantingExecution()
    {
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        var request = new ConfigureWorkDeliveryPlanRequest(Guid.NewGuid(), "Release", Guid.NewGuid(), [], [], [], "draft");
        var legacy = System.Text.Json.Nodes.JsonNode.Parse(JsonSerializer.Serialize(request, options))!;
        legacy.AsObject().Remove("taskAssignments");
        Assert.Empty(JsonSerializer.Deserialize<ConfigureWorkDeliveryPlanRequest>(legacy.ToJsonString(), options)!.TaskAssignments);
        var proposal = new WorkDeliveryTaskAssignment(Guid.NewGuid(), [new("quality", "Human", request.ManagerOrganizationUserId)]);
        var restored = JsonSerializer.Deserialize<ConfigureWorkDeliveryPlanRequest>(JsonSerializer.Serialize(request with { TaskAssignments = [proposal] }, options), options)!;
        Assert.Equal(proposal.ItemId, Assert.Single(restored.TaskAssignments).ItemId);
        Assert.Equal(request.ManagerOrganizationUserId, Assert.Single(restored.TaskAssignments[0].Stages).OrganizationUserId);
    }

    [Fact]
    public void LegacyPlanningRevisionPreservesAssignmentsAndNewRevisionRoundTripsExplicitReplacement()
    {
        var request = new ReviseWorkItemPlanningRequest(Guid.NewGuid(), Guid.NewGuid(), "Movement", "Implement movement", null,
            new WorkItemPlanningSpecification(["Movement"], ["Input moves player"], []), 1, 1, "planning");
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        var legacy = JsonSerializer.Deserialize<ReviseWorkItemPlanningRequest>(JsonSerializer.Serialize(request, options), options)!;
        Assert.Null(legacy.StageAssignments);
        var owner = Guid.NewGuid();
        var revised = request with { AccountableOrganizationUserId = owner,
            StageAssignments = [new WorkStageAssignment("execution", "AgentInstallation", owner, Guid.NewGuid())] };
        var restored = JsonSerializer.Deserialize<ReviseWorkItemPlanningRequest>(JsonSerializer.Serialize(revised, options), options)!;
        Assert.Equal(owner, restored.AccountableOrganizationUserId);
        Assert.Equal(revised.StageAssignments[0], Assert.Single(restored.StageAssignments!));
    }
}

using System.Text.Json;
namespace CSweet.WorkManagement.Contracts.Tests;
public sealed class PlanningAssignmentCompatibilityTests
{
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

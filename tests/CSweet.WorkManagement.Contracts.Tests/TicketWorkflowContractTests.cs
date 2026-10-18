using System.Text.Json;

namespace CSweet.WorkManagement.Contracts.Tests;

public sealed class TicketWorkflowContractTests
{
    [Fact]
    public void DeclaredWorkflowsRoundTripWithoutInventingStepsOrPredecessors()
    {
        var ticket=Guid.NewGuid();var employee=Guid.NewGuid();
        var request=new ConfigureWorkDeliveryPlanRequest(Guid.NewGuid(),"Agent-defined increment",employee,[],[],[],"declare") {
            TicketWorkflows=[new(ticket,[new("verify-requirements","source-review",new("verify-requirements","Human",employee))])]
        };
        var roundTrip=JsonSerializer.Deserialize<ConfigureWorkDeliveryPlanRequest>(JsonSerializer.Serialize(request))!;
        Assert.Empty(roundTrip.EpicItemIds);Assert.Empty(roundTrip.Assignments);
        var workflow=Assert.Single(roundTrip.TicketWorkflows);Assert.Equal(ticket,workflow.TicketId);
        Assert.Equal("verify-requirements",Assert.Single(workflow.Steps).Key);Assert.False(workflow.PromoteOnCompletion);
        Assert.Empty(workflow.FinalReleaseDependencyItemIds);Assert.Empty(workflow.CompleteContainerItemIds);
    }
}

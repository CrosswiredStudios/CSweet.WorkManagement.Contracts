namespace CSweet.WorkManagement.Contracts.Tests;

public sealed class IntegrationReworkPolicyTests
{
    [Xunit.Fact]
    public void NewSoftwarePolicyRoutesIntegrationConflictsBackToItsAuthor()
    {
        var workflow = HierarchicalWorkflows.Software(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        var transition = Xunit.Assert.Single(workflow.Transitions.Where(x => x.FromStageKey == "task-integration" && x.OutcomeCode == "integration_conflicts"));
        Xunit.Assert.Equal("development", transition.ToStageKey);
        Xunit.Assert.Equal(3, transition.MaximumTraversals);
    }
}

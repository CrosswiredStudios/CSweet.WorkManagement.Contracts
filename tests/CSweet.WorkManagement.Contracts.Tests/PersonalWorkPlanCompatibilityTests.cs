using System.Text.Json;

namespace CSweet.WorkManagement.Contracts.Tests;

public sealed class PersonalWorkPlanCompatibilityTests
{
    [Fact]
    public void LegacyPersonalTicketDefaultsToTaskWithoutAPlan()
    {
        var item = JsonSerializer.Deserialize<PersonalTodoItem>("{\"id\":\"00000000-0000-0000-0000-000000000001\",\"title\":\"Legacy\"}", new JsonSerializerOptions(JsonSerializerDefaults.Web))!;
        Assert.Equal("Task", item.Kind);
        Assert.Null(item.ParentItemId);
        Assert.Null(item.PlanRootId);
        Assert.Empty(item.AcceptanceCriteria);
    }

    [Fact]
    public void PlanningMetadataPreservesExistingRequirementsAndDependencies()
    {
        var root = Guid.NewGuid(); var dependency = Guid.NewGuid();
        var spec = new WorkItemPlanningSpecification(["Build game"], ["Playable"])
            { PersonalPlan = new(root, 3, "Validation"), DependencyItemIds = [dependency] };
        var copy = JsonSerializer.Deserialize<WorkItemPlanningSpecification>(JsonSerializer.Serialize(spec))!;
        Assert.Equal(root, copy.PersonalPlan!.RootItemId);
        Assert.Equal("Validation", copy.PersonalPlan.Execution);
        Assert.Equal(dependency, Assert.Single(copy.DependencyItemIds!));
        Assert.Equal("Playable", Assert.Single(copy.AcceptanceCriteria));
    }
}

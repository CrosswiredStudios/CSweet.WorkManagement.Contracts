using System.Text.Json;
using CSweet.WorkManagement.Contracts;

namespace CSweet.WorkManagement.Contracts.Tests;

public sealed class PersonalDependencyCompatibilityTests
{
    [Fact]
    public void Public_wire_preserves_exact_dependencies_and_legacy_waits_remain_readable()
    {
        var json = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        var dependency = new PersonalTodoDependency(PersonalTodoDependencyKinds.ResourceChange, Guid.NewGuid(), "Pending");
        var command = new DeferPersonalTodoItemRequest(Guid.NewGuid(), Guid.NewGuid(), 3,
            DateTimeOffset.UtcNow.AddHours(1), new string('x', 14000), null, "defer-exact") { Dependency = dependency };
        var restored = JsonSerializer.Deserialize<DeferPersonalTodoItemRequest>(JsonSerializer.Serialize(command, json), json)!;
        Assert.Equal(dependency, restored.Dependency); Assert.Equal(command.Reason, restored.Reason);
        var legacy = JsonSerializer.Deserialize<PersonalTodoWaitState>("{\"nextReviewAt\":\"2026-11-16T04:00:00Z\",\"reason\":\"Pending review\"}", json)!;
        Assert.Null(legacy.Dependency);
    }
}

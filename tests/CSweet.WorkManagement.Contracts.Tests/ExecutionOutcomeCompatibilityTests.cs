using System.Text.Json;
using CSweet.WorkManagement.Contracts;
using Xunit;

public sealed class ExecutionOutcomeCompatibilityTests
{
    [Fact]
    public void OldInputHasNoInventedOutcomeAndNewInputRoundTrips()
    {
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        var old = JsonSerializer.Deserialize<WorkExecutionInputV1>("{\"planningRevision\":1}", options)!;
        Assert.Empty(old.AllowedOutcomeCodes);
        var current = old with { AllowedOutcomeCodes = ["code-published", "completed"] };
        var copy = JsonSerializer.Deserialize<WorkExecutionInputV1>(JsonSerializer.Serialize(current, options), options)!;
        Assert.Equal(current.AllowedOutcomeCodes, copy.AllowedOutcomeCodes);
    }
}

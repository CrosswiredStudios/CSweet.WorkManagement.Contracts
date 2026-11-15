using System.Text.Json;
using Xunit;

namespace CSweet.WorkManagement.Contracts.Tests;

public sealed class DecisionAuthorshipTests
{
    [Fact]
    public void DecisionReadAuthorshipIsAdditiveAndOlderHostsRemainUnknown()
    {
        var legacy = JsonSerializer.Deserialize<DecisionRecord>("{}")!;
        Assert.Null(legacy.RequestedByOrganizationUserId); Assert.Null(legacy.DecidedByOrganizationUserId);
        var value = legacy with { RequestedByOrganizationUserId = Guid.NewGuid(), DecidedByOrganizationUserId = Guid.NewGuid() };
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        Assert.Equal(value, JsonSerializer.Deserialize<DecisionRecord>(JsonSerializer.Serialize(value, options), options));
    }
}

using System.Text.Json;
using Xunit;

namespace CSweet.WorkManagement.Contracts.Tests;

public sealed class ImportedProjectContractTests
{
    [Fact]
    public void LegacyProjectPlansDoNotImplicitlyAdoptAnExistingProject()
    {
        var legacy = JsonSerializer.Deserialize<WorkstreamPlanProposalV2Request>("{\"ProfileData\":{}}")!;
        Assert.Null(legacy.ExistingProjectId); Assert.Null(legacy.ExpectedProjectRevision);
        var bound = legacy with { ExistingProjectId = Guid.NewGuid(), ExpectedProjectRevision = 17 };
        var restored = JsonSerializer.Deserialize<WorkstreamPlanProposalV2Request>(JsonSerializer.Serialize(bound))!;
        Assert.Equal(bound.ExistingProjectId, restored.ExistingProjectId);
        Assert.Equal(17, restored.ExpectedProjectRevision);
    }
}

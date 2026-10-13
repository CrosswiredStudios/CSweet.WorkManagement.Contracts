using System.Text.Json;
using CSweet.WorkManagement.Contracts;
using Xunit;

public sealed class WorkArtifactDeliveryTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ContractWritesCanonicalNamesWithAnySerializerDefaults(bool web)
    {
        var delivery = new WorkArtifactDelivery(Guid.NewGuid(), Guid.NewGuid(), new string('a', 64))
            { RemainingRisks = ["Review engine compatibility"] };
        var output = JsonSerializer.SerializeToElement(delivery, web ? new(JsonSerializerDefaults.Web) : new JsonSerializerOptions());
        Assert.True(output.TryGetProperty("artifactId", out _));
        Assert.False(output.TryGetProperty("ArtifactId", out _));
        Assert.True(WorkArtifactDelivery.TryRead(output, out var read));
        Assert.Equal(delivery.ArtifactId, read.ArtifactId);
        Assert.Equal(delivery.RemainingRisks, read.RemainingRisks);
    }

    [Theory]
    [InlineData("artifactId", "revisionId", "sha256")]
    [InlineData("ArtifactId", "RevisionId", "Sha256")]
    [InlineData("ArtifactId", "revisionId", "SHA256")]
    public void ReadsPersistedOutcomeWithoutChangingItsIdentities(string artifactField, string revisionField, string digestField)
    {
        var artifact = Guid.NewGuid(); var revision = Guid.NewGuid(); var sha = new string('b', 64);
        var output = JsonSerializer.SerializeToElement(new Dictionary<string, object>
            { [artifactField] = artifact, [revisionField] = revision, [digestField] = sha, ["futureMetadata"] = "retained elsewhere" });
        Assert.True(WorkArtifactDelivery.TryRead(output, out var read));
        Assert.Equal(new WorkArtifactDelivery(artifact, revision, sha), read);
    }

    [Theory]
    [InlineData("{}")]
    [InlineData("null")]
    [InlineData("[]")]
    [InlineData("{\"artifactId\":\"invalid\"}")]
    public void RejectsMissingOrMalformedIdentity(string json) =>
        Assert.False(WorkArtifactDelivery.TryRead(JsonSerializer.Deserialize<JsonElement>(json), out _));

    [Theory]
    [InlineData("empty-artifact")]
    [InlineData("empty-revision")]
    [InlineData("short-digest")]
    [InlineData("nonhex-digest")]
    [InlineData("duplicate-identity")]
    [InlineData("duplicate-digest")]
    [InlineData("numeric-digest")]
    public void RejectsInvalidAndAmbiguousDelivery(string problem)
    {
        var artifact = problem == "empty-artifact" ? Guid.Empty : Guid.NewGuid();
        var revision = problem == "empty-revision" ? Guid.Empty : Guid.NewGuid();
        object sha = problem switch { "short-digest" => "abc", "nonhex-digest" => new string('z', 64), "numeric-digest" => 5, _ => new string('a', 64) };
        var fields = new Dictionary<string, object> { ["artifactId"] = artifact, ["revisionId"] = revision, ["sha256"] = sha };
        if (problem == "duplicate-identity") fields["ArtifactId"] = Guid.NewGuid();
        if (problem == "duplicate-digest") fields["Sha256"] = new string('b', 64);
        Assert.False(WorkArtifactDelivery.TryRead(JsonSerializer.SerializeToElement(fields), out _));
    }
}

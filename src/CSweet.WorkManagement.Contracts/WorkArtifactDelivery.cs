using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CSweet.WorkManagement.Contracts;

/// <summary>The exact document delivered by a task author for independent review.</summary>
public sealed record WorkArtifactDelivery(
    [property: JsonPropertyName("artifactId")] Guid ArtifactId,
    [property: JsonPropertyName("revisionId")] Guid RevisionId,
    [property: JsonPropertyName("sha256")] string Sha256)
{
    private static readonly JsonSerializerOptions ReadOptions = new() { PropertyNameCaseInsensitive = true };

    [JsonPropertyName("remainingRisks")]
    public IReadOnlyList<string> RemainingRisks { get; init; } = [];

    /// <summary>Creates canonical output regardless of the caller's serializer defaults.</summary>
    public JsonElement ToOutput()
    {
        if (!IsValid(this)) throw new InvalidOperationException("Artifact delivery requires nonempty identities and a SHA-256 digest.");
        return JsonSerializer.SerializeToElement(this);
    }

    /// <summary>Reads canonical and legacy PascalCase persisted outputs without accepting ambiguous fields.</summary>
    public static bool TryRead(JsonElement output, [NotNullWhen(true)] out WorkArtifactDelivery? delivery)
    {
        delivery = null;
        if (output.ValueKind != JsonValueKind.Object) return false;
        var fields = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var field in output.EnumerateObject())
        {
            if ((field.Name.Equals("artifactId", StringComparison.OrdinalIgnoreCase) ||
                 field.Name.Equals("revisionId", StringComparison.OrdinalIgnoreCase) ||
                 field.Name.Equals("sha256", StringComparison.OrdinalIgnoreCase) ||
                 field.Name.Equals("remainingRisks", StringComparison.OrdinalIgnoreCase)) && !fields.Add(field.Name))
                return false;
        }
        try
        {
            var candidate = output.Deserialize<WorkArtifactDelivery>(ReadOptions);
            if (candidate is null || !IsValid(candidate)) return false;
            delivery = candidate;
            return true;
        }
        catch (JsonException) { return false; }
    }

    private static bool IsValid(WorkArtifactDelivery delivery) =>
        delivery.ArtifactId != Guid.Empty && delivery.RevisionId != Guid.Empty &&
        delivery.Sha256 is { Length: 64 } && delivery.Sha256.All(Uri.IsHexDigit) &&
        delivery.RemainingRisks is not null && delivery.RemainingRisks.All(x => x is not null);
}

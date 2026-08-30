using System.Text.Json;

namespace CSweet.WorkManagement.Contracts;

public static class ToolchainCertificationStatuses
{
    public const string Pending = "Pending";
    public const string Running = "Running";
    public const string Certified = "Certified";
    public const string Failed = "Failed";
    public const string Expired = "Expired";
    public const string Revoked = "Revoked";
}

public static class DeliveryBuildStatuses
{
    public const string Queued = "Queued";
    public const string Claimed = "Claimed";
    public const string Running = "Running";
    public const string Succeeded = "Succeeded";
    public const string Failed = "Failed";
    public const string Blocked = "Blocked";
    public const string CancelRequested = "CancelRequested";
    public const string Cancelled = "Cancelled";
    public const string Exhausted = "Exhausted";
}

public sealed record ToolchainAdapterContribution(string Key, int Version, string DefinitionResource);

public sealed record ToolchainAdapterManifest(
    IReadOnlyList<ToolchainAdapterContribution> Provides,
    IReadOnlyList<string> Requires);

public sealed record ToolchainRecipeDefinition(
    string Key,
    IReadOnlyList<string> Operations,
    IReadOnlyList<string> TargetKeys,
    JsonElement ConfigurationSchema,
    IReadOnlyList<string> RequiredEnvironmentProfileKeys,
    IReadOnlyList<ToolchainCertificationFixture> CertificationFixtures);

public sealed record ToolchainCertificationFixture(
    string Key,
    string Resource,
    IReadOnlyList<string> ExpectedCheckKeys);

public sealed record ToolchainAdapterDefinition(
    Guid Id,
    string Key,
    int Version,
    string DisplayName,
    string ProviderPackageId,
    string ProviderPackageVersion,
    string DefinitionDigest,
    IReadOnlyList<ToolchainRecipeDefinition> Recipes,
    JsonElement RequiredExecutableVersions,
    JsonElement OutputPolicy,
    IReadOnlyList<string> SupportedContentTypes,
    IReadOnlyList<string> PreviewModes,
    DateTimeOffset CreatedAt);

public sealed record ToolchainEligibility(
    Guid DefinitionId,
    Guid ProviderInstallationId,
    Guid CertificationRunId,
    string EnvironmentProfileKey,
    string EnvironmentImageDigest,
    DateTimeOffset CertifiedAt,
    DateTimeOffset ExpiresAt,
    bool CompatibleCapacityOnline);

public sealed record ReadToolchainCatalogV2Request(
    string? ProfileKey = null,
    string? RecipeKey = null,
    IReadOnlyList<string>? TargetKeys = null,
    IReadOnlyList<string>? RequiredOperations = null);

public sealed record EligibleToolchainAdapter(
    ToolchainAdapterDefinition Definition,
    ToolchainEligibility Eligibility);

public sealed record RequestBuildV2Request(
    Guid WorkstreamId,
    Guid? TeamId,
    Guid ToolchainDefinitionId,
    Guid ProviderInstallationId,
    Guid RepositoryId,
    string SourceRevision,
    string RecipeKey,
    string TargetKey,
    JsonElement Configuration,
    int MaximumAttempts,
    string IdempotencyKey);

public sealed record ReadBuildV2Request(Guid? BuildId = null, Guid? WorkstreamId = null);

public sealed record ClaimBuildRequest(
    Guid BuildId,
    long ExpectedRevision,
    TimeSpan LeaseDuration,
    string IdempotencyKey);

public sealed record HeartbeatBuildRequest(
    Guid BuildId,
    Guid ClaimId,
    long ExpectedRevision,
    TimeSpan LeaseExtension,
    string IdempotencyKey);

public sealed record CancelBuildRequest(
    Guid BuildId,
    long ExpectedRevision,
    string Reason,
    string IdempotencyKey);

public sealed record BuildOutputManifestEntry(
    string RelativePath,
    string Sha256,
    long Size,
    string ContentType,
    string TypeKey);

public sealed record BuildExecutionProvenance(
    string ProviderPackageId,
    string ProviderPackageVersion,
    string AdapterDefinitionDigest,
    string EnvironmentImageDigest,
    JsonElement ToolVersions,
    string LockfileHash,
    IReadOnlyList<string> Commands,
    string SourceRevision,
    string NormalizedOutputManifestHash,
    IReadOnlyList<string> LogReferences);

public sealed record ReportBuildV2Request(
    Guid BuildId,
    Guid ClaimId,
    long ExpectedRevision,
    string Status,
    IReadOnlyList<BuildOutputManifestEntry> Outputs,
    BuildExecutionProvenance Provenance,
    IReadOnlyList<BuildValidationReport> Validations,
    string? FailureCode,
    string? FailureSummary,
    string IdempotencyKey);

public sealed record DeliveryBuildV2(
    Guid Id,
    Guid WorkstreamId,
    Guid? TeamId,
    Guid ToolchainDefinitionId,
    Guid ProviderInstallationId,
    Guid RepositoryId,
    string SourceRevision,
    string RecipeKey,
    string TargetKey,
    JsonElement Configuration,
    string DefinitionDigest,
    string Status,
    int Attempt,
    int MaximumAttempts,
    Guid? ClaimId,
    Guid? ExecutionNodeId,
    DateTimeOffset? LeaseExpiresAt,
    IReadOnlyList<BuildOutputManifestEntry> Outputs,
    BuildExecutionProvenance? Provenance,
    string? FailureCode,
    string? FailureSummary,
    long Revision,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public sealed record CreatePreviewV2Request(
    Guid WorkstreamId,
    Guid BuildId,
    string Mode,
    TimeSpan Lifetime,
    IReadOnlyList<string> EvidenceTypeKeys,
    string IdempotencyKey);

public sealed record ReadPreviewV2Request(Guid? PreviewId = null, Guid? WorkstreamId = null);

public sealed record DeliveryPreviewV2(
    Guid Id,
    Guid WorkstreamId,
    Guid BuildId,
    string Mode,
    string Status,
    string? AccessReference,
    IReadOnlyList<EvidenceReference> Evidence,
    DateTimeOffset ExpiresAt,
    DateTimeOffset CreatedAt);

public sealed record CertificationCheckResult(
    string Key,
    string Status,
    string Summary,
    IReadOnlyList<EvidenceReference> Evidence);

public sealed record ToolchainCertificationRun(
    Guid Id,
    Guid DefinitionId,
    Guid ProviderInstallationId,
    string EnvironmentProfileKey,
    string EnvironmentImageDigest,
    string Status,
    IReadOnlyList<CertificationCheckResult> Checks,
    string? FirstManifestHash,
    string? SecondManifestHash,
    DateTimeOffset CreatedAt,
    DateTimeOffset? CompletedAt,
    DateTimeOffset? ExpiresAt,
    string? RevocationReason);

public sealed record MediaProviderSummary(
    Guid InstallationId,
    string PackageId,
    string PackageVersion,
    IReadOnlyList<string> OperationTypeKeys,
    JsonElement ConfigurationSchemas,
    bool Eligible);

public sealed record ReadMediaProviderCatalogRequest(IReadOnlyList<string>? OperationTypeKeys = null);

public sealed record RequestMediaJobRequest(
    Guid WorkstreamId,
    Guid? WorkItemId,
    Guid ProviderInstallationId,
    string OperationTypeKey,
    JsonElement Input,
    JsonElement Configuration,
    string IdempotencyKey);

public sealed record ReadMediaJobRequest(Guid? JobId = null, Guid? WorkstreamId = null);
public sealed record CancelMediaJobRequest(Guid JobId, long ExpectedRevision, string Reason, string IdempotencyKey);
public sealed record ReferenceMediaAssetRequest(Guid AssetId, Guid WorkstreamId, string PurposeTypeKey);

public static class MediaOperationTypeKeys
{
    public const string ImageGenerateV1 = "media.image.generate.v1";
    public const string ImageEditV1 = "media.image.edit.v1";
    public const string VideoGenerateV1 = "media.video.generate.v1";
    public const string VideoEditV1 = "media.video.edit.v1";
    public const string AudioGenerateV1 = "media.audio.generate.v1";
    public const string AudioEditV1 = "media.audio.edit.v1";
    public const string TextureGenerateV1 = "media.texture.generate.v1";
    public const string Model3DGenerateV1 = "media.3d-model.generate.v1";
}

public sealed record MediaJob(
    Guid Id,
    Guid WorkstreamId,
    Guid? WorkItemId,
    Guid ProviderInstallationId,
    string OperationTypeKey,
    string Status,
    IReadOnlyList<MediaJobAsset> Assets,
    string? ErrorCode,
    string? ErrorMessage,
    long Revision,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public sealed record MediaJobAsset(
    Guid Id, string FileName, string ContentType, long Size, string Sha256,
    int? Width, int? Height, double? DurationSeconds);

public sealed record MediaAssetReference(
    Guid AssetId,
    Guid WorkstreamId,
    string TypeKey,
    string ContentType,
    string Sha256,
    long Size,
    string OpaqueReference,
    DateTimeOffset ExpiresAt);

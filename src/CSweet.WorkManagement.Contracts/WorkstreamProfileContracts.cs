using System.Text.Json;

namespace CSweet.WorkManagement.Contracts;

/// <summary>Generic project and portfolio capabilities owned by C-Sweet Core.</summary>
public static class WorkstreamCapabilityNames
{
    public const string ReadV1 = "platform.workstream.read.v1";
    public const string PlanProposeV2 = "platform.workstream.plan.propose.v2";
    public const string ChangeProposeV1 = "platform.workstream.change.propose.v1";
    public const string GateReadV1 = "platform.workstream.gate.read.v1";
    public const string GateSubmitV1 = "platform.workstream.gate.submit.v1";
    public const string GateDecideV1 = "platform.workstream.gate.decide.v1";
    public const string PortfolioReadV1 = "platform.management.portfolio.read.v1";
    public const string TeamRosterReadV2 = "platform.team-roster.read.v2";
}

public static class DecisionCapabilityNames
{
    public const string RequestV1 = "platform.decision.request.v1";
    public const string ReadV1 = "platform.decision.read.v1";
    public const string DecideV1 = "platform.decision.decide.v1";
}

public static class DeliveryEvidenceCapabilityNames
{
    public const string ToolchainCatalogReadV1 = "platform.toolchain.catalog.read.v1";
    public const string BuildRequestV1 = "platform.build.request.v1";
    public const string BuildReadV1 = "platform.build.read.v1";
    public const string ValidationReadV1 = "platform.validation.read.v1";
    public const string PreviewCreateV1 = "platform.preview.create.v1";
    public const string PreviewReadV1 = "platform.preview.read.v1";
    public const string EvaluationPlanV1 = "platform.evaluation-session.plan.v1";
    public const string EvaluationReadV1 = "platform.evaluation-session.read.v1";
    public const string EvaluationReportV1 = "platform.evaluation-session.report.v1";
    public const string ReleaseReadinessReadV1 = "platform.release-readiness.read.v1";
    public const string ReleaseReadinessSubmitV1 = "platform.release-readiness.submit.v1";
    public const string PublicationProposeV1 = "platform.publication.propose.v1";
}

/// <summary>Generic event names. Domain meaning is carried by profile and type keys.</summary>
public static class WorkstreamEventNames
{
    public const string ChangedV2 = "com.csweet.workstream.changed.v2";
    public const string TeamAssignmentChangedV1 = "com.csweet.workstream.team-assignment.changed.v1";
    public const string SupervisionChangedV1 = "com.csweet.workstream.supervision.changed.v1";
    public const string AuthorityEnvelopeChangedV1 = "com.csweet.workstream.authority-envelope.changed.v1";
    public const string GateRequestedV1 = "com.csweet.workstream.gate.requested.v1";
    public const string GateDecidedV1 = "com.csweet.workstream.gate.decided.v1";
    public const string DecisionRequestedV1 = "com.csweet.decision.requested.v1";
    public const string DecisionDecidedV1 = "com.csweet.decision.decided.v1";
    public const string ArtifactRevisionSubmittedV1 = "com.csweet.artifact.revision.submitted.v1";
    public const string ArtifactRevisionDecidedV1 = "com.csweet.artifact.revision.decided.v1";
    public const string ArtifactPackageSubmittedV1 = "com.csweet.artifact.package.submitted.v1";
    public const string ArtifactPackageDecidedV1 = "com.csweet.artifact.package.decided.v1";
    public const string WorkItemChangedV1 = "com.csweet.work.item.changed.v1";
    public const string SprintChangedV1 = "com.csweet.work.sprint.changed.v1";
    public const string ExecutionChangedV1 = "com.csweet.work.execution.changed.v1";
    public const string BuildPublishedV1 = "com.csweet.build.published.v1";
    public const string ValidationCompletedV1 = "com.csweet.validation.completed.v1";
    public const string EvaluationCompletedV1 = "com.csweet.evaluation-session.completed.v1";
    public const string MediaJobCompletedV1 = "com.csweet.media-job.completed.v1";
    public const string ReleaseReadinessChangedV1 = "com.csweet.release-readiness.changed.v1";
}

public static class WorkstreamProfileStatuses
{
    public const string Active = "Active";
    public const string Deprecated = "Deprecated";
    public const string Disabled = "Disabled";
}

/// <summary>An immutable, declarative specialization of the generic Workstream resource.</summary>
public sealed record WorkstreamProfileDefinition(
    string Key,
    int Version,
    string DisplayName,
    string MetadataSchemaJson,
    string LifecyclePolicyKey,
    string DefaultBoardProfileKey,
    string? AuthorityPolicyKey,
    string Status,
    string ProviderPackageId,
    string ProviderPackageVersion,
    string DefinitionDigest);

public sealed record WorkstreamProfileReference(string Key, int Version, string DefinitionDigest);

public sealed record WorkstreamProfileContribution(
    string Key,
    int Version,
    string DefinitionResource);

public sealed record WorkstreamProfileManifest(
    IReadOnlyList<WorkstreamProfileContribution> Provides,
    IReadOnlyList<string> Requires);

/// <summary>Broker-authenticated resource context. Callers cannot broaden this context.</summary>
public sealed record AgentWorkContext(
    Guid OrganizationId,
    Guid WorkstreamId,
    Guid? TeamId,
    Guid? BoardId,
    Guid? WorkItemId,
    Guid? MilestoneId,
    Guid? GateId,
    Guid CorrelationId,
    Guid? CausationId,
    string? ProfileKey);

public sealed record WorkstreamAuthorityEnvelope(
    decimal? MaximumBudgetVariance,
    int? MaximumScheduleVarianceDays,
    IReadOnlyList<string> AuthorizedStaffingRoleKeys,
    IReadOnlyList<string> HumanRequiredActionKeys,
    IReadOnlyList<string> AgentAuthorizedActionKeys,
    DateTimeOffset? ExpiresAt);

public sealed record WorkstreamMilestoneProposal(
    string Key,
    string Name,
    string LifecycleStage,
    DateTimeOffset? TargetDate,
    IReadOnlyList<string> RequiredEvidenceTypeKeys,
    IReadOnlyList<string> RequiredReviewerRoleKeys);

public sealed record WorkstreamPlanProposalV2Request(
    string Name,
    string Outcome,
    IReadOnlyList<string> SuccessCriteria,
    string LifecycleStage,
    string ManagerTitle,
    IReadOnlyList<string> RequiredCapabilities,
    Guid? StrategicObjectiveId,
    DateTimeOffset? TargetDate,
    decimal? ProposedBudgetAmount,
    string? ProposedBudgetCurrency,
    string Rationale,
    string IdempotencyKey,
    string ProfileKey,
    int ProfileVersion,
    JsonElement ProfileData,
    WorkstreamAuthorityEnvelope AuthorityEnvelope,
    IReadOnlyList<WorkstreamMilestoneProposal> InitialMilestones);

public sealed record ReadWorkstreamRequest(Guid WorkstreamId);

public sealed record WorkstreamDetail(
    Guid Id,
    string Name,
    string Outcome,
    IReadOnlyList<string> SuccessCriteria,
    string LifecycleStage,
    string Status,
    Guid AccountableManagerOrganizationUserId,
    DateTimeOffset? TargetDate,
    decimal? BudgetAmount,
    string? BudgetCurrency,
    string? ProfileKey,
    int? ProfileVersion,
    JsonElement? ProfileData,
    string? ProfileDefinitionDigest,
    long Revision);

public sealed record WorkstreamChangeProposalRequest(
    Guid WorkstreamId,
    long ExpectedRevision,
    string Summary,
    JsonElement Changes,
    string Rationale,
    string IdempotencyKey);

public sealed record WorkstreamTeamAssignment(
    Guid Id,
    Guid WorkstreamId,
    Guid TeamId,
    DateTimeOffset StartsAt,
    DateTimeOffset? EndsAt,
    long Revision);

public sealed record PortfolioSupervisionAssignment(
    Guid Id,
    Guid WorkstreamId,
    Guid SupervisorOrganizationUserId,
    string RoleKey,
    DateTimeOffset StartsAt,
    DateTimeOffset? EndsAt,
    long Revision);

public sealed record ReadPortfolioRequest(
    IReadOnlyList<Guid>? WorkstreamIds = null,
    bool IncludeClosed = false);

public sealed record PortfolioWorkstream(
    WorkstreamDetail Workstream,
    WorkstreamTeamAssignment? ActiveTeam,
    IReadOnlyList<WorkstreamGateSummary> Gates,
    IReadOnlyList<string> OpenDecisionSummaries);

public sealed record PortfolioResponse(IReadOnlyList<PortfolioWorkstream> Workstreams);

public static class WorkstreamGateStatuses
{
    public const string Pending = "Pending";
    public const string Submitted = "Submitted";
    public const string Approved = "Approved";
    public const string ChangesRequired = "ChangesRequired";
    public const string Rejected = "Rejected";
}

public sealed record EvidenceReference(
    string Kind,
    Guid ResourceId,
    Guid? RevisionId,
    string? Digest,
    string TypeKey,
    string Status);

public sealed record WorkstreamGateSummary(
    Guid Id,
    Guid WorkstreamId,
    string Key,
    string Name,
    string LifecycleStage,
    string Status,
    long Revision,
    DateTimeOffset? DueAt);

public sealed record ReadWorkstreamGatesRequest(Guid WorkstreamId, Guid? GateId = null);

public sealed record SubmitWorkstreamGateRequest(
    Guid WorkstreamId,
    Guid GateId,
    long ExpectedRevision,
    IReadOnlyList<EvidenceReference> Evidence,
    string Summary,
    string IdempotencyKey);

public sealed record DecideWorkstreamGateRequest(
    Guid WorkstreamId,
    Guid GateId,
    long ExpectedRevision,
    string Decision,
    string Rationale,
    IReadOnlyList<ReviewFinding> Findings,
    string IdempotencyKey);

public static class DecisionStatuses
{
    public const string Pending = "Pending";
    public const string Decided = "Decided";
    public const string Cancelled = "Cancelled";
    public const string Superseded = "Superseded";
}

public sealed record DecisionOption(string Id, string Label, string? Description);

public sealed record DecisionRequest(
    Guid WorkstreamId,
    string TypeKey,
    string Summary,
    string AuthorityRuleKey,
    IReadOnlyList<DecisionOption> Options,
    string RecommendedOptionId,
    IReadOnlyList<EvidenceReference> Evidence,
    DateTimeOffset? DueAt,
    string BlockingImpact,
    Guid? SupersedesDecisionId,
    string IdempotencyKey);

public sealed record ReadDecisionRequest(Guid? DecisionId = null, Guid? WorkstreamId = null, bool PendingOnly = false);

public sealed record DecideDecisionRequest(
    Guid DecisionId,
    long ExpectedRevision,
    string SelectedOptionId,
    string Rationale,
    string IdempotencyKey);

public sealed record DecisionRecord(
    Guid Id,
    Guid WorkstreamId,
    string TypeKey,
    string Summary,
    string AuthorityRuleKey,
    IReadOnlyList<DecisionOption> Options,
    string RecommendedOptionId,
    string? SelectedOptionId,
    string Status,
    string? Rationale,
    IReadOnlyList<EvidenceReference> Evidence,
    Guid? SupersedesDecisionId,
    Guid? SupersededByDecisionId,
    DateTimeOffset? DueAt,
    long Revision,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public static class ReviewFindingSeverities
{
    public const string Information = "Information";
    public const string Minor = "Minor";
    public const string Major = "Major";
    public const string Critical = "Critical";
}

public sealed record ReviewFinding(
    string Code,
    string Section,
    string Severity,
    bool Blocking,
    string Summary,
    string? RequiredFollowUp);

public sealed record StructuredArtifactDecisionRequest(
    Guid ArtifactId,
    Guid RevisionId,
    string RevisionDigest,
    string RubricTypeKey,
    string Disposition,
    IReadOnlyList<ReviewFinding> Findings,
    string? Comment,
    string IdempotencyKey,
    Guid? EvidenceConversationMessageId = null);

public sealed record TeamRosterV2Request(
    Guid? TeamId = null,
    Guid? WorkstreamId = null,
    int Page = 1,
    int PageSize = 50);

public sealed record GenericResourceEvent(
    Guid EventId,
    DateTimeOffset OccurredAt,
    AgentWorkContext Context,
    string AggregateType,
    Guid AggregateId,
    long Revision,
    string TypeKey,
    string Action,
    JsonElement Metadata);

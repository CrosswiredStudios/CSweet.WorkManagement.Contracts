namespace CSweet.WorkManagement.Contracts;

/// <summary>Public current-state recovery facts. They are evidence, not an execution grant.</summary>
public static class WorkRecoveryCapabilities { public const string Read = "work.recovery.read.v1"; }
public sealed record ReadWorkRecoveryRequest(Guid ItemId);
public sealed record WorkRecoveryFacts(Guid ItemId, Guid OwnerEmployeeId, Guid? OwnerInstallationId,
    Guid? BoardId, string FailureCode, string? FailedCapability, string Eligibility, string Basis, string Explanation)
{
    public Guid? FailureEventId { get; init; }
    public string? ContinuationCapability { get; init; }
    public bool CanResume => Eligibility is "Permitted" or "AlternativeAvailable";
}

/// <summary>A wake hint. Recipients must reread current work and operation facts; this event grants no retry.</summary>
public static class WorkRecoveryEvents { public const string Changed = "work.recovery.changed.v1"; }
public sealed record WorkRecoveryChangedEvent(Guid OwnerEmployeeId);

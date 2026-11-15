namespace CSweet.WorkManagement.Contracts;

/// <summary>Shared structural validation. Meaning, staffing and consultation choices remain with the manager.</summary>
public static class WorkIssueValidation
{
    public static void Validate(ReadWorkIssuesRequest r)
    {
        ArgumentNullException.ThrowIfNull(r);
        Id(r.BoardId); if (r.Page < 1 || r.PageSize is < 1 or > 100) throw new ArgumentException("Invalid discovery page.");
    }
    public static void Validate(ReconcileWorkIssuesRequest r)
    {
        ArgumentNullException.ThrowIfNull(r); Id(r.BoardId); Source(r.Source); Key(r.IdempotencyKey);
        if (r.Issues is null || r.Issues.Any(i => i is null) || r.Issues.Select(i => i.ObligationKey).Distinct().Count() != r.Issues.Count) throw new ArgumentException("Select each obligation once.");
        foreach (var i in r.Issues)
        {
            Key(i.ObligationKey); Text(i.Statement); Text(i.Detail); Text(i.NextAction);
            if (i.ScopeKeys is null || i.Links is null || i.ScopeKeys.Any(string.IsNullOrWhiteSpace) || i.ExpectedRevision < 0 ||
                i.Status is not ("Open" or "Resolved" or "Superseded") || i.Phase is not ("Working" or "AwaitingEvidence" or "AwaitingDecision" or "AwaitingReview")) throw new ArgumentException("Invalid issue lifecycle or scope.");
            foreach (var l in i.Links.Concat(i.ReopeningEvidence is null ? [] : new[] { i.ReopeningEvidence })) { ArgumentNullException.ThrowIfNull(l); Id(l.Id); if (l.Kind is not ("Coordination" or "Decision" or "Incident" or "WorkItem")) throw new ArgumentException("Unknown issue relationship."); }
            if (i.Status != "Open") Text(i.ResolutionEvidence);
        }
    }
    public static void Validate(CommissionWorkIssuesRequest r)
    {
        ArgumentNullException.ThrowIfNull(r); Id(r.BoardId); Id(r.TargetEmployeeId); Source(r.Source); Text(r.Question); Key(r.IdempotencyKey);
        if (r.IssueIds is null || r.IssueIds.Count == 0 || r.IssueIds.Distinct().Count() != r.IssueIds.Count || r.SupersededSessionIds is null) throw new ArgumentException("Select distinct issues for this commission.");
        foreach (var id in r.IssueIds.Concat(r.SupersededSessionIds)) Id(id);
    }
    private static void Source(WorkApprovedSource? source) { ArgumentNullException.ThrowIfNull(source); Id(source.DocumentId); Id(source.RevisionId); Text(source.Digest); }
    private static void Id(Guid value) { if (value == Guid.Empty) throw new ArgumentException("An existing identity is required."); }
    private static void Text(string? value) { if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Provide meaningful nonblank content."); }
    private static void Key(string value) { Text(value); if (value.Length > 160) throw new ArgumentException("Opaque identity exceeds the protocol identifier size."); }
}

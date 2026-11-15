namespace CSweet.WorkManagement.Contracts.Tests;

public sealed class WorkIssueValidationTests
{
    private static readonly WorkApprovedSource Source = new(Guid.NewGuid(), Guid.NewGuid(), "source");
    [Fact]
    public void AuthoredExplanationAndCommissionHaveNoBusinessTextCeiling()
    {
        var content = string.Concat(Enumerable.Repeat("Complete finding and evidence. ", 20000));
        WorkIssueValidation.Validate(new ReconcileWorkIssuesRequest(Guid.NewGuid(), Source,
            [new("opaque-obligation", content, content, []) { BriefTitle = "A concise responsibility", BriefSummary = "The manager owns resolution." }], "write"));
        WorkIssueValidation.Validate(new CommissionWorkIssuesRequest(Guid.NewGuid(), Guid.NewGuid(), [Guid.NewGuid()], Source, content, "commission"));
    }
    [Fact]
    public void InvalidIdentityDuplicateSelectionAndEvidenceFreeResolutionAreRejectedLocally()
    {
        var board = Guid.NewGuid(); var id = Guid.NewGuid();
        Assert.Throws<ArgumentException>(() => WorkIssueValidation.Validate(new ReadWorkIssuesRequest(Guid.Empty)));
        Assert.Throws<ArgumentException>(() => WorkIssueValidation.Validate(new CommissionWorkIssuesRequest(board, Guid.NewGuid(), [id,id], Source, "Question", "key")));
        Assert.Throws<ArgumentException>(() => WorkIssueValidation.Validate(new ReconcileWorkIssuesRequest(board, Source,
            [new("obligation", "Meaning", "Full detail", []) { Status = "Resolved" }], "write")));
        WorkIssueValidation.Validate(new ReconcileWorkIssuesRequest(board, Source,
            [new("obligation", "Meaning", "Full detail", []) { ReopeningEvidence = new("WorkItem", id) }], "new-evidence"));
    }
}

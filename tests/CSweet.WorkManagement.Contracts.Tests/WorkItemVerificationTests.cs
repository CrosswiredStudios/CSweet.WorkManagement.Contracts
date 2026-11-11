using System.Text.Json;
namespace CSweet.WorkManagement.Contracts.Tests;

public sealed class WorkItemVerificationTests
{
    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web);

    [Fact]
    public void Legacy_planning_has_no_declaration_and_writes_no_field()
    {
        var planning = new WorkItemPlanningSpecification(["Scene"], ["Scene renders"]);
        var json = JsonSerializer.Serialize(planning, Options);

        Assert.DoesNotContain("verifications", json);
        var restored = JsonSerializer.Deserialize<WorkItemPlanningSpecification>(json, Options)!;
        Assert.Null(restored.Verifications);
        Assert.False(WorkItemVerifications.IsDeclared(restored));
        Assert.Null(WorkItemVerifications.BrowserSmoke(restored));
    }

    [Fact]
    public void An_explicit_none_is_declared_and_survives_a_round_trip()
    {
        var planning = new WorkItemPlanningSpecification(["Pin"], ["Pinned"]) { Verifications = [] };

        var restored = JsonSerializer.Deserialize<WorkItemPlanningSpecification>(JsonSerializer.Serialize(planning, Options), Options)!;

        Assert.True(WorkItemVerifications.IsDeclared(restored));
        Assert.Empty(restored.Verifications!);
        Assert.Null(WorkItemVerifications.BrowserSmoke(restored));
    }

    [Fact]
    public void A_browser_requirement_round_trips_with_its_settings()
    {
        var planning = new WorkItemPlanningSpecification(["Break"], ["Ball breaks a block"])
        {
            Verifications = [new(WorkItemVerificationKinds.BrowserSmoke) { RequireCanvas = true, ViewportWidths = [1280] }]
        };

        var restored = JsonSerializer.Deserialize<WorkItemPlanningSpecification>(JsonSerializer.Serialize(planning, Options), Options)!;
        var browser = WorkItemVerifications.BrowserSmoke(restored)!;

        Assert.True(browser.RequireCanvas);
        Assert.False(browser.RequireWebGl);
        Assert.Equal(new[] { 1280 }, browser.ViewportWidths);
        Assert.Empty(WorkItemVerifications.Validate(restored.Verifications));
    }

    [Fact]
    public void Validation_rejects_unknown_kinds_duplicates_and_bad_widths()
    {
        Assert.Empty(WorkItemVerifications.Validate(null));
        Assert.Empty(WorkItemVerifications.Validate([]));
        Assert.Contains(WorkItemVerifications.Validate([new("pixel-diff")]), issue => issue.Contains("Unknown verification kind"));
        Assert.Contains(WorkItemVerifications.Validate([new(WorkItemVerificationKinds.BrowserSmoke), new(WorkItemVerificationKinds.BrowserSmoke)]),
            issue => issue.Contains("once"));
        Assert.Contains(WorkItemVerifications.Validate([new(WorkItemVerificationKinds.BrowserSmoke) { ViewportWidths = [100] }]),
            issue => issue.Contains("Viewport"));
        Assert.Contains(WorkItemVerifications.Validate([new(WorkItemVerificationKinds.BrowserSmoke) { ViewportWidths = [1280, 1280] }]),
            issue => issue.Contains("Viewport"));
    }
}

public sealed class BrowserSmokeReceiptTests
{
    private static readonly WorkItemVerificationRequirement Canvas = new(WorkItemVerificationKinds.BrowserSmoke) { RequireCanvas = true };
    private static readonly WorkItemVerificationRequirement Page = new(WorkItemVerificationKinds.BrowserSmoke);

    [Theory]
    [InlineData("node .csweet/browser-smoke.cjs dist --canvas", true, 0, true)]
    [InlineData("node .csweet/browser-smoke.cjs dist", true, 0, false)]
    [InlineData("node .csweet/browser-smoke.cjs dist --canvas", false, 1, false)]
    [InlineData("node .csweet/browser-smoke.cjs dist --canvas", true, 2, false)]
    [InlineData("node .csweet/browser-smoke.cjs --canvas", true, 0, false)]
    [InlineData("node .csweet/browser-smoke.cjs dist --canvas && echo ok", true, 0, false)]
    [InlineData("check_browser_build(buildDirectory=dist, requireCanvas=true)", true, 0, false)]
    [InlineData("npm test", true, 0, false)]
    public void Canvas_requirements_need_the_canvas_receipt(string command, bool succeeded, int exitCode, bool expected) =>
        Assert.Equal(expected, WorkItemVerifications.IsBrowserSmokeReceipt(command, succeeded, exitCode, Canvas));

    [Fact]
    public void Page_requirements_accept_either_receipt()
    {
        Assert.True(WorkItemVerifications.IsBrowserSmokeReceipt("node .csweet/browser-smoke.cjs dist", true, 0, Page));
        Assert.True(WorkItemVerifications.IsBrowserSmokeReceipt("node .csweet/browser-smoke.cjs build/web --canvas", true, 0, Page));
    }
}

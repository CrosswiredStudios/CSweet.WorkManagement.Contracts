using System.Reflection;

namespace CSweet.WorkManagement.Contracts.Tests;

public sealed class WorkManagementContractsTests
{
    [Fact]
    public void CapabilityNames_AreUniqueAndComplete()
    {
        var constants = typeof(WorkManagementCapabilityNames)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(field =>
                field.IsLiteral &&
                !field.IsInitOnly &&
                field.FieldType == typeof(string))
            .Select(field => (string)field.GetRawConstantValue()!)
            .ToHashSet(StringComparer.Ordinal);

        Assert.Equal(constants.Count, WorkManagementCapabilityNames.All.Count);
        Assert.Equal(
            constants.Order(StringComparer.Ordinal),
            WorkManagementCapabilityNames.All.Order(StringComparer.Ordinal));
    }

    [Theory]
    [InlineData(WorkManagementCapabilityNames.BoardRead)]
    [InlineData(WorkManagementCapabilityNames.ItemStart)]
    [InlineData(WorkManagementCapabilityNames.ItemComplete)]
    [InlineData(WorkManagementCapabilityNames.SprintCarryOver)]
    [InlineData(WorkManagementCapabilityNames.AutomationManage)]
    public void CapabilityNames_UseWorkNamespace(string capability)
    {
        Assert.StartsWith("work.", capability, StringComparison.Ordinal);
    }

    [Fact]
    public void WorkVocabulary_UsesStableStringValues()
    {
        Assert.Equal("Task", WorkItemKinds.Task);
        Assert.Equal("Critical", WorkPriorities.Critical);
        Assert.Equal("Backlog", WorkStatuses.Backlog);
        Assert.Equal("WaitingForApproval", WorkStatuses.WaitingForApproval);
        Assert.Equal("work.item.assigned.v1", WorkItemEvents.Assigned);
    }

    [Fact]
    public void DevelopmentAssignmentContracts_CarryStructuredAuthority()
    {
        var installationId = Guid.NewGuid();
        var connectionId = Guid.NewGuid();
        var request = new AssignWorkItemRequest(
            Guid.NewGuid(),
            Guid.NewGuid(),
            installationId,
            new SoftwareDevelopmentBrief(
                connectionId,
                "main",
                "software-development-polyglot-v1",
                ["Implement the change."],
                ["All tests pass."]),
            4,
            "assignment-4");

        Assert.Equal(installationId, request.AssignedInstallationId);
        Assert.Equal(connectionId, request.Development.RepositoryConnectionId);
        Assert.Equal("software-development-polyglot-v1", request.Development.EnvironmentProfile);
    }
}

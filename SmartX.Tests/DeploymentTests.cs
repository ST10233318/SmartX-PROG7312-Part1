using SmartX.Api.Models;
using SmartX.Api.Utilities;

namespace SmartX.Tests;

public class DeploymentTests
{
    [Fact]
    public void RecursiveValidationFindsInvalidNode()
    {
        var root = new DeploymentNode
        {
            Name = "Facility A",
            IsConfigured = true
        };

        var zone = new DeploymentNode
        {
            Name = "Zone 1",
            IsConfigured = true
        };

        var subZone = new DeploymentNode
        {
            Name = "Sub-Zone B",
            IsConfigured = true
        };

        var node = new DeploymentNode
        {
            Name = "Node 5",
            IsConfigured = false
        };

        subZone.Children.Add(node);
        zone.Children.Add(subZone);
        root.Children.Add(zone);

        var errors =
            DeploymentValidator.Validate(root);

        Assert.Single(errors);

        Assert.Contains(
            "Facility A -> Zone 1 -> Sub-Zone B -> Node 5",
            errors[0]);
    }
}
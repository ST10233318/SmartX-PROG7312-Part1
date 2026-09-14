namespace SmartX.Api.Models;

public class DeploymentNode
{
    public string Name { get; set; } = string.Empty;

    public bool IsConfigured { get; set; }

    public List<DeploymentNode> Children { get; set; } = new();
}